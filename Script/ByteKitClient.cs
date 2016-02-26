using System;

public class ByteKit
{
	private ByteKit()
	{
	}

	public static ErlType complexAnalyse(ByteBuffer data)
	{
		int position = data.position;
		int num2 = data.readByte();
		data.position = position;
		if ((num2 == ErlArray.TAG[0]) || (num2 == ErlArray.TAG[1]))
		{
			ErlArray array = new ErlArray(null);
			array.bytesRead(data);
			return array;
		}
		switch (num2)
		{
		case 0x6a:
			{
				ErlNullList list = new ErlNullList();
				list.bytesRead(data);
				return list;
			}
		case 0x6c:
			{
				ErlList list2 = new ErlList(null);
				list2.bytesRead(data);
				return list2;
			}
		case 100:
			{
				ErlAtom atom = new ErlAtom(null);
				atom.bytesRead(data);
				return atom;
			}
		case 0x6b:
			{
				ErlString str = new ErlString(null);
				str.sampleBytesRead(data);
				return str;
			}
		case 110:
			{
				ErlLong @long = new ErlLong();
				@long.bytesRead(data);
				return @long;
			}
		case 0x6d:
			{
				ErlByteArray array2 = new ErlByteArray(null);
				array2.bytesRead(data);
				return array2;
			}
		}
		return null;
	}

	public static int getReadLength(byte b)
	{
		int num = b & 0xff;
		if (num >= 0x80)
		{
			return 1;
		}
		if (num >= 0x40)
		{
			return 2;
		}
		if (num < 0x20)
		{
			throw new Exception(typeof(ByteKit).ToString() + " getReadLength, invalid number:" + num);
		}
		return 4;
	}

	public static int getUTFLength(string str, int index, int len)
	{
		int num = 0;
		char[] chArray = str.ToCharArray();
		for (int i = index; i < len; i++)
		{
			int num2 = chArray[i];
			if ((num2 >= 1) && (num2 <= 0x7f))
			{
				num++;
			}
			else if (num2 > 0x7ff)
			{
				num += 3;
			}
			else
			{
				num += 2;
			}
		}
		return num;
	}

	public static int getUTFLength(char[] chars, int index, int len)
	{
		int num = 0;
		for (int i = index; i < len; i++)
		{
			int num2 = chars[i];
			if ((num2 >= 1) && (num2 <= 0x7f))
			{
				num++;
			}
			else if (num2 > 0x7ff)
			{
				num += 3;
			}
			else
			{
				num += 2;
			}
		}
		return num;
	}

	public static int getWriteLength(int len)
	{
		if ((len >= 0x20000000) || (len < 0))
		{
			throw new Exception(typeof(ByteKit).ToString() + " getWriteLength, invalid len:" + len);
		}
		if (len >= 0x4000)
		{
			return 4;
		}
		if (len >= 0x80)
		{
			return 2;
		}
		return 1;
	}

	public static ErlType natureAnalyse(ByteBuffer data)
	{
		uint position = (uint) data.position;
		uint num2 = (uint) data.readUnsignedByte();
		if (num2 != 0x83)
		{
			data.position = (int) position;
			if ((((num2 != ErlArray.TAG[0]) && (num2 != ErlArray.TAG[1])) && ((num2 != 0x6a) && (num2 != 0x6c))) && ((num2 != 100) && (num2 != 0x6d)))
			{
				return natureSampleAnalyse(data);
			}
		}
		return complexAnalyse(data);
	}

	public static ErlType natureSampleAnalyse(ByteBuffer data)
	{
		int position = data.position;
		int num2 = data.readByte();
		data.position = position;
		switch (num2)
		{
		case 0x61:
			{
				ErlByte num3 = new ErlByte(0);
				num3.bytesRead(data);
				return num3;
			}
		case 0x62:
			{
				ErlInt num4 = new ErlInt(0);
				num4.bytesRead(data);
				return num4;
			}
		case 0x6b:
			{
				ErlString str = new ErlString(string.Empty);
				str.sampleBytesRead(data);
				return str;
			}
		case 110:
			{
				ErlLong @long = new ErlLong();
				@long.bytesRead(data);
				return @long;
			}
		}
		return null;
	}

	public static bool readBoolean(byte[] bytes, int pos)
	{
		return (bytes[pos] != 0);
	}

	public static sbyte readByte(byte[] bytes, int pos)
	{
		return (sbyte) bytes[pos];
	}

	public static char readChar(byte[] bytes, int pos)
	{
		return (char) readUnsignedShort(bytes, pos);
	}

	public static double readDouble(byte[] bytes, int pos)
	{
		return Convert.ToDouble(readLong(bytes, pos));
	}

	public static float readFloat(byte[] bytes, int pos)
	{
		return Convert.ToSingle(readInt(bytes, pos));
	}

