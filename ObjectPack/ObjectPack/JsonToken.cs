using System;
namespace Entap.ObjectPack
{
	/// <summary>
	/// トークンの種類
	/// </summary>
	public enum JsonTokenType
	{
		Number,
		String,
		Boolean,
		LeftSquareBracket,
		RightSquareBracket,
		LeftCurlyBracket,
		RightCurlyBracket,
		Comma,
		Colon,
		Null,
		End
	};

	/// <summary>
	/// JSONのトークン
	/// </summary>
	public struct JsonToken
	{
		/// <summary>
		/// トークンの種類
		/// </summary>
		public readonly JsonTokenType Type;

		/// <summary>
		/// トークンの値
		/// </summary>
		public readonly object Value;

		/// <summary>
		/// このトークンの位置
		/// </summary>
		public readonly int Position;

		/// <summary>
		/// <see cref="T:Entap.Json.JsonToken"/> クラスのインスタンスを初期化する。
		/// </summary>
		/// <param name="type">トークンの種類</param>
		/// <param name="value">トークンの値</param>
		/// <param name="position">トークンの位置</param>
		public JsonToken(JsonTokenType type, object value, int position)
		{
			Type = type;
			Value = value;
			Position = position;
		}
	}
}
