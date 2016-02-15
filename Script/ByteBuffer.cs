using System;
using System.Text;
 

public class ByteBuffer : ICloneable
{
    private int _position;
    private int _top;
    private byte[] array;
    public uint bytesAvailable;
    public const int CAPACITY = 0x20;
    public static readonly byte[] EMPTY_ARRAY = new byte[0];
    public const string EMPTY_STRING = "";
    public const int MAX_DATA_LENGTH = 0x64000;

    public ByteBuffer() : this(0x20)
    {
    }

    public ByteBuffer(int capacity)
    {
        if (capacity < 1)
        {
            throw new Exception(this + " <init>, invalid capatity:" + capacity);
        }
        this.array = new byte[capacity];
        this.top = 0;
        this.position = 0;
    }

    public ByteBuffer(byte[] data)
    {
        if (data == null)
        {
            throw new Exception(this + " <init>, null data");
        }
        this.array = data;
        this.top = data.Length;
        this.position = 0;
    }

    public ByteBuffer(byte[] data, int index, int length)
    {
        if (data == null)
        {
            throw new Exception(this + " <init>, null data");
        }
        if ((index < 0) || (index > data.Length))
        {
            throw new Exception(this + " <init>, invalid index:" + index);
        }
        if ((length < 0) || (data.Length < (index + length)))
        {
            throw new Exception(this + " <init>, invalid length:" + length);
        }
        this.array = data;
        this.top = index + length;
        this.position = index;
    }

    public object bytesRead(ByteBuffer data)
    {
        int len = data.readLength() - 1;
        if (len < 0)
        {
            return null;
        }
        if (len > 0x64000)
        {
            throw new Exception(this + " bytesRead, data overflow:" + len);
        }
        if (this.array.Length < len)
        {
            this.array = new byte[len];
        }
        if (len > 0)
        {
            data.read(this.array, 0, len);
        }
        this.top = len;
        this.position = 0;
        return this;
    }

    public void bytesWrite(ByteBuffer data)
    {
        data.writeData(this.array, this.position, this.top - this.position);
    }

    public int capacity()
    {
        return this.array.Length;
    }

    public bool checkClass(object obj)
    {
        return (obj is ByteBuffer);
    }

    public void clear()
    {
        this.top = 0;
        this.position = 0;
    }

    public object Clone()
    {
        object obj2;
        try
        {
            ByteBuffer buffer = (ByteBuffer) base.MemberwiseClone();
            byte[] array = buffer.array;
            buffer.array = new byte[buffer.top];
            Array.Copy(array, 0, buffer.array, 0, buffer.top);
            obj2 = buffer;
        }
        catch (Exception exception)
        {
            throw new Exception(this + " clone, capacity=" + this.array.Length, exception);
        }
        return obj2;
    }