	public static int readInt(byte[] bytes, int pos)
	{
		return ((((bytes[pos + 3] & 0xff) + ((bytes[pos + 2] & 0xff) << 8)) + ((bytes[pos + 1] & 0xff) << 0x10)) + ((bytes[pos] & 0xff) << 0x18));
	}

	public static string readISO8859_1(byte[] data)
	{
		return readISO8859_1(data, 0, data.Length);
	}

	public static string readISO8859_1(byte[] data, int pos, int len)
	{
		char[] chArray = new char[len];
		int index = (pos + len) - 1;
		for (int i = chArray.Length - 1; index >= pos; i--)
		{
			chArray[i] = (char) data[index];
			index--;
		}
		return new string(chArray);
	}

	public static int readLength(byte[] data, int pos)
	{
		int num = data[pos] & 0xff;
		if (num >= 0x80)
		{
			return (num - 0x80);
		}
		if (num >= 0x40)
		{
			return (((num << 8) + (data[pos + 1] & 0xff)) - 0x4000);
		}
		if (num < 0x20)
		{
			throw new Exception(typeof(ByteKit).ToString() + " readLength, invalid number:" + num);
		}
		return (((((num << 0x18) + ((data[pos + 1] & 0xff) << 0x10)) + ((data[pos + 2] & 0xff) << 8)) + (data[pos + 3] & 0xff)) - 0x20000000);
	}

	public static long readLong(byte[] bytes, int pos)
	{
		return ((((((((bytes[pos + 7] & 0xffL) + ((bytes[pos + 6] & 0xffL) << 8)) + ((bytes[pos + 5] & 0xffL) << 0x10)) + ((bytes[pos + 4] & 0xffL) << 0x18)) + ((bytes[pos + 3] & 0xffL) << 0x20)) + ((bytes[pos + 2] & 0xffL) << 40)) + ((bytes[pos + 1] & 0xffL) << 0x30)) + ((bytes[pos] & 0xffL) << 0x38));
	}

	public static short readShort(byte[] bytes, int pos)
	{
		return (short) readUnsignedShort(bytes, pos);
	}

	public static int readUnsignedByte(byte[] bytes, int pos)
	{
		return (bytes[pos] & 0xff);
	}

	public static int readUnsignedShort(byte[] bytes, int pos)
	{
		return ((bytes[pos + 1] & 0xff) + ((bytes[pos] & 0xff) << 8));
	}

	public static string readUTF(byte[] data)
	{
		char[] array = new char[data.Length];
		int length = readUTF(data, 0, data.Length, array);
		return ((length < 0) ? null : new string(array, 0, length));
	}

	public static string readUTF(byte[] data, int pos, int length)
	{
		char[] array = new char[length];
		int num = readUTF(data, pos, length, array);
		return ((num < 0) ? null : new string(array, 0, num));
	}

	public static int readUTF(byte[] data, int pos, int length, char[] array)
	{
		int num5 = 0;
		int num6 = pos + length;
		while (pos < num6)
		{
			int num2 = data[pos] & 0xff;
			int num = num2 >> 4;
			if (num < 8)
			{
				pos++;
				array[num5++] = (char) num2;
			}
			else
			{
				int num3;
				if ((num == 12) || (num == 13))
				{
					pos += 2;
					if (pos > num6)
					{
						return -1;
					}
					num3 = data[pos - 1];
					if ((num3 & 0xc0) != 0x80)
					{
						return -1;
					}
					array[num5++] = (char) (((num2 & 0x1f) << 6) | (num3 & 0x3f));
				}
				else
				{
					if (num != 14)
					{
						return -1;
					}
					pos += 3;
					if (pos > num6)
					{
						return -1;
					}
					num3 = data[pos - 2];
					int num4 = data[pos - 1];
					if (((num3 & 0xc0) != 0x80) || ((num4 & 0xc0) != 0x80))
					{
						return -1;
					}
					array[num5++] = (char) ((((num2 & 15) << 12) | ((num3 & 0x3f) << 6)) | (num4 & 0x3f));
				}
				continue;
			}
		}
		return num5;
	}

	public static ErlType simpleAnalyse(ByteBuffer data)
	{
		int position = data.position;
		int num2 = data.readByte();
		data.position = position;
		switch (num2)
		{
		case 0x61:
			{
				ErlByte num3 = new ErlByte(0);
				num3.bytesRead(data);
				return num3;
			}
		case 0x6d:
			{
				ErlByteArray array = new ErlByteArray(null);
				array.simpleBytesRead(data);
				return array;
			}
		case 0x62:
			{
				ErlInt num4 = new ErlInt(0);
				num4.bytesRead(data);
				return num4;
			}
		case 0x6b:
			{
				ErlString str = new ErlString(string.Empty);
				str.bytesRead(data);
				return str;
			}
		case 110:
			{
				ErlLong @long = new ErlLong();
				@long.bytesRead(data);
				return @long;
			}
		}
		return null;
	}

