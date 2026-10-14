using Godot;
using LOI.Utility;

namespace LOI.DataStructures
{
    /// <summary> 8位存储器 </summary>
    public struct Bit8
    {
        public const int BitLength = 8;
        private byte value;
        private bool[] bits;
        private static byte[] bit = new byte[Bit8.BitLength]
        {
            0b00000001,
            0b00000010,
            0b00000100,
            0b00001000,
            0b00010000,
            0b00100000,
            0b01000000,
            0b10000000,
        };


        /// <summary> 获取指定的一个位的值 </summary>
        /// <param name="index"> 位数（最低位为0，最高位为7） </param>
        public bool this[int index]
        {
            get => (this.value & Bit8.bit[index]) != 0; //  bit[index] == (1 << index)
            set => this.value = (byte)((this.bits[index] = value) ? (this.value | Bit8.bit[index]) : (this.value & (~Bit8.bit[index])));
        }

        /// <summary> 获取多个指定位的逻辑运算结果 </summary>
        /// <param name="any"> true为或运算，false为与运算 </param>
        /// <param name="indexes"> 位数（最低位为0，最高位为7），留空或null则检测所有位 </param>
        public bool this[bool any, params int[] indexes]
        {
            get
            {
                if (indexes == null || indexes.Length == 0) return (any ? this.value : (this.value & byte.MaxValue)) != 0;
                else
                {
                    foreach (int index in indexes)
                        if (0 <= index && index < Bit8.BitLength)
                            if (this.bits[index] == any) return any;
                    return !any;
                }
            }
        }

        /// <summary> 对多个指定的位赋值 </summary>
        /// <param name="indexes"> 位数（最低位为0，最高位为7） </param>
        public bool this[params int[] indexes]
        {
            set
            {
                if (indexes != null)
                {
                    bool?[] bits = new bool?[Bit8.BitLength];
                    foreach (int index in indexes)
                        if (0 <= index && index < Bit8.BitLength && bits[index] == null)
                            bits[index] = value;
                    this.SetValues(bits);
                }
            }
        }

        public Bit8(byte value)
        {
            this.value = value;
            this.bits = new bool[Bit8.BitLength];
            int j = this.bits.Length;
            for (byte i = 128; i > 0; i >>= 1)
                bits[--j] = (value & i) != 0;
        }

        public Bit8(params bool[] bits)
        {
            this.value = 0;
            this.bits = new bool[Bit8.BitLength];
            if (bits != null)
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (this.bits[i] = bits[i]) this.value |= (byte)(1 << i);
        }

        public Bit8(params int[] bits)
        {
            this.value = 0;
            this.bits = new bool[Bit8.BitLength];
            if (bits != null)
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (this.bits[i] = (bits[i] != 0)) this.value |= (byte)(1 << i);
        }

        public int Count(bool value) => value.IfElse(byte.PopCount(this.value), 8 - byte.PopCount(this.value));

        public bool Count(bool value, int count) => this.Count(value) == count;

        public void Transform(int transSeq)
        {
            Bit8 orig = this.value;
            for (int i = 0; i < Bit8.BitLength; i++)
                this[i] = orig[(transSeq >> (i * 3)) & 0b111];
        }

        public void SetValues(params bool?[] bits)
        {
            if (bits != null && bits.Length > 0)
            {
                byte value = 0;
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (bits[i] != null)
                        if (this.bits[i] = bits[i].Value) value |= (byte)(1 << i);
                        else value <<= 1;
                this.value &= value;
            }
        }

        public void SetValues(params int?[] bits)
        {
            if (bits != null && bits.Length > 0)
            {
                byte value = 0;
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (bits[i] != null)
                        if (this.bits[i] = (bits[i].Value != 0)) value |= (byte)(1 << i);
                        else value <<= 1;
                this.value &= value;
            }
        }

        public bool[] GetValues(out bool bit0)
            => new bool[1] { bit0 = this[0] };

        public bool[] GetValues(out bool bit0, out bool bit1)
            => new bool[2] { bit0 = this[0], bit1 = this[1] };

