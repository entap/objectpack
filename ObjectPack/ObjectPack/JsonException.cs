using System;

namespace Entap.ObjectPack
{
	[System.Serializable]
	public class JsonException : Exception
	{
		readonly long _position;

		/// <summary>
		/// 例外が発生した読み込み位置
		/// </summary>
		public long Position {
			get {
				return _position;
			}
		}

		/// <summary>
		/// <see cref="T:JsonTokenizerException"/> クラスのインスタンスを初期化する。
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="position">例外が発生した読み込み位置</param>
		public JsonException(string message, long position) : base(message)
		{
			_position = position;
		}
	}
}
