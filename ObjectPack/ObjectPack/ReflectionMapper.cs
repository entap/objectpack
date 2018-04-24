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
		/// プロパティを指定し、その型に適合するオブジェクトを生成する。
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
		/// プロパティに値を設定する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="propertyValue">設定する値</param>
		public void SetProperty(object target, string propertyName, object propertyValue)
		{
			if (target is IDictionary) {
				// 辞書型に追加する
				var elementType = ReflectionUtils.GetCollectionElementType(target.GetType());
				var value = ReflectionUtils.Convert(propertyValue, elementType);
				((IDictionary)target).Add(propertyName, value);
			} else {
				// オブジェクトのプロパティに値を設定
				ReflectionUtils.SetProperty(target, propertyName, propertyValue);
			}
		}

		/// <summary>
		/// プロパティを指定し、その型に適合する配列を生成する。
		/// </summary>
		/// <returns>生成された配列</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		public object CreateArray(object target, string propertyName)
		{
			Type type = GetPropertyType(target, propertyName);
			if (type.IsArray) {
				// 固定長配列の場合、一度、可変長配列を生成する
				return new ArrayList();
			} else if (ReflectionUtils.HasInterface(type, typeof(IList))) {
				// 指定されたプロパティには配列の機能がある。
				// 決まった型を生成する。
				return Activator.CreateInstance(type);
			} else {
				return null;
			}
		}

		/// <summary>
		/// 配列に値を追加する。
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
		/// オブジェクトのプロパティの型を取得する。
		/// </summary>
		/// <returns>型</returns>
		/// <param name="target">対象のオブジェクト</param>
		/// <param name="propertyName">対象のプロパティ名</param>
		Type GetPropertyType(object target, string propertyName)
		{
			if (target == null) {
				// ルートオブジェクトを返す。
				return typeof(T);
			}

			if (propertyName == null || ReflectionUtils.HasInterface(target.GetType(), typeof(IDictionary))) {
				// targetは、コレクションの要素
				return ReflectionUtils.GetCollectionElementType(target.GetType());
			}

			// targetは、オブジェクトのプロパティ
			var property = target.GetType().GetProperty(propertyName);
			if (property == null) {
				return null; // プロパティがない
			}
			return property.PropertyType;
		}
	}
}
