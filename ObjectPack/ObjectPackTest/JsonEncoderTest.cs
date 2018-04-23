using NUnit.Framework;
using Entap.ObjectPack;
using System.Collections.Generic;

namespace Entap.ObjectPack.Test
{
	public class JsonEncoderTest
	{
		[Test]
		public void EncoderTest1()
		{
			Assert.AreEqual("0.12", Json.Encode(0.12));
			Assert.AreEqual("1234", Json.Encode(1234));
			Assert.AreEqual("\"abc\\t\\u1234\"", Json.Encode("abc\t\u1234"));
			Assert.AreEqual("true", Json.Encode(true));
			Assert.AreEqual("false", Json.Encode(false));
			Assert.AreEqual("null", Json.Encode(null));
		}

		[Test]
		public void EncoderTest2()
		{
			Assert.AreEqual("[0,1,2,3]", Json.Encode(new int[] { 0, 1, 2, 3 }));
			Assert.AreEqual("[0.1,1.1,2.2,3.3]", Json.Encode(new double[] { 0.1, 1.1, 2.2, 3.3 }));
			Assert.AreEqual("[\"xxx\",\"yyy\"]", Json.Encode(new List<string>() { "xxx", "yyy" }));
		}

		[Test]
		public void EncoderTest3()
		{
			// dic1
			var dic1 = new Dictionary<string, int>();
			dic1["a"] = 1;
			dic1["b"] = 2;
			dic1["c"] = 3;
			Assert.AreEqual("{\"a\":1,\"b\":2,\"c\":3}", Json.Encode(dic1));
		}

		[Test]
		public void EncoderTest4()
		{
			var m = new Model();
			m.i = 1;
			m.d = 1.2;
			m.s = "xxx";
			m.b = false;
			m.models = new Model[1];
			m.models[0].i = 2;
			m.models[0].d = 5.6;
			m.models[0].s = "yyy";
			m.models[0].b = true;
			Assert.AreEqual("{\"i\":1,\"d\":1.2,\"s\":\"xxx\",\"b\":false,\"models\":[{\"i\":2,\"d\":5.6,\"s\":\"yyy\",\"b\":true,\"models\":null}]}", Json.Encode(m));
		}

		struct Model {
			public int i;
			public double d;
			public string s;
			public bool b;
			public Model[] models;
		}
	}
}
