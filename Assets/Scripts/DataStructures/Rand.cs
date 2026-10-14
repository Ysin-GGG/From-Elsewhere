using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot.Bridge;
using LOI.Utility;

namespace LOI.DataStructures
{
    /// <summary> 伪随机数生成器 </summary>
    public class Rand
    {
        /*
         * 由于double随机数生成不需要类型转换，所以返回值写的是“this.Next(min, max - min)”，而不是有精度丢失可能的“min + this.Next(0.0d, max - min)”
         * 此类下的数据转换均不使用Convert类（因为Convert类对浮点数转整数会进行四舍五入）
         */

        /// <summary> 全局默认伪随机数生成器 </summary>
        public static readonly Rand Instance = new Rand(null, false);
        /// <summary> 种子 </summary>
        public ulong Seed { get; private set; }
        /// <summary> 是否启用正态分布 </summary>
        public bool Gaussian { get; set; }

        private byte head;
        private byte tail;
        private ulong[] rand;

        public void Serialization(SerializationInfo info)
        {
            info.AddValue(nameof(this.Gaussian), this.Gaussian);
            info.AddValue(nameof(this.Seed), this.Seed);
            info.AddValue(nameof(this.head), this.head);
            info.AddValue(nameof(this.tail), this.tail);
            info.AddValue(nameof(this.rand), this.rand);
        }

        public void Deserialization(SerializationInfo info)
        {
            this.Gaussian = info.GetBoolean(nameof(this.Gaussian));
            this.Seed = info.GetUInt64(nameof(this.Seed));
            this.head = info.GetByte(nameof(this.head));
            this.tail = info.GetByte(nameof(this.tail));
            this.rand = info.GetValue(nameof(this.rand), typeof(ulong[])) as ulong[];
        }

        /// <summary> 创建伪随机数生成器 </summary>
        /// <param Name="seed"> 种子（设null则取当前时间为种子） </param>
        public Rand(ulong? seed = null, bool gaussian = false)
        {
            this.Seed = seed ?? (ulong)DateTime.Now.Ticks;
            this.Gaussian = gaussian;
            this.rand = new ulong[256];
            ulong ii = 1, jj = this.Seed;
            this.head = 0;
            this.tail = 127;
            this.rand[255] = jj;
            for (byte i, j = 255; j > 0; j--)
            {
                i = (byte)(127 * j);
                this.rand[i] = ii;
                ii = jj - ii;
                jj = this.rand[i];
            }
            for (byte i = 255; i > 0; i--)
                this.rand[i] -= this.rand[(byte)(i + 127)];
            for (byte i = 255; i > 0; i--)
                this.next(0, 0);
        }

        /// <summary> 泛用随机数（[0,1) * mul + add） </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        private double next(double add = 0.0d, double mul = 1.0d) => this.Gaussian ? this.NextGauss(add + mul / 2.0d, mul / 2.0d) : this.Next(add, mul);

        /// <summary> 服从均匀分布的泛用随机数（[0,1) * mul + add） </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        public double Next(double add = 0.0d, double mul = 1.0d)
        {
            while (++this.head == 0) ;
            while (++this.tail == 0) ;
            return (this.rand[this.head] -= this.rand[this.tail]) / (double)ulong.MaxValue * mul + add;
        }

        /// <summary> 服从正态分布的泛用随机数 </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        public double NextGauss(double add = 0.0d, double mul = 1.0d)
        {
            double r = 1.0d - this.Next(0.0d, 1.0d); // 由于Ln(0)为负无穷大，取值范围应为(0, 1]，
            double theta = this.Next(0.0d, 1.0d) * LazyMath.Tau;
            return LazyMath.Sqrt(-2.0d * r.Ln()) * Math.Sin(LazyMath.Tau * theta).Abs() * this.Next(add, mul) / 8.8480445953779840d; // 用Box-Muller将均匀分布转换为正态分布
        }

        /// <summary> 随机8位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue) => (byte)(min + this.next(0.0d, max - min));

        /// <summary> 随机8位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public sbyte NextSByte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue) => (sbyte)(min + this.next(0.0d, max - min));
        