	public static void writeBoolean(bool b, byte[] bytes, int pos)
	{
		bytes[pos] = !b ? ((byte) 0) : ((byte) 1);
	}

	public static void writeByte(byte b, byte[] bytes, int pos)
	{
		bytes[pos] = b;
	}

	public static void writeChar(char c, byte[] bytes, int pos)
	{
		writeShort((short) c, bytes, pos);
	}

	public static void writeDouble(double d, byte[] bytes, int pos)
	{
		writeLong((long) Convert.ToInt32(d), bytes, pos);
	}

	public static void writeFloat(float f, byte[] bytes, int pos)
	{
		writeInt(Convert.ToInt32(f), bytes, pos);
	}

	public static void writeInt(int i, byte[] bytes, int pos)
	{
		Array.Copy(BitConverter.GetBytes(i), 0, bytes, pos, 4);
	}

	public static int writeLength(ByteBuffer data, int len)
	{
		if ((len >= 0x20000000) || (len < 0))
		{
			throw new Exception(typeof(ByteKit).ToString() + " writeLength, invalid len:" + len);
		}
		if (len >= 0x4000)
		{
			data.writeInt(len + 0x20000000);
			return 4;
		}
		if (len >= 0x80)
		{
			data.writeShort(len + 0x4000);
			return 2;
		}
		data.writeByte(len + 0x80);
		return 1;
	}

	public static int writeLength(ByteBuffer data, int len, int pos)
	{
		if ((len >= 0x20000000) || (len < 0))
		{
			throw new Exception(typeof(ByteKit).ToString() + " writeLength, invalid len:" + len);
		}
		if (len >= 0x4000)
		{
			data.writeInt(len + 0x20000000);
			return 4;
		}
		if (len >= 0x80)
		{
			data.writeShort((short) (len + 0x4000));
			return 2;
		}
		data.writeByte((byte) (len + 0x80));
		return 1;
	}

	public static int writeLength(int len, byte[] bytes, int pos)
	{
		if ((len >= 0x20000000) || (len < 0))
		{
			throw new Exception(typeof(ByteKit).ToString() + " writeLength, invalid len:" + len);
		}
		if (len >= 0x4000)
		{
			writeInt(len + 0x20000000, bytes, pos);
			return 4;
		}
		if (len >= 0x80)
		{
			writeShort((short) (len + 0x4000), bytes, pos);
			return 2;
		}
		writeByte((byte) (len + 0x80), bytes, pos);
		return 1;
	}

	public static void writeLong(long l, byte[] bytes, int pos)
	{
		Array.Copy(BitConverter.GetBytes(l), 0, bytes, pos, 8);
	}

	public static void writeShort(short s, byte[] bytes, int pos)
	{
		Array.Copy(BitConverter.GetBytes(s), 0, bytes, pos, 2);
	}

	public static byte[] writeUTF(string str)
	{
		return writeUTF(str, 0, str.Length);
	}

	public static byte[] writeUTF(string str, int index, int len)
	{
		byte[] data = new byte[getUTFLength(str, index, len)];
		writeUTF(str, index, len, data, 0);
		return data;
	}

	public static void writeUTF(string str, int index, int len, byte[] data, int pos)
	{
		char[] chArray = str.ToCharArray();
		for (int i = index; i < len; i++)
		{
			int num = chArray[i];
			if ((num >= 1) && (num <= 0x7f))
			{
				data[pos++] = (byte) num;
			}
			else if (num > 0x7ff)
			{
				data[pos++] = (byte) (0xe0 | ((num >> 12) & 15));
				data[pos++] = (byte) (0x80 | ((num >> 6) & 0x3f));
				data[pos++] = (byte) (0x80 | (num & 0x3f));
			}
			else
			{
				data[pos++] = (byte) (0xc0 | ((num >> 6) & 0x1f));
				data[pos++] = (byte) (0x80 | (num & 0x3f));
			}
		}
	}

	public static void writeUTF(char[] chars, int index, int len, byte[] data, int pos)
	{
		for (int i = index; i < len; i++)
		{
			int num = chars[i];
			if ((num >= 1) && (num <= 0x7f))
			{
				data[pos++] = (byte) num;
			}
			else if (num > 0x7ff)
			{
				data[pos++] = (byte) (0xe0 | ((num >> 12) & 15));
				data[pos++] = (byte) (0x80 | ((num >> 6) & 0x3f));
				data[pos++] = (byte) (0x80 | (num & 0x3f));
			}
			else
			{
				data[pos++] = (byte) (0xc0 | ((num >> 6) & 0x1f));
				data[pos++] = (byte) (0x80 | (num & 0x3f));
			}
		}
	}
}

