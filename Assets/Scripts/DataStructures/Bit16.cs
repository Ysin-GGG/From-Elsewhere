using Godot;
using LOI.Utility;

namespace LOI.DataStructures
{
    /// <summary> 16位存储器 </summary>
    public struct Bit16
    {
        public const int BitLength = 16;
        private ushort value;
        private bool[] bits;
        private static ushort[] bit = new ushort[Bit16.BitLength]
        {
            0b00000000_00000001,
            0b00000000_00000010,
            0b00000000_00000100,
            0b00000000_00001000,
            0b00000000_00010000,
            0b00000000_00100000,
            0b00000000_01000000,
            0b00000000_10000000,
            0b00000001_00000000,
            0b00000010_00000000,
            0b00000100_00000000,
            0b00001000_00000000,
            0b00010000_00000000,
            0b00100000_00000000,
            0b01000000_00000000,
            0b10000000_00000000,
        };

        /// <summary> 获取指定的一个位的值 </summary>
        /// <param name="index"> 位数（最低位为0，最高位为15） </param>
        public bool this[int index]
        {
            get => (this.value & Bit16.bit[index]) != 0; //  bit[index] == (1 << index)
            set => this.value = (byte)((this.bits[index] = value) ? (this.value | Bit16.bit[index]) : (this.value & (~Bit16.bit[index])));
        }

        /// <summary> 获取多个指定位的逻辑运算结果 </summary>
        /// <param name="any"> true为或运算，false为与运算 </param>
        /// <param name="indexes"> 位数（最低位为0，最高位为15），留空或null则检测所有位 </param>
        public bool this[bool any, params int[] indexes]
        {
            get
            {
                if (indexes == null || indexes.Length == 0) return (any ? this.value : (this.value & ushort.MaxValue)) != 0;
                else
                {
                    foreach (int index in indexes)
                        if (0 <= index && index < Bit16.BitLength)
                            if (this.bits[index] == any) return any;
                    return !any;
                }
            }
        }

        /// <summary> 对多个指定的位赋值 </summary>
        /// <param name="indexes"> 位数（最低位为0，最高位为15） </param>
        public bool this[params int[] indexes]
        {
            set
            {
                if (indexes != null)
                {
                    bool?[] bits = new bool?[Bit16.BitLength];
                    foreach (int index in indexes)
                        if (0 <= index && index < Bit16.BitLength && bits[index] == null)
                            bits[index] = value;
                    this.SetValues(bits);
                }
            }
        }

        public Bit16(ushort value)
        {
            this.value = value;
            this.bits = new bool[Bit16.BitLength];
            int j = this.bits.Length;
            for (ushort i = 32768; i > 0; i >>= 1)
                this.bits[--j] = (value & i) != 0;
        }

        public Bit16(params bool[] bits)
        {
            this.value = 0;
            this.bits = new bool[Bit16.BitLength];
            if (bits != null)
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (this.bits[i] = bits[i]) this.value |= (ushort)(1 << i);
        }

        public Bit16(params int[] bits)
        {
            this.value = 0;
            this.bits = new bool[Bit16.BitLength];
            if (bits != null)
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (this.bits[i] = (bits[i] != 0)) this.value |= (ushort)(1 << i);
        }

        public int Count(bool value) => value.IfElse(ushort.PopCount(this.value), 16 - ushort.PopCount(this.value));

        public bool Count(bool value, int count) => this.Count(value) == count;

        public void Transform(ulong transSeq)
        {
            Bit16 orig = this.value;
            for (int i = 0; i < Bit16.BitLength; i++)
                this[i] = orig[(int)((transSeq >> (i * 4)) & 0b1111)];
        }

        public void SetValues(params bool?[] bits)
        {
            if (bits != null && bits.Length > 0)
            {
                ushort value = 0;
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (bits[i] != null)
                        if (this.bits[i] = bits[i].Value) value |= (ushort)(1 << i);
                        else value <<= 1;
                this.value &= value;
            }
        }