        /// <summary> 随机16位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public short NextShort(short min = short.MinValue, short max = short.MaxValue) => (short)(min + this.next(0.0d, max - min));

        /// <summary> 随机16位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public ushort NextUShort(ushort min = ushort.MinValue, ushort max = ushort.MaxValue) => (ushort)(min + this.next(0.0d, max - min));

        /// <summary> 随机32位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public int NextInt(int min = int.MinValue, int max = int.MaxValue) => (int)(min + this.next(0.0d, max - min));

        /// <summary> 随机32位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public uint NextUInt(uint min = uint.MinValue, uint max = uint.MaxValue) => (uint)(min + this.next(0.0d, max - min));

        /// <summary> 随机64位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public long NextLong(long min = long.MinValue, long max = long.MaxValue) => (long)(min + this.next(0.0d, max - min));

        /// <summary> 随机64位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public ulong NextULong(ulong min = ulong.MinValue, ulong max = ulong.MaxValue) => (ulong)(min + this.next(0.0d, max - min));

        /// <summary> 随机单精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public float NextFloat(float min = float.MinValue, float max = float.MaxValue) => (float)this.next(min, max - min);

        /// <summary> 随机双精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public double NextDouble(double min = double.MinValue, double max = double.MaxValue) => (double)this.next(min, max - min);

        /// <summary> 随机布尔值 </summary>
        /// <param Name="denominator"> 分母（返回true的概率为：1/denominator * 100%） </param>
        public bool NextBool(int denominator) => this.next(0.0d, 1.0d) < (1.0d / LazyMath.Max(1, denominator));

        /// <summary> 随机布尔值 </summary>
        /// <param Name="probability"> 百分比概率（返回true的可能性为：probability * 100%） </param>
        public bool NextBool(double probability = 0.5d) => this.next(0.0d, 1.0d) < probability;

