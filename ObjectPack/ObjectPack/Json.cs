using System.IO;
using System.Text;

namespace Entap.ObjectPack
{
	public static class Json
	{
		/// <summary>
		/// JSON形式の文字列をデコードし、オブジェクトにマッピングする。
		/// </summary>
		/// <returns>デコード結果</returns>
		/// <param name="jsonString">JSON形式の文字列</param>
		public static T Decode<T>(string jsonString) where T : new()
		{
			return (T)(new JsonDecoder(jsonString, new ReflectionMapper<T>())).Decode();
		}

		/// <summary>
		/// JSON形式の文字列をデコードし、オブジェクトにマッピングする。
		/// </summary>
		/// <returns>デコード結果</returns>
		/// <param name="reader">入力</param>
		public static T Decode<T>(TextReader reader) where T : new()
		{
			return (T)(new JsonDecoder(reader, new ReflectionMapper<T>())).Decode();
		}

		/// <summary>
		/// JSON形式の文字列をデコードし、コレクションとして取得する。
		/// </summary>
		/// <returns>デコード結果</returns>
		/// <param name="jsonString">JSON形式の文字列</param>
		public static object Decode(string jsonString)
		{
			return (new JsonDecoder(jsonString, new CollectionMapper())).Decode();
		}

		/// <summary>
		/// JSON形式の文字列をデコードし、コレクションとして取得する。
		/// </summary>
		/// <returns>デコード結果</returns>
		/// <param name="reader">入力</param>
		public static object Decode(TextReader reader)
		{
			return (new JsonDecoder(reader, new CollectionMapper())).Decode();
		}

		/// <summary>
		/// オブジェクトをJSON形式の文字列にエンコードする。
		/// </summary>
		/// <returns>エンコード結果</returns>
		/// <param name="obj">エンコードするオブジェクト</param>
		public static string Encode(object obj)
		{
			var sb = new StringBuilder();
			(new JsonEncoder(new StringWriter(sb))).Encode(obj);
			return sb.ToString();
		}
	}
}
