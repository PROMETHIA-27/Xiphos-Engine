//using System.Runtime.CompilerServices;

//namespace Xiphos.Resources;

//internal unsafe class SpanStream : Stream
//{
//    public override bool CanRead => true;
//    public override bool CanSeek => true;
//    public override bool CanWrite => true;
//    public override long Length { get; }
//    public override long Position { get; set; }

//    IntPtr _spanPtr;

//    GCHandle _spanHandle;

//    Span<byte> BackingSpan => *(Span<byte>*)_spanPtr;

//    public SpanStream(ref Span<byte> span)
//    {
//    }

//    public override void Flush() => throw new NotImplementedException();
//    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
//    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
//    public override void SetLength(long value) => throw new NotImplementedException();
//    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
//}