        /// <summary> 随机取单个字符 </summary>
        /// <param Name="str"> 源字符串 </param>
        public char NextChar(string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ") => str[(int)(this.next(0.0d, str.Length))];

        /// <summary> 随机取角度（单精度） </summary>
        /// <param Name="min"> 最小角度 </param>
        /// <param Name="max"> 最大角度 </param>
        public float NextRadian(float min = 0.0f, float max = 360.0f) => (float)(this.next(min, max - min) * LazyMath.Deg01);

        /// <summary> 随机取角度（双精度） </summary>
        /// <param Name="min"> 最小角度 </param>
        /// <param Name="max"> 最大角度 </param>
        public double NextRadian(double min = 0.0d, double max = 360.0d) => (double)(this.next(min, max - min) * LazyMath.Deg01);

        /// <summary> 随机取对象 </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        public T NextObject<T>(params T[] objs) => objs[(int)this.next(0.0d, objs.Length)];

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="pairs"> 对象-概率对数组 </param>
        public T NextObject<T>(params (T, double)[] pairs)
        {
            int length = pairs.Length;
            double min = 0.0d, max = 0.0d;
            double probability = this.next(0.0d, 1.0d);
            for (int i = 0; i < length; i++)
            {
                min = max;
                max += pairs[i].Item2;
                if (min <= probability && probability < max)
                    return pairs[i].Item1;
            }
            return default(T);
        }

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="probabilities"> 概率数组（若Length少于对象数组，则会循环取概率） </param>
        public T NextObject<T>(T[] objs, double[] probabilities)
        {
            int length = objs.Length;
            (T, double)[] pairs = new (T, double)[length];
            for (int i = 0; i < length; i++)
                pairs[i] = (objs[i], probabilities[i % length]);
            return this.NextObject<T>(pairs);
        }

        /// <summary> 随机取对象（指定占比） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="pairs"> 对象-占比对数组 </param>
        public T NextObject<T>(params (T, int)[] pairs)
        {
            int length = pairs.Length;
            int sum = 0;
            for (int i = 0; i < length; i++)
                sum += pairs[i].Item2;
            int idx = (int)this.next(0.0d, sum);
            int min = 0, max = 0;
            for (int i = 0; i < length; i++)
            {
                min = max;
                max += pairs[i].Item2;
                if (min <= idx && idx < max)
                    return pairs[i].Item1;
            }
            return default(T);
        }

        /// <summary> 随机取对象（指定占比） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="shares"> 占比数组（若Length少于对象数组，则会循环取概率） </param>
        public T NextObject<T>(T[] objs, int[] shares)
        {
            int length = objs.Length;
            (T, int)[] pairs = new (T, int)[length];
            for (int i = 0; i < length; i++)
                pairs[i] = (objs[i], shares[i % length]);
            return this.NextObject<T>(pairs);
        }

        /// <summary> 泛用随机数（[0,1) * mul + add） </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static double Next(double add = 0.0d, double mul = 1.0d, Rand generator = null) => (generator ?? Rand.Instance).next(add, mul);

        /// <summary> 随机8位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextByte(min, max);

        /// <summary> 随机8位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static sbyte NextUbyte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextSByte(min, max);

        /// <summary> 随机16位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static short NextShort(short min = short.MinValue, short max = short.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextShort(min, max);

        /// <summary> 随机16位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static ushort NextUShort(ushort min = ushort.MinValue, ushort max = ushort.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextUShort(min, max);
        
        /// <summary> 随机32位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static int NextInt(int min = int.MinValue, int max = int.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextInt(min, max);

        /// <summary> 随机32位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static uint NextUInt(uint min = uint.MinValue, uint max = uint.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextUInt(min, max);
        
        /// <summary> 随机64位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static long NextLong(long min = long.MinValue, long max = long.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextLong(min, max);

        /// <summary> 随机64位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static ulong NextULong(ulong min = ulong.MinValue, ulong max = ulong.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextULong(min, max);

        /// <summary> 随机单精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static float NextFloat(float min = float.MinValue, float max = float.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextFloat(min, max);

        /// <summary> 随机双精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static double NextDouble(double min = double.MinValue, double max = double.MaxValue, Rand generator = null) => (generator ?? Rand.Instance).NextDouble(min, max);

        /// <summary> 随机布尔值 </summary>
        /// <param Name="denominator"> 分母（返回true的概率为：1/denominator * 100%） </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static bool NextBool(int denominator, Rand generator = null) => (generator ?? Rand.Instance).NextBool(denominator);

        /// <summary> 随机布尔值 </summary>
        /// <param Name="probability"> 百分比概率（返回true的可能性为：probability * 100%） </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static bool NextBool(double probability = 0.5d, Rand generator = null) => (generator ?? Rand.Instance).NextBool(probability);

        /// <summary> 随机取单个字符 </summary>
        /// <param Name="str"> 源字符串 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static char NextChar(string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", Rand generator = null) => (generator ?? Rand.Instance).NextChar(str);

        /// <summary> 随机取对象 </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static T NextObject<T>(Rand generator = null, params T[] objs) => (generator ?? Rand.Instance).NextObject<T>(objs);

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="pairs"> 对象-概率对数组 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static T NextObject<T>(Rand generator = null, params (T, double)[] pairs) => (generator ?? Rand.Instance).NextObject<T>(pairs);

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="probabilities"> 概率数组（若Length少于对象数组，则会循环取概率） </param>
        public static T NextObject<T>(T[] objs, double[] probabilities, Rand generator = null) => (generator ?? Rand.Instance).NextObject<T>(objs, probabilities);

        public static int NextIndex(Rand generator = null, params uint[] weights)
        {
            if (weights.Length > 0)
            {
                ulong[] prefix = new ulong[weights.Length];
                for (int i = 1; i < weights.Length; i++)
                    prefix[i] = prefix[i - 1] + weights[i - 1];
                ulong value = (generator ?? Rand.Instance).NextULong(0, prefix[weights.Length - 1] + weights[weights.Length - 1]);
                int l, r, m;
                for (l = 0, r = weights.Length - 1, m = (l + r) >> 1; l < r; m = (l + r) >> 1)
                    if (value < prefix[m]) r = m;
                    else l = m + 1;
                return l;
            }
            return -1;
        }
    }
}
