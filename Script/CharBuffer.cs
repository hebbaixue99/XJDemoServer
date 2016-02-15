using System;

public class CharBuffer
{
    private int _offset;
    private int _top;
    private char[] array;
    public const int CAPACITY = 0x20;
    public const string NULL = "null";

    public CharBuffer() : this(0x20)
    {
    }

    public CharBuffer(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentException(this + " <init>, invalid capatity:" + capacity);
        }
        this.array = new char[capacity];
        this._top = 0;
        this._offset = 0;
    }

    public CharBuffer(char[] data)
    {
        if (data == null)
        {
            throw new ArgumentException(this + " <init>, null data");
        }
        this.array = data;
        this._top = data.Length;
        this._offset = 0;
    }

    public CharBuffer(string str)
    {
        if (str == null)
        {
            throw new ArgumentException(this + " <init>, null str");
        }
        int length = str.Length;
        this.array = new char[length + 0x20];
        str.CopyTo(0, this.array, 0, length);
        this._top = length;
        this._offset = 0;
    }

    public CharBuffer(char[] data, int index, int length)
    {
        if (data == null)
        {
            throw new ArgumentException(this + " <init>, null data");
        }
        if ((index < 0) || (index > data.Length))
        {
            throw new ArgumentException(this + " <init>, invalid index:" + index);
        }
        if ((length < 0) || (data.Length < (index + length)))
        {
            throw new ArgumentException(this + " <init>, invalid length:" + length);
        }
        this.array = data;
        this._top = index + length;
        this._offset = index;
    }

    public CharBuffer append(char[] data)
    {
        if (data == null)
        {
            return this.append("null");
        }
        return this.append(data, 0, data.Length);
    }

    public CharBuffer append(bool b)
    {
        int index = this._top;
        if (b)
        {
            if (this.array.Length < (index + 4))
            {
                this.setCapacity(index + 0x20);
            }
            this.array[index] = 't';
            this.array[index + 1] = 'r';
            this.array[index + 2] = 'u';
            this.array[index + 3] = 'e';
            this._top += 4;
        }
        else
        {
            if (this.array.Length < (index + 5))
            {
                this.setCapacity(index + 0x20);
            }
            this.array[index] = 'f';
            this.array[index + 1] = 'a';
            this.array[index + 2] = 'l';
            this.array[index + 3] = 's';
            this.array[index + 4] = 'e';
            this._top += 5;
        }
        return this;
    }

    public CharBuffer append(char c)
    {
        if (this.array.Length < (this._top + 1))
        {
            this.setCapacity(this._top + 0x20);
        }
        this.array[this._top++] = c;
        return this;
    }

    public CharBuffer append(double d)
    {
        return this.append(Convert.ToString(d));
    }

    public CharBuffer append(int i)
    {
        int num4;
        if (i == -2147483648)
        {
            this.append("-2147483648");
            return this;
        }
        int num = this._top;
        int num2 = 0;
        int num3 = 0;
        if (i < 0)
        {
            i = -i;
            num3 = 0;
            num4 = i;
            while ((num4 /= 10) > 0)
            {
                num3++;
            }
            num2 = num3 + 2;
            if (this.array.Length < (num + num2))
            {
                this.setCapacity(num + num2);
            }
            this.array[num++] = '-';
        }
        else
        {
            num3 = 0;
            num4 = i;
            while ((num4 /= 10) > 0)
            {
                num3++;
            }
            num2 = num3 + 1;
            if (this.array.Length < (num + num2))
            {
                this.setCapacity(num + num2);
            }
        }
        while (num3 >= 0)
        {
            this.array[num + num3] = (char) (0x30 + (i % 10));
            i /= 10;
            num3--;
        }
        this._top += num2;
        return this;
    }

    public CharBuffer append(long i)
    {
        long num4;
        if (i == -9223372036854775808L)
        {
            this.append("-9223372036854775808");
            return this;
        }
        int num = this._top;
        int num2 = 0;
        int num3 = 0;
        if (i < 0L)
        {
            i = -i;
            num3 = 0;
            num4 = i;
            while ((num4 /= 10L) > 0L)
            {
                num3++;
            }
            num2 = num3 + 2;
            if (this.array.Length < (num + num2))
            {
                this.setCapacity(num + num2);
            }
            this.array[num++] = '-';
        }
        else
        {
            num3 = 0;
            num4 = i;
            while ((num4 /= 10L) > 0L)
            {
                num3++;
            }
            num2 = num3 + 1;
            if (this.array.Length < (num + num2))
            {
                this.setCapacity(num + num2);
            }
        }
        while (num3 >= 0)
        {
            this.array[num + num3] = (char) ((ushort) (0x30L + (i % 10L)));
            i /= 10L;
            num3--;
        }
        this._top += num2;
        return this;
    }

    public CharBuffer append(object obj)
    {
        return this.append((obj == null) ? "null" : obj.ToString());
    }

    public CharBuffer append(float f)
    {
        return this.append(Convert.ToString(f));
    }

    public CharBuffer append(string str)
    {
        if (str == null)
        {
            str = "null";
        }
        int length = str.Length;
        if (length > 0)
        {
            if (this.array.Length < (this._top + length))
            {
                this.setCapacity(this._top + length);
            }
            str.CopyTo(0, this.array, this._top, length);
            this._top += length;
        }
        return this;
    }

    public CharBuffer append(char[] data, int pos, int len)
    {
        if (data == null)
        {
            return this.append("null");
        }
        this.write(data, pos, len);
        return this;
    }

    public int capacity()
    {
        return this.array.Length;
    }

    public void clear()
    {
        this._top = 0;
        this._offset = 0;
    }

    public bool equals(object obj)
    {
        if (this != obj)
        {
            if (!(obj is CharBuffer))
            {
                return false;
            }
            CharBuffer buffer = (CharBuffer) obj;
            if (buffer._top != this._top)
            {
                return false;
            }
            if (buffer._offset != this._offset)
            {
                return false;
            }
            for (int i = this._top - 1; i >= 0; i--)
            {
                if (buffer.array[i] != this.array[i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public char[] getArray()
    {
        return this.array;
    }

    public string getString()
    {
        return new string(this.array, this._offset, this._top - this._offset);
    }

    public int hashCode()
    {
        int num = 0;
        char[] array = this.array;
        int num2 = this._top;
        for (int i = this._offset; i < num2; i++)
        {
            num = (0x1f * num) + array[i];
        }
        return num;
    }

    public int length()
    {
        return (this._top - this._offset);
    }

    public int offset()
    {
        return this._offset;
    }

    public char read()
    {
        return this.array[this._offset++];
    }

    public char read(int pos)
    {
        return this.array[pos];
    }

    public void read(char[] data, int pos, int len)
    {
        Array.Copy(this.array, this._offset, data, pos, len);
        this._offset += len;
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
            char[] destinationArray = new char[length];
            Array.Copy(this.array, 0, destinationArray, 0, this._top);
            this.array = destinationArray;
        }
    }

    public void setOffset(int offset)
    {
        if ((offset < 0) || (offset > this._top))
        {
            throw new ArgumentException(this + " setOffset, invalid offset:" + offset);
        }
        this._offset = offset;
    }

    public void setTop(int top)
    {
        if (top < this._offset)
        {
            throw new ArgumentException(this + " setTop, invalid top:" + top);
        }
        if (top > this.array.Length)
        {
            this.setCapacity(top);
        }
        this._top = top;
    }

    public char[] toArray()
    {
        char[] destinationArray = new char[this._top - this._offset];
        Array.Copy(this.array, this._offset, destinationArray, 0, destinationArray.Length);
        return destinationArray;
    }

    public int top()
    {
        return this._top;
    }

    public override string ToString()
    {
        object[] objArray1 = new object[] { base.ToString(), "[", this._top, ",", this._offset, ",", this.array.Length, "]" };
        return string.Concat(objArray1);
    }

    public void write(char c)
    {
        this.array[this._top++] = c;
    }

    public void write(char c, int pos)
    {
        this.array[pos] = c;
    }

    public void write(char[] data, int pos, int len)
    {
        if (this.array.Length < (this._top + len))
        {
            this.setCapacity(this._top + len);
        }
        Array.Copy(data, pos, this.array, this._top, len);
        this._top += len;
    }
}

