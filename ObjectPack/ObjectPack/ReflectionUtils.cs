using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Entap.ObjectPack
{
	static internal class ReflectionUtils
	{
		/// <summary>
		/// 数値型の型の一覧
		/// </summary>
		static readonly List<Type> NumericTypes = new List<Type> {
			typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
			typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
		};

		/// <summary>
		/// クラスが実装しているインターフェイス
		/// </summary>
		static Dictionary<Type, HashSet<Type>> Interfaces = new Dictionary<Type, HashSet<Type>>();

		/// <summary>
		/// クラスが実装しているインターフェイス
		/// </summary>
		static Dictionary<Type, Type> CollectionElementTypes = new Dictionary<Type, Type>();

		/// <summary>
		/// 指定された型が数値型か判定する。
		/// </summary>
		/// <returns>指定された型が数値型なら<c>true</c>、そうでないなら<c>false</c></returns>
		/// <param name="type">型</param>
		public static bool IsNumericType(Type type)
		{
			return NumericTypes.Contains(type);
		}

		/// <summary>
		/// プリミティブな値を変換する。
		/// </summary>
		/// <returns>変換結果</returns>
		/// <param name="value">変換する値</param>
		/// <param name="type">変換先の型</param>
		public static object Convert(object value, Type type)
		{
			if (value == null) {
				return null;
			}
			if (value.GetType() == type || type == typeof(object)) {
				// 変換は不要
				return value;
			}
			if (type == typeof(bool)) {
				// 真偽値に変換
				var doubleValue = 0.0;
				if (value == null) {
					return false;
				} else if (value is string) {
					var strValue = (string)value;
					if (strValue == "true") {
						return true;
					}
					double.TryParse(strValue, out doubleValue);
				} else {
					doubleValue = System.Convert.ToDouble(value);
				}
				return doubleValue != 0.0;
			}
			if (type == typeof(string)) {
				// 文字列に変換
				return value.ToString();
			}
			if (IsNumericType(type)) {
				// 数値に変換
				var doubleValue = 0.0;
				if (value is bool) {
					doubleValue = (bool)value ? 1 : 0;
				} else if (value is string) {
					double.TryParse((string)value, out doubleValue);
				} else {
					doubleValue = System.Convert.ToDouble(value);
				}
				return System.Convert.ChangeType(doubleValue, type);
			}
			var toArrayMethod = value.GetType().GetMethod("ToArray", BindingFlags.Instance);
			if (toArrayMethod != null && type.IsArray) {
				// 可変長配列から固定長配列に変換
				return toArrayMethod.Invoke(value, new object[0]);
			}
			return null;
		}

		/// <summary>
		/// クラスがインターフェイスを実装しているか調べる。
		/// </summary>
		/// <returns>クラスがインターフェイスを実装しているなら<c>true</c>、そうでなければ<c>false</c></returns>
		/// <param name="classType">クラスの型</param>
		/// <param name="interfaceType">インターフェイスの型</param>
		public static bool HasInterface(Type classType, Type interfaceType)
		{
			if (!Interfaces.ContainsKey(classType)) {
				var typeSet = new HashSet<Type>();
				foreach (var type in classType.GetInterfaces()) {
					typeSet.Add(type);
				}
				Interfaces.Add(classType, typeSet);
			}
			return Interfaces[classType].Contains(interfaceType);
		}

		/// <summary>
		/// 配列の要素の型を取得する。
		/// </summary>
		/// <returns>配列の要素の型</returns>
		/// <param name="type">配列の型</param>
		public static Type GetCollectionElementType(Type type)
		{
			if (!CollectionElementTypes.ContainsKey(type)) {
				var parameters = type.GetMethod("Add").GetParameters();
				CollectionElementTypes[type] = parameters[parameters.Length - 1].ParameterType;
			}
			return CollectionElementTypes[type];
		}
	}
}
