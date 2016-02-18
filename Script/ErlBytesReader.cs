using System;

public interface ErlBytesReader
{
    void bytesRead(ByteBuffer data);
	void bytesReadServer(ByteBuffer data);
}