        public void SetValues(params int?[] bits)
        {
            if (bits != null && bits.Length > 0)
            {
                ushort value = 0;
                for (int i = bits.Length.Min(this.bits.Length); i-- > 0;)
                    if (bits[i] != null)
                        if (this.bits[i] = (bits[i].Value != 0)) value |= (ushort)(1 << i);
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

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0)
            => new bool[9] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1)
            => new bool[10] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1, out bool bit1_2)
            => new bool[11] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9], bit1_2 = this[10] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1, out bool bit1_2, out bool bit1_3)
            => new bool[12] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9], bit1_2 = this[10], bit1_3 = this[11] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1, out bool bit1_2, out bool bit1_3, out bool bit1_4)
            => new bool[13] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9], bit1_2 = this[10], bit1_3 = this[11], bit1_4 = this[12] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1, out bool bit1_2, out bool bit1_3, out bool bit1_4, out bool bit1_5)
            => new bool[14] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9], bit1_2 = this[10], bit1_3 = this[11], bit1_4 = this[12], bit1_5 = this[13] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1, out bool bit1_2, out bool bit1_3, out bool bit1_4, out bool bit1_5, out bool bit1_6)
            => new bool[15] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9], bit1_2 = this[10], bit1_3 = this[11], bit1_4 = this[12], bit1_5 = this[13], bit1_6 = this[14] };

        public bool[] GetValues(out bool bit0_0, out bool bit0_1, out bool bit0_2, out bool bit0_3, out bool bit0_4, out bool bit0_5, out bool bit0_6, out bool bit0_7, out bool bit1_0, out bool bit1_1, out bool bit1_2, out bool bit1_3, out bool bit1_4, out bool bit1_5, out bool bit1_6, out bool bit1_7)
            => new bool[16] { bit0_0 = this[0], bit0_1 = this[1], bit0_2 = this[2], bit0_3 = this[3], bit0_4 = this[4], bit0_5 = this[5], bit0_6 = this[6], bit0_7 = this[7], bit1_0 = this[8], bit1_1 = this[9], bit1_2 = this[10], bit1_3 = this[11], bit1_4 = this[12], bit1_5 = this[13], bit1_6 = this[14], bit1_7 = this[15] };

        public bool[] GetValues(int length = 16)
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
                case 9: return new bool[9] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8] };
                case 10: return new bool[10] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9] };
                case 11: return new bool[11] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10] };
                case 12: return new bool[12] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11] };
                case 13: return new bool[13] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11], this[12] };
                case 14: return new bool[14] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11], this[12], this[13] };
                case 15: return new bool[15] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11], this[12], this[13], this[14] };
                case 16: return new bool[16] { this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11], this[12], this[13], this[14], this[15] };
                default: return null;
            }
        }

        public static implicit operator Bit16(int value) => new Bit16((ushort)value);
        public static implicit operator Bit16(Bit8 value) => new Bit16(value.GetValues(Bit8.BitLength));
        public static implicit operator byte(Bit16 value) => (byte)value.value;
        public static implicit operator sbyte(Bit16 value) => (sbyte)value.value;
        public static implicit operator short(Bit16 value) => (short)value.value;
        public static implicit operator ushort(Bit16 value) => (ushort)value.value;
        public static implicit operator int(Bit16 value) => (int)value.value;
        public static implicit operator uint(Bit16 value) => (uint)value.value;
        public static implicit operator long(Bit16 value) => (long)value.value;
        public static implicit operator ulong(Bit16 value) => (ulong)value.value;
        public static implicit operator float(Bit16 value) => (float)value.value;
        public static implicit operator double(Bit16 value) => (double)value.value;

        public override string ToString() => $"({this.value}): [ {string.Join(", ", this.bits)} ]";
        public override int GetHashCode() => this.value;
        public override bool Equals(object obj)
        {
            if (obj is Bit16 value) return this.value == value.value;
            else if (obj is byte value0) return this.value == (ushort)value0;
            else if (obj is sbyte value1) return this.value == (ushort)value1;
            else if (obj is short value2) return this.value == (ushort)value2;
            else if (obj is ushort value3) return this.value == (ushort)value3;
            else if (obj is int value4) return this.value == (ushort)value4;
            else if (obj is uint value5) return this.value == (ushort)value5;
            else if (obj is long value6) return this.value == (ushort)value6;
            else if (obj is ulong value7) return this.value == (ushort)value7;
            else if (obj is float value8) return this.value == (ushort)value8;
            else if (obj is double value9) return this.value == (ushort)value9;
            return false;
        }
    }
}