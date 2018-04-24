using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Entap.ObjectPack
{
	public class JsonEncoder
	{
		TextWriter _writer;

		/// <summary>
		/// <see cref="T:Entap.ObjectPack.JsonEncoder"/> クラスのインスタンスを初期化する。
		/// </summary>
		/// <param name="writer">出力</param>
		public JsonEncoder(TextWriter writer)
		{
			_writer = writer;
		}

		/// <summary>
		/// 指定されたオブジェクトをエンコードする。
		/// </summary>
		/// <param name="obj">対象のオブジェクト</param>
		public void Encode(object obj)
		{
			if (obj == null) {
				_writer.Write("null");
				return;
			}
			var type = obj.GetType();
			if (ReflectionUtils.HasInterface(type, typeof(IDictionary))) {
				EncodeDictionary((IDictionary)obj);
			} else if (ReflectionUtils.HasInterface(type, typeof(IList))) {
				EncodeArray((IList)obj);
			} else if (type == typeof(string)) {
				EncodeString((string)obj);
			} else if (ReflectionUtils.IsNumericType(type)) {
				EncodeNumber(obj);
			} else if (obj is bool) {
				EncodeBoolean((bool)obj);
			} else {
				EncodeObject(obj);
			}
		}

		/// <summary>
		/// 配列をエンコードする。
		/// </summary>
		/// <param name="array">配列</param>
		void EncodeArray(IList array)
		{
			_writer.Write("[");
			var n = array.Count;
			for (var i = 0; i < n; i++) {
				if (i != 0) {
					_writer.Write(",");
				}
				Encode(array[i]);
			}
			_writer.Write("]");
		}

		/// <summary>
		/// ディクショナリをエンコードする。
		/// </summary>
		/// <param name="dictionary">ディクショナリ</param>
		void EncodeDictionary(IDictionary dictionary)
		{
			_writer.Write("{");
			var enumerator = dictionary.GetEnumerator();
			var isHead = true;
			while (enumerator.MoveNext()) {
				if (isHead) {
					isHead = false;
				} else {
					_writer.Write(',');
				}
				Encode((string)enumerator.Entry.Key);
				_writer.Write(':');
				Encode(enumerator.Entry.Value);
			}
			_writer.Write("}");
		}

		/// <summary>
		/// 文字列をエンコードする。
		/// </summary>
		/// <param name="str">文字列</param>
		void EncodeString(string str)
		{
			_writer.Write('"');
			for (var i = 0; i < str.Length; i++) {
				_writer.Write(EscapeChar(str[i]));
			}
			_writer.Write("\"");
		}

		/// <summary>
		/// 文字をエスケープする。
		/// </summary>
		/// <returns>エスケープ結果の文字列</returns>
		/// <param name="c">文字</param>
		string EscapeChar(char c)
		{
			switch (c) {
				case '"':
					return "\\\"";
				case '\\':
					return "\\\\";
				case '\b':
					return "\\b";
				case '\f':
					return "\\f";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\t':
					return "\\t";
				default:
					return ('\x20' <= c && c <= '\x7f') ? c.ToString() : ("\\u" + ((int)c).ToString("x4"));
			}
		}

		/// <summary>
		/// 数値をエンコードする。
		/// </summary>
		/// <param name="number">数値</param>
		void EncodeNumber(object number)
		{
			var type = number.GetType();
			if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) {
				_writer.Write(Convert.ToDouble(number).ToString());
			} else {
				_writer.Write(Convert.ToInt64(number).ToString());
			}
		}

		/// <summary>
		/// 真偽値型をエンコードする。
		/// </summary>
		/// <param name="b">真偽値型の値</param>
		void EncodeBoolean(bool b)
		{
			_writer.Write(b ? "true" : "false");
		}

		/// <summary>
		/// オブジェクトをエンコードする。
		/// </summary>
		/// <param name="obj">オブジェクト</param>
		void EncodeObject(object obj)
		{
			var isHead = true;
			_writer.Write('{');
			EncodeProperties(obj, (name, value) => {
				if (isHead) {
					isHead = false;
				} else {
					_writer.Write(',');
				}
				Encode(name);
				_writer.Write(':');
				Encode(value);
			});
			_writer.Write('}');
		}

		/// <summary>
		/// オブジェクトのプロパティをエンコードする。
		/// </summary>
		/// <param name="obj">対象のオブジェクト</param>
		/// <param name="encoder">エンコードする関数.</param>
		void EncodeProperties(object obj, Action<string, object> encoder)
		{
			foreach (var field in obj.GetType().GetFields()) {
				encoder(field.Name, field.GetValue(obj));
			}
			foreach (var property in obj.GetType().GetProperties()) {
				encoder(property.Name, property.GetValue(obj, null));
			}
		}
	}
}
