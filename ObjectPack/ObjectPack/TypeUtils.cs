using System;
using System.Collections.Generic;
using System.Reflection;

namespace Entap.ObjectPack
{
	using TypeToMembers = Dictionary<Type, Dictionary<string, MemberInfo>>;
	using TypeToType = Dictionary<Type, Type>;
	using TypeToTypeSet = Dictionary<Type, HashSet<Type>>;

	static internal class TypeUtils
	{
		/// <summary>
		/// 数値型の型の一覧
		/// </summary>
		static readonly HashSet<Type> NumberTypes = new HashSet<Type> {
			typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
			typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
		};

		/// <summary>
		/// クラスが継承しているインターフェイス
		/// </summary>
		static TypeToTypeSet Interfaces = new TypeToTypeSet();

		/// <summary>
		/// コレクションの要素の型
		/// </summary>
		static TypeToType CollectionElementTypes = new TypeToType();

		/// <summary>
		/// プロパティ名からプロパティ・フィールドへの写像
		/// </summary>
		static TypeToMembers Properties = new TypeToMembers();

		/// <summary>
		/// 指定された型が数値型か判定する。
		/// </summary>
		/// <returns>指定された型が数値型なら<c>true</c>、そうでないなら<c>false</c></returns>
		/// <param name="type">型</param>
		public static bool IsNumberType(Type type)
		{
			return NumberTypes.Contains(type);
		}

		/// <summary>
		/// プリミティブな値を変換する。
		/// </summary>
		/// <returns>変換結果</returns>
		/// <param name="value">変換元の値</param>
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
				return ConvertToBool(value);
			}
			if (type == typeof(string)) {
				// 文字列に変換
				return value.ToString();
			}
			if (IsNumberType(type)) {
				// 数値に変換
				return ConvertToNumber(value, type);
			}
			var toArrayMethod = GetToArrayMethod(value);
			if (toArrayMethod != null && type.IsArray) {
				return toArrayMethod.Invoke(value, new object[0]);
			}
			return null;
		}

		/// <summary>
		/// 真偽値に変換する。
		/// </summary>
		/// <returns>変換結果</returns>
		/// <param name="value">変換元の値</param>
		static bool ConvertToBool(object value)
		{
			if (value == null) {
				return false;
			}
			if (value is string) {
				var strValue = (string)value;
				if (strValue == "true") {
					return true;
				}
				var doubleValue = 0.0;
				double.TryParse(strValue, out doubleValue);
				return doubleValue != 0.0;
			}
			if (IsNumberType(value.GetType())) {
				return System.Convert.ToDouble(value) != 0.0;
			}
			return false;
		}

		/// <summary>
		/// 数値に変換する。
		/// </summary>
		/// <returns>変換結果</returns>
		/// <param name="value">変換元の値</param>
		static object ConvertToNumber(object value, Type type)
		{
			if (value == null) {
				return 0.0;
			}
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

		/// <summary>
		/// ToArrayメソッドを取得する
		/// </summary>
		/// <returns>ToArrayメソッド</returns>
		/// <param name="obj">オブジェクト</param>
		static MethodInfo GetToArrayMethod(object obj)
		{
			return obj.GetType().GetMethod("ToArray", BindingFlags.Instance);
		}

		/// <summary>
		/// クラスがインターフェイスを実装しているか調べる。
		/// </summary>
		/// <returns>クラスがインターフェイスを実装しているなら<c>true</c>、そうでなければ<c>false</c></returns>
		/// <param name="classType">クラスの型</param>
		/// <param name="interfaceType">インターフェイスの型</param>
		public static bool HasInterface(Type classType, Type interfaceType)
		{
			if (Interfaces.ContainsKey(classType)) {
				return Interfaces[classType].Contains(interfaceType);
			} else {
				var interfaceTypes = new HashSet<Type>();
				foreach (var type in classType.GetInterfaces()) {
					interfaceTypes.Add(type);
				}
				Interfaces.Add(classType, interfaceTypes);
				return interfaceTypes.Contains(interfaceType);
			}
		}

		/// <summary>
		/// 配列の要素の型を取得する。
		/// </summary>
		/// <returns>配列の要素の型</returns>
		/// <param name="type">配列の型</param>
		public static Type GetCollectionElementType(Type type)
		{
			if (CollectionElementTypes.ContainsKey(type)) {
				return CollectionElementTypes[type];
			} else {
				var parameters = type.GetMethod("Add").GetParameters();
				return CollectionElementTypes[type] = parameters[parameters.Length - 1].ParameterType;
			}
		}

		/// <summary>
		/// プロパティもしくはフィールドの型を取得する。
		/// </summary>
		/// <param name="type">対象の型</param>
		/// <param name="propertyName">プロパティ名</param>
		public static Type GetPropertyType(Type type, string propertyName)
		{
			// プロパティがある場合
			var property = type.GetProperty(propertyName);
			if (property != null) {
				return property.PropertyType;
			}

			// フィールドがある場合
			var field = type.GetField(propertyName);
			if (field != null) {
				return field.FieldType;
			}

			// プロパティもフィールドもない場合
			return null;
		}

		/// <summary>
		/// オブジェクトのプロパティ・フィールドの値を設定する。
		/// </summary>
		/// <param name="obj">対象のオブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="value">設定する値</param>
		public static void SetPropertyValue(object obj, string propertyName, object value)
		{
			var type = obj.GetType();

			// プロパティがある場合
			var property = type.GetProperty(propertyName);
			if (property != null) {
				property.SetValue(obj, value, null);
				return;
			}

			// フィールドがある場合
			var field = type.GetField(propertyName);
			if (field != null) {
				field.SetValue(obj, value);
				return;
			}
		}

		/// <summary>
		/// オブジェクトのプロパティ・フィールドの値を取得する。
		/// </summary>
		/// <param name="obj">対象のオブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		public static object GetPropertyValue(object obj, string propertyName)
		{
			var type = obj.GetType();

			// プロパティがある場合
			var property = type.GetProperty(propertyName);
			if (property != null) {
				return property.GetValue(obj, null);
			}

			// フィールドがある場合
			var field = type.GetField(propertyName);
			if (field != null) {
				return field.GetValue(obj);
			}

			return null;
		}
	}
}