        public bool[] GetValues(out bool bit0, out bool bit1, out bool bit2)
            => new bool[3] { bit0 = this[0], bit1 = this[1], bit2 = this[2] };

        public bool[] GetValues(out bool bit0, out bool bit1, out bool bit2, out bool bit3)
            => new bool[4] { bit0 = this[0], bit1 = this[1], bit2 = this[2], bit3 = this[3] };

        public bool[] GetValues(out bool bit0, out bool bit1, out bool bit2, out bool bit3, out bool bit4)
            => new bool[5] { bit0 = this[0], bit1 = this[1], bit2 = this[2], bit3 = this[3], bit4 = this[4] };

        public bool[] GetValues(out bool bit0, out bool bit1, out bool bit2, out bool bit3, out bool bit4, out bool bit5)
            => new bool[6] { bit0 = this[0], bit1 = this[1], bit2 = this[2], bit3 = this[3], bit4 = this[4], bit5 = this[5] };

        public bool[] GetValues(out bool bit0, out bool bit1, out bool bit2, out bool bit3, out bool bit4, out bool bit5, out bool bit6)
            => new bool[7] { bit0 = this[0], bit1 = this[1], bit2 = this[2], bit3 = this[3], bit4 = this[4], bit5 = this[5], bit6 = this[6] };

        public bool[] GetValues(out bool bit0, out bool bit1, out bool bit2, out bool bit3, out bool bit4, out bool bit5, out bool bit6, out bool bit7)
            => new bool[8] { bit0 = this[0], bit1 = this[1], bit2 = this[2], bit3 = this[3], bit4 = this[4], bit5 = this[5], bit6 = this[6], bit7 = this[7] };

        public bool[] GetValues(int length = 8)
        {
            switch (length)
            {
                case 1: return new bool[1] { this[0] };
                case 2: return new bool[2] { this[0], this[1] };
                case 3: return new bool[3] { this[0], this[1], this[2] };
                case 4: return new bool[4] { this[0], this[1], this[2], this[3] };
                case 5: return new bool[5] { this[0], this[1], this[2], this[3], this[4] };
                case 6: return new bool[6] { this[0], this[1], this[2], this[3], this[4], this[5] };
                case 7: return new bool[7] { this[0], this[1], this[2], this[3], this[4], this[5], this[6] };
                case 8: return new bool[8] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7] };
                default: return null;
            }
        }

        public static implicit operator Bit8(int value) => new Bit8((byte)value);
        public static implicit operator Bit8(Bit16 value) => new Bit8(value.GetValues(Bit8.BitLength));
        public static implicit operator byte(Bit8 value) => (byte)value.value;
        public static implicit operator sbyte(Bit8 value) => (sbyte)value.value;
        public static implicit operator short(Bit8 value) => (short)value.value;
        public static implicit operator ushort(Bit8 value) => (ushort)value.value;
        public static implicit operator int(Bit8 value) => (int)value.value;
        public static implicit operator uint(Bit8 value) => (uint)value.value;
        public static implicit operator long(Bit8 value) => (long)value.value;
        public static implicit operator ulong(Bit8 value) => (ulong)value.value;
        public static implicit operator float(Bit8 value) => (float)value.value;
        public static implicit operator double(Bit8 value) => (double)value.value;

        public override string ToString() => $"({this.value}): [ {string.Join(", ", this.bits)} ]";
        public override int GetHashCode() => this.value;
        public override bool Equals(object obj)
        {
            if (obj is Bit8 value) return this.value == value.value;
            else if (obj is byte value0) return this.value == (byte)value0;
            else if (obj is sbyte value1) return this.value == (byte)value1;
            else if (obj is short value2) return this.value == (byte)value2;
            else if (obj is ushort value3) return this.value == (byte)value3;
            else if (obj is int value4) return this.value == (byte)value4;
            else if (obj is uint value5) return this.value == (byte)value5;
            else if (obj is long value6) return this.value == (byte)value6;
            else if (obj is ulong value7) return this.value == (byte)value7;
            else if (obj is float value8) return this.value == (byte)value8;
            else if (obj is double value9) return this.value == (byte)value9;
            return false;
        }
    }
}