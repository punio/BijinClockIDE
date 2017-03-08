using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BijinClockIDE
{
	/// <summary>
	/// ストリームのWrapperクラス
	/// </summary>
	/// <remarks>
	/// Dispose 時に、内部ストリームの参照を外します
	/// </remarks>
	public class WrappingStream : Stream
	{
		Stream _streamBase;

		public WrappingStream(Stream streamBase)
		{
			if (streamBase == null)
			{
				throw new ArgumentNullException("streamBase");
			}
			this._streamBase = streamBase; //渡したStreamを内部ストリームとして保持
		}

		//Streamクラスのメソッドをオーバーライドして、内部ストリームの同じメソッドをそのまま呼ぶだけ
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			ThrowIfDisposed();
			return this._streamBase.ReadAsync(buffer, offset, count, cancellationToken);
		}
		public override long Seek(long offset, SeekOrigin origin) => this._streamBase.Seek(offset, origin);
		public override void SetLength(long value) => this._streamBase.SetLength(value);
		public override int Read(byte[] buffer, int offset, int count) => this._streamBase.Read(buffer, offset, count);
		public override void Write(byte[] buffer, int offset, int count) => this._streamBase.Write(buffer, offset, count);

		public override bool CanRead => this._streamBase.CanRead;

		public override bool CanSeek => this._streamBase.CanSeek;

		public override bool CanWrite => this._streamBase.CanWrite;

		public override long Length => this._streamBase.Length;

		public override long Position
		{
			get { return _streamBase.Position; }
			set { _streamBase.Position = value; }
		}

		public new Task<int> ReadAsync(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			return this._streamBase.ReadAsync(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._streamBase.Dispose();
				this._streamBase = null;  //disposeしたら内部ストリームをnullにして参照を外す
			}
			base.Dispose(disposing);
		}
		public override void Flush() => this._streamBase.Flush();
		private void ThrowIfDisposed()
		{
			if (this._streamBase == null)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}
	}
}
