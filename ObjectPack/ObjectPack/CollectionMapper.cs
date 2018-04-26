using System;
using System.Collections;
using System.Collections.Generic;

namespace Entap.ObjectPack
{
	/// <summary>
	/// Hashtable/ArrayListへのマッパー
	/// </summary>
	public sealed class CollectionMapper : IObjectMapper
	{
		/// <summary>
		/// 親オブジェクトのプロパティに該当するオブジェクトを生成する。
		/// </summary>
		/// <returns>生成されたオブジェクト</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		public object CreateObject(object target, string propertyName)
		{
			return new Hashtable();
		}

		/// <summary>
		/// 親オブジェクトのプロパティに値を設定する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="propertyValue">設定する値</param>
		public void SetProperty(object target, string propertyName, object propertyValue)
		{
			((Hashtable)target).Add(propertyName, propertyValue);
		}

		/// <summary>
		/// 親オブジェクトのプロパティに該当する配列を生成する。
		/// </summary>
		/// <returns>生成されたオブジェクト</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		public object CreateArray(object target, string propertyName)
		{
			return new ArrayList();
		}

		/// <summary>
		/// 親オブジェクトの配列に値を追加する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="element">追加する値</param>
		public void AddElement(object target, object element)
		{
			((ArrayList)target).Add(element);
		}
	}
}
