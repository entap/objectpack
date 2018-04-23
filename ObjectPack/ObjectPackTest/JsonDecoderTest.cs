using NUnit.Framework;
using Entap.ObjectPack;
using System;
using System.Collections.Generic;

namespace Entap.ObjectPack.Test
{
	public class JsonDecoderTest
	{
		[Test]
		public void DecoderTest1()
		{
			Assert.AreEqual(0.12, Json.Decode("0.12"));
			Assert.AreEqual(1234, Json.Decode("1234"));
			Assert.AreEqual("xyz\t\u1234", Json.Decode("\"xyz\\t\\u1234\""));
			Assert.AreEqual(true, Json.Decode("true"));
			Assert.AreEqual(false, Json.Decode("false"));
			Assert.AreEqual(null, Json.Decode("null"));
		}

		[Test]
		public void DecoderTest2()
		{
			var m1 = Json.Decode<Model>("{\"i\":123,\"d\":1.0,\"s\":\"xyz\",\"b\":true}");
			Assert.AreEqual(123, m1.i);
			Assert.AreEqual(1.0, m1.d);
			Assert.AreEqual("xyz", m1.s);
			Assert.AreEqual(true, m1.b);

			var m2 = Json.Decode<List<int>>("[1,2,3]");
			Assert.AreEqual(1, m2[0]);
			Assert.AreEqual(2, m2[1]);
			Assert.AreEqual(3, m2[2]);
		}

		struct Model
		{
			public int i;
			public double d;
			public string s;
			public bool b;
			public List<Model> models;
		}
	}
}
