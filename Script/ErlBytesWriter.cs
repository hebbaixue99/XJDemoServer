using System;

public interface ErlBytesWriter
{
    void bytesWrite(ByteBuffer data);
	void bytesWriteServer(ByteBuffer data);
}

