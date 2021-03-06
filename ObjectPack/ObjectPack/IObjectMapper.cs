﻿using System;

namespace Entap.ObjectPack
{
	public interface IObjectMapper
	{
		/// <summary>
		/// プロパティを指定し、その型に適合するオブジェクトを生成する。
		/// </summary>
		/// <returns>生成されたオブジェクト</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		object CreateObject(object target, string propertyName);

		/// <summary>
		/// プロパティに値を設定する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="propertyValue">設定する値</param>
		void SetProperty(object target, string propertyName, object propertyValue);

		/// <summary>
		/// プロパティを指定し、その型に適合する配列を生成する。
		/// </summary>
		/// <returns>生成されたオブジェクト</returns>
		/// <param name="target">親オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		object CreateArray(object target, string propertyName);

		/// <summary>
		/// 配列に値を追加する。
		/// </summary>
		/// <param name="target">親オブジェクト</param>
		/// <param name="element">追加する値</param>
		void AddElement(object target, object element);
	}
}
