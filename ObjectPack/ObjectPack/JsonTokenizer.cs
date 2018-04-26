using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Entap.ObjectPack
{
	/// <summary>
	/// JSON形式の文字列をトークンに分解する。
	/// </summary>
	public sealed class JsonTokenizer
	{
		readonly TextReader _reader;
		int _position;

		/// <summary>
		/// 現在の読み込み位置を取得する。
		/// </summary>
		/// <value>現在の読み込み位置</value>
		public int Position {
			get {
				return _position;
			}
		}

		/// <summary>
		/// <see cref="T:Entap.Json.JsonParser"/> クラスのインスタンスを初期化する。
		/// </summary>
		/// <param name="reader">入力</param>
		public JsonTokenizer(TextReader reader)
		{
			_reader = reader;
			_position = 0;
		}

		/// <summary>
		/// 現在位置の文字を読み込む。
		/// </summary>
		/// <returns>現在位置の文字</returns>
		int PeekChar()
		{
			return _reader.Peek();
		}

		/// <summary>
		/// 指定された文字数だけ文字を読み込む。
		/// </summary>
		/// <returns>読み込んだ文字列</returns>
		/// <param name="n">文字数</param>
		string ReadChars(int n)
		{
			_position += n;
			var buffer = new char[n];
			_reader.ReadBlock(buffer, 0, n);
			return new string(buffer);
		}

		/// <summary>
		/// 読み込み位置を進める。
		/// </summary>
		void NextChar()
		{
			_position++;
			_reader.Read();
		}

		/// <summary>
		/// 条件が合致する間、文字を読み込む。
		/// </summary>
		/// <returns>読み込む条件</returns>
		/// <param name="predicate">条件</param>
		StringBuilder ReadWhile(Predicate<char> predicate)
		{
			var s = new StringBuilder();
			var c = PeekChar();
			while (c != -1 && predicate((char)c)) {
				s.Append((char)c);
				NextChar();
				c = PeekChar();
			}
			return s;
		}

		/// <summary>
		/// トークンを読み込む。
		/// </summary>
		/// <returns>トークン</returns>
		public JsonToken ReadToken()
		{
			ReadWhile(IsWhiteSpace);
			var c = PeekChar();
			switch (c) {
				case -1: // 終端
					return new JsonToken(JsonTokenType.End, null, _position);
				case '"': // 文字列
					return ReadString();
				case '+':
				case '-':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9': // 数値
					return ReadNumber();
				case '{': // {
					NextChar();
					return new JsonToken(JsonTokenType.LeftCurlyBracket, null, _position);
				case '}': // }
					NextChar();
					return new JsonToken(JsonTokenType.RightCurlyBracket, null, _position);
				case '[': // [
					NextChar();
					return new JsonToken(JsonTokenType.LeftSquareBracket, null, _position);
				case ']': // ]
					NextChar();
					return new JsonToken(JsonTokenType.RightSquareBracket, null, _position);
				case ':': // :
					NextChar();
					return new JsonToken(JsonTokenType.Colon, null, _position);
				case ',': // ,
					NextChar();
					return new JsonToken(JsonTokenType.Comma, null, _position);
				case 't': // true
					ReadKeyword("true");
					return new JsonToken(JsonTokenType.Boolean, true, _position);
				case 'f': // false
					ReadKeyword("false");
					return new JsonToken(JsonTokenType.Boolean, false, _position);
				case 'n': // null
					ReadKeyword("null");
					return new JsonToken(JsonTokenType.Null, null, _position);
				default: // 不明な文字
					throw new JsonException("Unexpected char: " + (char)c, _position);
			}
		}

		/// <summary>
		/// 文字が空白文字か判定する。
		/// </summary>
		/// <returns>空白文字なら<c>true</c>、そうでなければ<c>false</c></returns>
		/// <param name="c">文字</param>
		bool IsWhiteSpace(char c)
		{
			return c == '\x20' || c == '\r' || c == '\n' || c == '\t';
		}

		/// <summary>
		/// 文字が数値の構成文字か判定する。
		/// </summary>
		/// <returns>空白文字なら<c>true</c>、そうでなければ<c>false</c></returns>
		/// <param name="c">文字</param>
		bool IsNumberChar(char c)
		{
			return c == '+' || c == '-' || c == '.' || c == 'e' || c == 'E' || ('0' <= c && c <= '9');
		}

		/// <summary>
		/// 数値トークンを読み込む。
		/// </summary>
		/// <returns>トークン</returns>
		JsonToken ReadNumber()
		{
			var position = _position;
			var s = ReadWhile(IsNumberChar).ToString();
			var intValue = 0;
			if (int.TryParse(s, out intValue)) {
				return new JsonToken(JsonTokenType.Number, intValue, position);
			}
			var doubleValue = 0.0;
			if (double.TryParse(s, out doubleValue)) {
				return new JsonToken(JsonTokenType.Number, doubleValue, position);
			}
			throw new JsonException("Incorrect number format: " + s, position);
		}

		/// <summary>
		/// 文字列トークンを読み込む。
		/// </summary>
		/// <returns>トークン</returns>
		JsonToken ReadString()
		{
			var position = _position;
			NextChar(); // 引用符
			var s = new StringBuilder();
			while (true) {
				var c = PeekChar();
				NextChar();
				if (c == '"') {
					return new JsonToken(JsonTokenType.String, s.ToString(), position);
				}
				if (c == -1 || c == '\r' || c == '\n') {
					throw new JsonException("Unclosed string", position);
				}
				if (c == '\\') {
					s.Append(ReadEscapeSequence());
				} else {
					s.Append((char)c);
				}
			}
		}

		/// <summary>
		/// エスケープシーケンスを読み込む。
		/// </summary>
		/// <returns>読み込んだ文字列</returns>
		char ReadEscapeSequence()
		{
			var position = _position;
			var c = PeekChar();
			NextChar();
			switch (c) {
				case '"':
					return '\"';
				case '\\':
					return '\\';
				case '/':
					return '/';
				case 'b':
					return '\b';
				case 'f':
					return '\f';
				case 'n':
					return '\n';
				case 'r':
					return '\r';
				case 't':
					return '\t';
				case 'u':
					try {
						return Convert.ToChar(Convert.ToInt32(ReadChars(4), 16));
					} catch (Exception e) {
						throw new JsonException("Incorrect unicode escape sequence: " + e.Message, position);
					}
				default:
					throw new JsonException("Incorrect escape sequence: " + c, position);
			}
		}

		/// <summary>
		/// キーワードを読み込む。キーワードが不正な場合には例外を送出する。
		/// </summary>
		/// <param name="keyword">キーワード</param>
		void ReadKeyword(string keyword)
		{
			var position = _position;
			string s;
			try {
				s = ReadChars(keyword.Length);
			} catch (Exception) {
				throw new JsonException("Unexpected end of file", position);
			}
			if (s != keyword) {
				throw new JsonException("Incorrect keyword: " + s, position);
			}
		}
	}
}