    public bool equals(object obj)
    {
        if (this != obj)
        {
            if (!this.checkClass(obj))
            {
                return false;
            }
            ByteBuffer buffer = (ByteBuffer) obj;
            if (buffer.top != this.top)
            {
                return false;
            }
            if (buffer.position != this.position)
            {
                return false;
            }
            for (int i = this.top - 1; i >= 0; i--)
            {
                if (buffer.array[i] != this.array[i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public byte[] getArray()
    {
        return this.array;
    }

    public int getHashCode()
    {
        int num = 0x11;
        for (int i = this.top - 1; i >= 0; i--)
        {
            num = (0x10001 * num) + this.array[i];
        }
        return num;
    }

    public int length()
    {
        return (this.top - this.position);
    }

    public int offset()
    {
        return this.position;
    }

    public byte read(int pos)
    {
        return this.array[pos];
    }

    public void read(byte[] data, int pos, int len)
    {
        Array.Copy(this.array, this.position, data, pos, len);
        this.position += len;
    }

    public bool readBoolean()
    {
        int num;
        this.position = (num = this.position) + 1;
        return (this.array[num] != 0);
    }

    public sbyte readByte()
    {
        int num;
        this.position = (num = this.position) + 1;
        return (sbyte) this.array[num];
    }

    public void readBytes(ByteBuffer data, int pos, int len)
    {
        if (data.array.Length < (len + 1))
        {
            data.setCapacity(len + 0x20);
        }
        Array.Copy(this.array, this.position, data.array, pos, len);
        this.position += len;
        data.top = len;
        data.position = pos;
    }

    public void readBytes(byte[] bytes, int pos, int len)
    {
        Array.Copy(this.array, this.position, bytes, pos, len);
        this.position += len;
    }

    public char readChar()
    {
        return (char) this.readUnsignedShort();
    }

    public byte[] readData()
    {
        int len = this.readLength() - 1;
        if (len < 0)
        {
            return null;
        }
        if (len > 0x64000)
        {
            throw new Exception(this + " readData, data overflow:" + len);
        }
        if (len == 0)
        {
            return EMPTY_ARRAY;
        }
        byte[] data = new byte[len];
        this.read(data, 0, len);
        return data;
    }

    public double readDouble()
    {
        return BitConverter.Int64BitsToDouble(this.readLong());
    }

    public float readFloat()
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(this.readInt()), 0);
    }

    public int readInt()
    {
        int position = this.position;
        this.position += 4;
        return ((((this.array[position + 3] & 0xff) + ((this.array[position + 2] & 0xff) << 8)) + ((this.array[position + 1] & 0xff) << 0x10)) + ((this.array[position] & 0xff) << 0x18));
    }

    public int readLength()
    {
        int num = this.array[this.position] & 0xff;
        if (num >= 0x80)
        {
            this.position++;
            return (num - 0x80);
        }
        if (num >= 0x40)
        {
            return (this.readUnsignedShort() - 0x4000);
        }
        if (num < 0x20)
        {
            throw new Exception(this + " readLength, invalid number:" + num);
        }
        return (this.readInt() - 0x20000000);
    }

    public long readLong()
    {
        int position = this.position;
        this.position += 8;
        return ((((((((this.array[position + 7] & 0xffL) + ((this.array[position + 6] & 0xffL) << 8)) + ((this.array[position + 5] & 0xffL) << 0x10)) + ((this.array[position + 4] & 0xffL) << 0x18)) + ((this.array[position + 3] & 0xffL) << 0x20)) + ((this.array[position + 2] & 0xffL) << 40)) + ((this.array[position + 1] & 0xffL) << 0x30)) + ((this.array[position] & 0xffL) << 0x38));
    }

    public short readShort()
    {
        return (short) this.readUnsignedShort();
    }

    public string readString()
    {
        return this.readString(null);
    }

    public string readString(string charsetName)
    {
        string str;
        int len = this.readLength() - 1;
        if (len < 0)
        {
            return null;
        }
        if (len > 0x64000)
        {
            throw new Exception(this + " readString, data overflow:" + len);
        }
        if (len == 0)
        {
            return string.Empty;
        }
        byte[] data = new byte[len];
        this.read(data, 0, len);
        if (charsetName == null)
        {
            return Encoding.Default.GetString(data);
        }
        try
        {
            str = Encoding.GetEncoding(charsetName).GetString(data);
        }
        catch (Exception exception)
        {
            object[] objArray1 = new object[] { this, " readString, invalid charsetName:", charsetName, " ", exception.ToString() };
            throw new Exception(string.Concat(objArray1));
        }
        return str;
    }

    public int readUnsignedByte()
    {
        int num;
        this.position = (num = this.position) + 1;
        return (this.array[num] & 0xff);
    }

    public int readUnsignedShort()
    {
        int position = this.position;
        this.position += 2;
        return ((this.array[position + 1] & 0xff) + ((this.array[position] & 0xff) << 8));
    }

    public string readUTF()
    {
        int length = this.readLength() - 1;
        if (length < 0)
        {
            return null;
        }
        if (length == 0)
        {
            return string.Empty;
        }
        if (length > 0x64000)
        {
            throw new Exception(this + " readUTF, data overflow:" + length);
        }
        char[] array = new char[length];
        int num2 = ByteKit.readUTF(this.array, this.position, length, array);
        if (num2 < 0)
        {
            throw new Exception(this + " readUTF, format err, len=" + length);
        }
        this.position += length;
        return new string(array, 0, num2);
    }

    public string readUTFBytes(int len)
    {
        if (len < 0)
        {
            return null;
        }
        if (len == 0)
        {
            return string.Empty;
        }
        if (len > 0x64000)
        {
            throw new Exception(this + " readUTF, data overflow:" + len);
        }
        byte[] data = new byte[len];
        this.read(data, 0, len);
        return Encoding.UTF8.GetString(data);
    }

    public void setCapacity(int len)
    {
        int length = this.array.Length;
        if (len > length)
        {
            while (length < len)
            {
                length = (length << 1) + 1;
            }
            byte[] destinationArray = new byte[length];
            Array.Copy(this.array, 0, destinationArray, 0, this.top);
            this.array = destinationArray;
        }
    }

    public void setOffset(int offset)
    {
        if ((offset < 0) || (offset > this.top))
        {
            throw new Exception(this + " setOffset, invalid offset:" + offset);
        }
        this.position = offset;
    }

    public void setTop(int top)
    {
        if (top < this.position)
        {
            throw new Exception(this + " setTop, invalid top:" + top);
        }
        if (top > this.array.Length)
        {
            this.setCapacity(top);
        }
        this.top = top;
    }

    public byte[] toArray()
    {
        byte[] destinationArray = new byte[this.top - this.position];
        Array.Copy(this.array, this.position, destinationArray, 0, destinationArray.Length);
        return destinationArray;
    }

    public override string ToString()
    {
        object[] objArray1 = new object[] { base.ToString(), "[", this.top, ",", this.position, ",", this.array.Length, "]" };
        return string.Concat(objArray1);
    }

    public void write(int b, int pos)
    {
        this.array[pos] = (byte) b;
    }

    public void write(byte[] data, int pos, int len)
    {
        if (len > 0)
        {
            if (this.array.Length < (this.top + len))
            {
                this.setCapacity(this.top + len);
            }
            Array.Copy(data, pos, this.array, this.top, len);
            this.top += len;
        }
    }

    public void writeBoolean(bool b)
    {
        int num;
        if (this.array.Length < (this.top + 1))
        {
            this.setCapacity(this.top + 0x20);
        }
        this.top = (num = this.top) + 1;
        this.array[num] = !b ? ((byte) 0) : ((byte) 1);
    }

    public void writeByte(int b)
    {
        int num;
        if (this.array.Length < (this.top + 1))
        {
            this.setCapacity(this.top + 0x20);
        }
        this.top = (num = this.top) + 1;
        this.array[num] = (byte) b;
    }

    public void writeBytes(byte[] b)
    {
        this.write(b, 0, b.Length);
    }

    public void writeBytes(byte[] b, int offset, int len)
    {
        this.write(b, offset, len);
    }

    public void writeBytes(ByteBuffer b, uint offset, uint len)
    {
        this.write(b.toArray(), (int) offset, (int) len);
    }

    public void writeChar(int c)
    {
        this.writeShort(c);
    }

    public void writeData(byte[] data)
    {
        this.writeData(data, 0, (data == null) ? 0 : data.Length);
    }

    public void writeData(byte[] data, int pos, int len)
    {
        if (data == null)
        {
            this.writeLength(0);
        }
        else
        {
            this.writeLength(len + 1);
            this.write(data, pos, len);
        }
    }

    public void writeDouble(double d)
    {
        this.writeLong(BitConverter.DoubleToInt64Bits(d));
    }

    public void writeFloat(float f)
    {
        this.writeInt(BitConverter.ToInt32(BitConverter.GetBytes(f), 0));
    }

    public void writeInt(int i)
    {
        int top = this.top;
        if (this.array.Length < (top + 4))
        {
            this.setCapacity(top + 0x20);
        }
        byte[] bytes = BitConverter.GetBytes(i);
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, this.array, top, 4);
        this.top += 4;
    }

    public void writeLength(int len)
    {
        if ((len >= 0x20000000) || (len < 0))
        {
            throw new Exception(this + " writeLength, invalid len:" + len);
        }
        if (len < 0x80)
        {
            this.writeByte(len + 0x80);
        }
        else if (len < 0x4000)
        {
            this.writeShort(len + 0x4000);
        }
        else
        {
            this.writeInt(len + 0x20000000);
        }
    }

    public void writeLong(long l)
    {
        int top = this.top;
        if (this.array.Length < (top + 8))
        {
            this.setCapacity(top + 0x20);
        }
        byte[] bytes = BitConverter.GetBytes(l);
        Array.Reverse(bytes);
        Array.Copy(bytes, 0, this.array, top, 8);
        this.top += 8;
    }

    public void writeShort(int s)
    {
        int top = this.top;
        if (this.array.Length < (top + 2))
        {
            this.setCapacity(top + 0x20);
        }
        byte[] bytes = BitConverter.GetBytes(s);
        Array.Reverse(bytes);
        Array.Copy(bytes, 2, this.array, top, 2);
        this.top += 2;
    }

    public void writeString(string str)
    {
        this.writeString(str, null);
    }

    public void writeString(string str, string charsetName)
    {
        if (str == null)
        {
            this.writeLength(0);
        }
        else if (str.Length <= 0)
        {
            this.writeLength(1);
        }
        else
        {
            byte[] bytes;
            if (charsetName != null)
            {
                try
                {
                    bytes = Encoding.GetEncoding(charsetName).GetBytes(str);
                }
                catch (Exception exception)
                {
                    object[] objArray1 = new object[] { this, " writeString, invalid charsetName:", charsetName, " ", exception.ToString() };
                    throw new Exception(string.Concat(objArray1));
                }
            }
            else
            {
                bytes = Encoding.Default.GetBytes(str);
            }
            this.writeLength(bytes.Length + 1);
            this.write(bytes, 0, bytes.Length);
        }
    }

    public void writeUTF(string str)
    {
        this.writeUTF(str, 0, (str == null) ? 0 : str.Length);
    }

    public void writeUTF(string str, int index, int length)
    {
        if (str == null)
        {
            this.writeLength(0);
        }
        else
        {
            int num = ByteKit.getUTFLength(str, index, length);
            this.writeLength(num + 1);
            if (num > 0)
            {
                int top = this.top;
                if (this.array.Length < (top + num))
                {
                    this.setCapacity(top + num);
                }
                ByteKit.writeUTF(str, index, length, this.array, top);
                this.top += num;
            }
        }
    }

    public void writeUTFBytes(string str)
    {
        if ((str != null) && (str.Length > 0))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this.writeBytes(bytes);
        }
    }

    public void zeroOffset()
    {
        int position = this.position;
        if (position > 0)
        {
            int length = this.top - position;
            Array.Copy(this.array, position, this.array, 0, length);
            this.top = length;
            this.position = 0;
        }
    }

    public int position
    {
        get
        {
            return this._position;
        }
        set
        {
            this._position = value;
            this.bytesAvailable = (uint) Math.Abs((int) (this.top - this._position));
        }
    }

    public int top
    {
        get
        {
            return this._top;
        }
        set
        {
            this._top = value;
            this.bytesAvailable = (uint) Math.Abs((int) (this._top - this.position));
        }
    }
}

