using NUnit.Framework;
using System;
using Entap.ObjectPack;
using System.IO;
using System.Text;

namespace Entap.ObjectPack.Test
{
	public class JsonTokenizerTest
	{
		[Test]
		public void NumberTest()
		{
			var tokenizer = new JsonTokenizer(new StringReader("0.1234 1234 0.12e-5"));
			var t1 = tokenizer.ReadToken();
			Assert.AreEqual(JsonTokenType.Number, t1.Type);
			Assert.AreEqual(0.1234, t1.Value);
			var t2 = tokenizer.ReadToken();
			Assert.AreEqual(JsonTokenType.Number, t2.Type);
			Assert.AreEqual(1234, t2.Value);
			var t3 = tokenizer.ReadToken();
			Assert.AreEqual(JsonTokenType.Number, t3.Type);
			Assert.AreEqual(0.12e-5, t3.Value);

			var tokenizer2 = new JsonTokenizer(new StringReader("12.12.12"));
			Assert.Throws<JsonException>(() => {
				tokenizer2.ReadToken();
			});
		}

		[Test]
		public void StringTest()
		{
			var tokenizer = new JsonTokenizer(new StringReader("\"abc\\n\\u3042\""));
			var t1 = tokenizer.ReadToken();
			Assert.AreEqual(JsonTokenType.String, t1.Type);
			Assert.AreEqual("abc\n\u3042", t1.Value);

			var tokenizer2 = new JsonTokenizer(new StringReader("\"unclosed string"));
			Assert.Throws<JsonException>(() => {
				tokenizer2.ReadToken();
			});

			var tokenizer3 = new JsonTokenizer(new StringReader("\"\\uBADBAD"));
			Assert.Throws<JsonException>(() => {
				tokenizer3.ReadToken();
			});
		}

		[Test]
		public void KeywordTest()
		{
			var tokenizer = new JsonTokenizer(new StringReader("[true,false,null,{}]"));
			Assert.AreEqual(JsonTokenType.LeftSquareBracket, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.Boolean, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.Comma, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.Boolean, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.Comma, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.Null, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.Comma, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.LeftCurlyBracket, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.RightCurlyBracket, tokenizer.ReadToken().Type);
			Assert.AreEqual(JsonTokenType.RightSquareBracket, tokenizer.ReadToken().Type);
		}
	}
}
