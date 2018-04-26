using System;
using System.Collections.Generic;
using System.IO;

namespace Entap.ObjectPack
{
	/// <summary>
	/// JSON形式の文字列をデコードする。
	/// </summary>
	public sealed class JsonDecoder
	{
		readonly JsonTokenizer _tokenizer;
		readonly IObjectMapper _mapper;
		object _currentTarget;
		string _currentPropertyName;

		/// <summary>
		/// <see cref="T:Entap.ObjectPack.JsonDecoder"/> クラスのインスタンスを初期化する。
		/// </summary>
		/// <param name="reader">入力</param>
		/// <param name="mapper">オブジェクトのマッピング</param>
		public JsonDecoder(TextReader reader, IObjectMapper mapper)
		{
			_tokenizer = new JsonTokenizer(reader);
			_mapper = mapper;
			_currentTarget = null;
			_currentPropertyName = null;
		}

		/// <summary>
		/// <see cref="T:Entap.ObjectPack.JsonDecoder"/> クラスのインスタンスを初期化する。
		/// </summary>
		/// <param name="s">JSON形式の文字列</param>
		public JsonDecoder(string s, IObjectMapper mapper)
			: this(new StringReader(s), mapper)
		{
		}

		/// <summary>
		/// デコードする。
		/// </summary>
		/// <returns>デコード結果のオブジェクト</returns>
		public object Decode()
		{
			var value = DecodeElement(_tokenizer.ReadToken());
			if (_tokenizer.ReadToken().Type != JsonTokenType.End) {
				throw new JsonException("Syntax error", _tokenizer.Position);
			}
			return value;
		}

		/// <summary>
		/// 要素をデコードする。
		/// </summary>
		/// <returns>デコード結果のオブジェクト</returns>
		/// <param name="token">先読みしたトークン</param>
		object DecodeElement(JsonToken token)
		{
			switch (token.Type) {
				case JsonTokenType.Number:
				case JsonTokenType.String:
				case JsonTokenType.Boolean:
				case JsonTokenType.Null:
					return token.Value;
				case JsonTokenType.LeftCurlyBracket:
					return DecodeObject();
				case JsonTokenType.LeftSquareBracket:
					return DecodeArray();
				default:
					throw new JsonException("Unexpected token", token.Position);
			}
		}

		/// <summary>
		/// オブジェクトをデコードする。
		/// </summary>
		/// <returns>デコード結果のオブジェクト</returns>
		object DecodeObject()
		{
			// 現在のオブジェクトとプロパティ名を退避
			var prevTarget = _currentTarget;
			var prevPropertyName = _currentPropertyName;

			// オブジェクトを生成
			var obj = _currentTarget = _mapper.CreateObject(_currentTarget, _currentPropertyName);

			// オブジェクトのプロパティを設定する
			while (true) {
				// プロパティ名を読み込む
				var token = _tokenizer.ReadToken();
				if (token.Type == JsonTokenType.RightCurlyBracket) {
					break; // {...,}という記述か、空のオブジェクト{}
				}
				if (token.Type != JsonTokenType.String) {
					throw new JsonException("JSON keys must be strings", token.Position);
				}
				_currentPropertyName = (string)token.Value;

				// ':'を読み込む
				token = _tokenizer.ReadToken();
				if (token.Type != JsonTokenType.Colon) {
					throw new JsonException("':' expected", token.Position);
				}

				// 値を読み込む
				var value = DecodeElement(_tokenizer.ReadToken());

				// プロパティを設定する
				_mapper.SetProperty(_currentTarget, _currentPropertyName, value);

				// 次のトークンが','なら次のプロパティを読み込む。
				// '}'ならオブジェクトの定義を終了。
				token = _tokenizer.ReadToken();
				if (token.Type == JsonTokenType.RightCurlyBracket) {
					break;
				}
				if (token.Type != JsonTokenType.Comma) {
					throw new JsonException("',' or '}' expected", token.Position);
				}
			}

			// 現在のオブジェクトとプロパティ名を戻す
			_currentTarget = prevTarget;
			_currentPropertyName = prevPropertyName;

			return obj;
		}

		/// <summary>
		/// 配列をデコードする。
		/// </summary>
		/// <returns>デコード結果のオブジェクト</returns>
		object DecodeArray()
		{
			// 現在のオブジェクトとプロパティ名を退避
			var prevTarget = _currentTarget;
			var prevPropertyName = _currentPropertyName;

			// オブジェクトを生成
			var obj = _mapper.CreateArray(_currentTarget, _currentPropertyName);

			// 配列の場合、プロパティ名はなし
			_currentTarget = obj;
			_currentPropertyName = null;

			// 各要素を読み込む
			while (true) {
				// 要素を読み込む
				var token = _tokenizer.ReadToken();
				if (token.Type == JsonTokenType.RightSquareBracket) {
					break; // [...,]という記述か、空の配列[]
				}
				var value = DecodeElement(token);

				// 配列に要素を追加
				_mapper.AddElement(obj, value);

				// 次のトークンが','なら次の要素を読み込む。']'なら配列の定義を終了
				token = _tokenizer.ReadToken();
				if (token.Type == JsonTokenType.RightSquareBracket) {
					break;
				}
				if (token.Type != JsonTokenType.Comma) {
					throw new JsonException("',' or ']' expected", token.Position);
				}
			}

			// 現在のオブジェクトとプロパティ名を戻す
			_currentTarget = prevTarget;
			_currentPropertyName = prevPropertyName;

			return obj;
		}
	}
}
