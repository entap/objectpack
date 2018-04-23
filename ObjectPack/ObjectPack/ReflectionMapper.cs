using System;
using System.Collections;

namespace Entap.ObjectPack
{
	/// <summary>
	/// リフレクションを使ったマッパー
	/// </summary>
	public class ReflectionMapper<T> : IObjectMapper where T : new()
	{
		/// <summary>
		/// 親オブジェクトのプロパティに該当するオブジェクトを生成する。
		/// </summary>
		/// <returns>生成されたオブジェクト</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		public object CreateObject(object target, string propertyName)
		{
			Type type = GetPropertyType(target, propertyName);
			return type.IsPrimitive ? null : Activator.CreateInstance(type);
		}

		/// <summary>
		/// 親オブジェクトのプロパティに値を設定する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="propertyValue">設定する値</param>
		public void SetProperty(object target, string propertyName, object propertyValue)
		{
			if (target is IDictionary) {
				// コレクションに追加する
				var elementType = ReflectionUtils.GetCollectionElementType(target.GetType());
				var value = ReflectionUtils.Convert(propertyValue, elementType);
				((IDictionary)target).Add(propertyName, value);
			} else {
				// オブジェクトのプロパティに値を設定
				var field = target.GetType().GetField(propertyName);
				if (field != null) {
					var value = ReflectionUtils.Convert(propertyValue, field.FieldType);
					if (value != null) {
						field.SetValue(target, value);
					}
				}
				var property = target.GetType().GetProperty(propertyName);
				if (property != null) {
					var value = ReflectionUtils.Convert(propertyValue, property.PropertyType);
					if (value != null) {
						property.SetValue(target, value, null);
					}
				}
			}
		}

		/// <summary>
		/// 親オブジェクトのプロパティに該当する配列を生成する。
		/// </summary>
		/// <returns>生成されたオブジェクト</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		public object CreateArray(object target, string propertyName)
		{
			Type type = GetPropertyType(target, propertyName);
			if (type.IsArray) {
				return new ArrayList(); // 固定長配列の場合、一度、可変長配列を生成する
			} else {
				return ReflectionUtils.HasInterface(type, typeof(IList)) ? Activator.CreateInstance(type) : null;
			}
		}

		/// <summary>
		/// 親オブジェクトの配列に値を追加する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="element">追加する値</param>
		public void AddElement(object target, object element)
		{
			var elementType = ReflectionUtils.GetCollectionElementType(target.GetType());
			var value = ReflectionUtils.Convert(element, elementType);
			if (value != null) {
				((IList)target).Add(value);
			}
		}

		/// <summary>
		/// プロパティの型を取得する。
		/// </summary>
		/// <returns>型</returns>
		/// <param name="target">対象のオブジェクト</param>
		/// <param name="propertyName">対象のプロパティ名</param>
		Type GetPropertyType(object target, string propertyName)
		{
			if (target == null) {
				// ルートオブジェクト
				return typeof(T);
			}
			if (propertyName == null || ReflectionUtils.HasInterface(target.GetType(), typeof(IDictionary))) {
				// コレクションの要素
				return ReflectionUtils.GetCollectionElementType(target.GetType());
			}
			// オブジェクトのプロパティ
			var property = target.GetType().GetProperty(propertyName);
			if (property == null) {
				return null; // プロパティがない
			}
			return property.PropertyType;
		}
	}
}
