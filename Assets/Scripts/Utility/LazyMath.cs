using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace LOI.Utility
{
    /// <summary> 数学工具库 </summary>
    public static class LazyMath
    {
        /// <summary> 纯小数黄金分割数（双精度） </summary>
        public const double Phi0 = 0.6180339887498948482d;
        /// <summary> 纯小数黄金分割数（双精度） </summary>
        public const double Phi1 = 1.6180339887498948482d;
        /// <summary> 重力加速度（双精度） </summary>
        public const double G = 9.8066500000000006000d;
        /// <summary> 自然对数（双精度） </summary>
        public const double E = 2.7182818284590452354d; // Math.E
        /// <summary> 双精度圆周率（180°） </summary>
        public const double Pi = 3.14159265358979323846d; // Math.PI
        /// <summary> 双精度圆弧度（360°） </summary>
        public const double Tau = LazyMath.Pi * 2d;
        /// <summary> 双精度角度制（1°） </summary>
        public const double Deg01 = LazyMath.Pi / 180d;
        /// <summary> 双精度角度制（15°） </summary>
        public const double Deg15 = LazyMath.Pi / 12d;
        /// <summary> 双精度角度制（30°） </summary>
        public const double Deg30 = LazyMath.Pi / 6d;
        /// <summary> 双精度角度制（45°） </summary>
        public const double Deg45 = LazyMath.Pi / 4d;
        /// <summary> 双精度角度制（60°） </summary>
        public const double Deg60 = LazyMath.Pi / 3d;
        /// <summary> 双精度角度制（90°） </summary>
        public const double Deg90 = LazyMath.Pi / 2d;
        /// <summary> 双精度角度制（105°） </summary>
        public const double Deg105 = LazyMath.Deg90 + LazyMath.Deg15;
        /// <summary> 双精度角度制（120°） </summary>
        public const double Deg120 = LazyMath.Deg90 + LazyMath.Deg30;
        /// <summary> 双精度角度制（135°） </summary>
        public const double Deg135 = LazyMath.Deg90 + LazyMath.Deg45;
        /// <summary> 双精度角度制（150°） </summary>
        public const double Deg150 = LazyMath.Deg90 + LazyMath.Deg60;
        /// <summary> 纯小数黄金分割数（单精度） </summary>
        public const float phi0 = 0.61803399f;
        /// <summary> 带小数黄金分割数（单精度） </summary>
        public const float phi1 = 1.61803399f;
        /// <summary> 重力加速度（单精度） </summary>
        public const float g = 9.80665000f;
        /// <summary> 自然对数（单精度） </summary>
        public const float e = 2.71828175f; // MathF.E
        /// <summary> 单精度圆周率（180°） </summary>
        public const float pi = 3.14159274f; // MathF.PI
        /// <summary> 单精度圆弧度（360°） </summary>
        public const float tau = LazyMath.pi * 2f;
        /// <summary> 单精度角度制（1°） </summary>
        public const float deg01 = LazyMath.pi / 180f;
        /// <summary> 单精度角度制（15°） </summary>
        public const float deg15 = LazyMath.pi / 12f;
        /// <summary> 单精度角度制（30°） </summary>
        public const float deg30 = LazyMath.pi / 6f;
        /// <summary> 单精度角度制（45°） </summary>
        public const float deg45 = LazyMath.pi / 4f;
        /// <summary> 单精度角度制（60°） </summary>
        public const float deg60 = LazyMath.pi / 3f;
        /// <summary> 单精度角度制（90°） </summary>
        public const float deg90 = LazyMath.pi / 2f;
        /// <summary> 单精度角度制（105°） </summary>
        public const float deg105 = LazyMath.deg90 + LazyMath.deg15;
        /// <summary> 单精度角度制（120°） </summary>
        public const float deg120 = LazyMath.deg90 + LazyMath.deg30;
        /// <summary> 单精度角度制（135°） </summary>
        public const float deg135 = LazyMath.deg90 + LazyMath.deg45;
        /// <summary> 单精度角度制（150°） </summary>
        public const float deg150 = LazyMath.deg90 + LazyMath.deg60;

        #region General
        /// <summary> 向上取整 </summary>
        /// <param name="value"> 原值 </param>
        public static int Ceiling(this float value) => (int)MathF.Ceiling(value);
        /// <summary> 向上取整 </summary>
        /// <param name="value"> 原值 </param>
        public static int Ceiling(this double value) => (int)Math.Ceiling(value);
        /// <summary> 向上取整 </summary>
        /// <param name="value"> 原值 </param>
        public static Vector2I Ceiling(this Vector2 value) => new Vector2I(value.X.Ceiling(), value.Y.Ceiling());
        /// <summary> 向上取整 </summary>
        /// <param name="value"> 原值 </param>
        public static Vector3I Ceiling(this Vector3 value) => new Vector3I(value.X.Ceiling(), value.Y.Ceiling(), value.Z.Ceiling());
        /// <summary> 向下取整 </summary>
        /// <param name="value"> 原值 </param>
        public static int Floor(this float value) => (int)MathF.Floor(value);
        /// <summary> 向下取整 </summary>
        /// <param name="value"> 原值 </param>
        public static int Floor(this double value) => (int)Math.Floor(value);
        /// <summary> 向下取整 </summary>
        /// <param name="value"> 原值 </param>
        public static Vector2I Floor(this Vector2 value) => new Vector2I(value.X.Floor(), value.Y.Floor());
        /// <summary> 向下取整 </summary>
        /// <param name="value"> 原值 </param>
        public static Vector3I Floor(this Vector3 value) => new Vector3I(value.X.Floor(), value.Y.Floor(), value.Z.Floor());
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static int Round(this float value) => (int)MathF.Round(value, 0);
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static float Round(this float value, int digits) => (float)MathF.Round((float)value, digits);
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static int Round(this double value) => (int)Math.Round(value, 0);
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static float Round(this double value, int digits) => (float)MathF.Round((float)value, digits);
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static Vector2I Round(this Vector2 value) => new Vector2I(value.X.Round(), value.Y.Round());
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static Vector2 Round(this Vector2 value, int digits) => new Vector2(value.X.Round(digits), value.Y.Round(digits));
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static Vector3I Round(this Vector3 value) => new Vector3I(value.X.Round(), value.Y.Round(), value.Z.Round());
        /// <summary> 四舍六入五成双 </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="digits"> 保留的小数位数 </param>
        public static Vector3 Round(this Vector3 value, int digits) => new Vector3(value.X.Round(digits), value.Y.Round(digits), value.Z.Round(digits));
        /// <summary> 单向取余 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 余数 </param>
        public static int Mod(this int x, int a) => (int)(((x % a) + a) % a);
        /// <summary> 单向取余 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 余数 </param>
        public static float Mod(this float x, double a) => (float)(((x % a) + a) % a);
        /// <summary> 单向取余 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 余数 </param>
        public static double Mod(this double x, double a) => (double)(((x % a) + a) % a);
        /// <summary> 幂函数 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 幂 </param>
        public static float Pow(this float x, float a = LazyMath.e) => MathF.Pow(x, a);
        /// <summary> 幂函数 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 幂 </param>
        public static double Pow(this float x, double a = LazyMath.E) => Math.Pow(x, a);
        /// <summary> 幂函数 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 幂 </param>
        public static double Pow(this double x, double a = LazyMath.E) => Math.Pow(x, a);
        /// <summary> 自然对数函数 </summary>
        /// <param name="x"> 自变量 </param>
        public static float Ln(this float x) => MathF.Log(x);
        /// <summary> 自然对数函数 </summary>
        /// <param name="x"> 自变量 </param>
        public static double Ln(this double x) => Math.Log(x);
        /// <summary> 对数函数（底数为10） </summary>
        /// <param name="x"> 自变量 </param>
        public static float Lg(this float x) => MathF.Log10(x);
        /// <summary> 对数函数（底数为10） </summary>
        /// <param name="x"> 自变量 </param>
        public static double Lg(this double x) => Math.Log10(x);
        /// <summary> 对数函数 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 底数（默认为2） </param>
        public static float Log(this float x, float a = 2) => a == 2 ? MathF.Log2(x) : MathF.Log(x, a);
        /// <summary> 对数函数 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 底数（默认为2） </param>
        public static double Log(this float x, double a = 2) => a == 2 ? Math.Log2(x) : Math.Log(x, a);
        /// <summary> 对数函数 </summary>
        /// <param name="x"> 自变量 </param>
        /// <param name="a"> 底数（默认为2） </param>
        public static double Log(this double x, double a = 2) => a == 2 ? Math.Log2(x) : Math.Log(x, a);
        /// <summary> 平方根函数 </summary>
        /// <param name="x"> 自变量 </param>
        public static float Sqrt(this float x) => MathF.Sqrt(x);
        /// <summary> 平方根函数 </summary>
        /// <param name="x"> 自变量 </param>
        public static double Sqrt(this double x) => Math.Sqrt(x);
        #endregion

        #region trigonometry
        /// <summary> 正弦函数 </summary>
        /// <param name="angle"> 角度 </param>
        public static float Sin(this float angle) => MathF.Sin(angle * LazyMath.deg01);
        /// <summary> 正弦函数 </summary>
        /// <param name="radian"> 弧度 </param>
        public static double Sin(this double radian) => Math.Sin(radian);
        /// <summary> 余弦函数 </summary>
        /// <param name="angle"> 角度 </param>
        public static float Cos(this float angle) => MathF.Cos(angle * LazyMath.deg01);
        /// <summary> 余弦函数 </summary>
        /// <param name="radian"> 弧度 </param>
        public static double Cos(this double radian) => Math.Cos(radian);
        /// <summary> 正切函数 </summary>
        /// <param name="angle"> 角度 </param>
        public static float Tan(this float angle) => MathF.Tan(angle * LazyMath.deg01);
        /// <summary> 正切函数 </summary>
        /// <param name="radian"> 弧度 </param>
        public static double Tan(this double radian) => Math.Tan(radian);
        /// <summary> 向量旋转（角度制） </summary>
        /// <param name="value"> 向量 </param>
        /// <param name="angle"> 角度 </param>
        public static Vector2 RotateBy(this Vector2 value, float angle)
        {
            float sin = LazyMath.Sin(angle);
            float cos = LazyMath.Cos(angle);
            float x = value.X * cos + value.Y * sin;
            float y = value.Y * cos - value.X * sin;
            return new Vector2(x, y);
        }
        /// <summary> 向量旋转（弧度制） </summary>
        /// <param name="value"> 向量 </param>
        /// <param name="radian"> 弧度 </param>
        public static Vector2 RotateBy(this Vector2 value, double radian)
        {
            double sin = LazyMath.Sin(radian);
            double cos = LazyMath.Cos(radian);
            double x = value.X * cos + value.Y * sin;
            double y = value.Y * cos - value.X * sin;
            return new Vector2((float)x, (float)y);
        }
        #endregion

        #region Absolute
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static sbyte Abs(this sbyte value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static short Abs(this short value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static int Abs(this int value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static long Abs(this long value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static decimal Abs(this decimal value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static float Abs(this float value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static double Abs(this double value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static Vector2 Abs(this Vector2 value) => new Vector2(Math.Abs(value.X), Math.Abs(value.Y));
        /// <summary> 绝对值 </summary>
        /// <param name="value"> 原值 </param>
        public static Vector2I Abs(this Vector2I value) => new Vector2I(Math.Abs(value.X), Math.Abs(value.Y));
        #endregion

        #region Extension
        /// <summary> 线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param name="damage"> 值1 </param>
        /// <param name="combo"> 值2 </param>
        public static int Lerp(this float weight, int value1, int value2) => (int)(value1 * (1f - LazyMath.Clamp(weight, 0, 1)) + value2 * LazyMath.Clamp(weight, 0, 1));
        /// <summary> 多点线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param name="values"> 值数组（至少要有两个值） </param>
        public static int Lerp(this float weight, params int[] values) => LazyMath.Lerp(weight % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight, 0, values.Length - 1))], values[LazyMath.Ceiling(LazyMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param name="damage"> 值1 </param>
        /// <param name="combo"> 值2 </param>
        public static float Lerp(this float weight, float value1, float value2) => (float)(value1 * (1f - LazyMath.Clamp(weight, 0, 1)) + value2 * LazyMath.Clamp(weight, 0, 1));
        /// <summary> 多点线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param name="values"> 值数组（至少要有两个值） </param>
        public static float Lerp(this float weight, params float[] values) => LazyMath.Lerp(weight % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight, 0, values.Length - 1))], values[LazyMath.Ceiling(LazyMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param name="damage"> 值1 </param>
        /// <param name="combo"> 值2 </param>
        public static Vector2I Lerp(this float weight, Vector2I value1, Vector2I value2) => new Vector2I((int)LazyMath.Lerp(weight, value1.X, value2.X), (int)LazyMath.Lerp(weight, value1.Y, value2.Y));
        /// <summary> 多点线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param name="values"> 值数组（至少要有两个值） </param>
        public static Vector2I Lerp(this float weight, params Vector2I[] values) => LazyMath.Lerp(weight % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight, 0, values.Length - 1))], values[LazyMath.Ceiling(LazyMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param name="damage"> 值1 </param>
        /// <param name="combo"> 值2 </param>
        public static Vector2 Lerp(this float weight, Vector2 value1, Vector2 value2) => new Vector2((float)LazyMath.Lerp(weight, value1.X, value2.X), (float)LazyMath.Lerp(weight, value1.Y, value2.Y));
        /// <summary> 多点线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param name="values"> 值数组（至少要有两个值） </param>
        public static Vector2 Lerp(this float weight, params Vector2[] values) => LazyMath.Lerp(weight % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight, 0, values.Length - 1))], values[LazyMath.Ceiling(LazyMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param name="damage"> 值1 </param>
        /// <param name="combo"> 值2 </param>
        public static Vector2I Lerp(this Vector2 weight, Vector2I value1, Vector2I value2) => new Vector2I((int)LazyMath.Lerp(weight.X, value1.X, value2.X), (int)LazyMath.Lerp(weight.Y, value1.Y, value2.Y));
        /// <summary> 多点线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param name="values"> 值数组（至少要有两个值） </param>
        public static Vector2I Lerp(this Vector2 weight, params Vector2I[] values) => new Vector2I(LazyMath.Lerp(weight.X % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight.X, 0, values.Length - 1))].X, values[LazyMath.Ceiling(LazyMath.Clamp(weight.X, 1, values.Length))].X), LazyMath.Lerp(weight.Y % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight.Y, 0, values.Length - 1))].Y, values[LazyMath.Ceiling(LazyMath.Clamp(weight.Y, 1, values.Length))].Y));
        /// <summary> 线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param name="damage"> 值1 </param>
        /// <param name="combo"> 值2 </param>
        public static Vector2 Lerp(this Vector2 weight, Vector2 value1, Vector2 value2) => new Vector2((float)LazyMath.Lerp(weight.X, value1.X, value2.X), (float)LazyMath.Lerp(weight.Y, value1.Y, value2.Y));
        /// <summary> 多点线性插值 </summary>
        /// <param name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param name="values"> 值数组（至少要有两个值） </param>
        public static Vector2 Lerp(this Vector2 weight, params Vector2[] values) => new Vector2(LazyMath.Lerp(weight.X % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight.X, 0, values.Length - 1))].X, values[LazyMath.Ceiling(LazyMath.Clamp(weight.X, 1, values.Length))].X), LazyMath.Lerp(weight.Y % 1f, values[LazyMath.Floor(LazyMath.Clamp(weight.Y, 0, values.Length - 1))].Y, values[LazyMath.Ceiling(LazyMath.Clamp(weight.Y, 1, values.Length))].Y));
        
        /// <summary> 取向量长度 </summary>
        /// <param name="value"> 原值 </param>
        public static float Length(this Vector2 value) => LazyMath.Sqrt(value.X * value.X + value.Y * value.Y);
        /// <summary> 取向量长度 </summary>
        /// <param name="value"> 原值 </param>
        public static float Length(this Vector2I value) => LazyMath.Sqrt(value.X * value.X + value.Y * value.Y);
        /// <summary> 取向量长度 </summary>
        /// <param name="value"> 原值 </param>
        public static float Length(this Vector3 value) => LazyMath.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
        /// <summary> 取向量长度 </summary>
        /// <param name="value"> 原值 </param>
        public static float Length(this Vector3I value) => LazyMath.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);

        /// <summary> 值是否在范围内 </summary>
        /// <typeparam name="T">可比较类型</typeparam>
        /// <param name="value"> 实例值 </param>
        /// <param name="min"> 最小值 </param>
        /// <param name="max"> 最大值 </param>
        /// <param name="equalMin"> 范围是否包含最小值 </param>
        /// <param name="equalMax"> 范围是否包含最大值 </param>
        /// <returns>在范围内返回true，否则返回false</returns>
        public static bool InRange<T>(this T value, T min, T max, bool equalMin = true, bool equalMax = false) where T : IComparable
            => equalMin.IfElse(min.CompareTo(value) <= 0, min.CompareTo(value) < 0) && equalMax.IfElse(value.CompareTo(max) <= 0, value.CompareTo(max) < 0);

        /// <summary> 值是否在范围外 </summary>
        /// <typeparam name="T">可比较类型</typeparam>
        /// <param name="value"> 实例值 </param>
        /// <param name="min"> 最小值 </param>
        /// <param name="max"> 最大值 </param>
        /// <param name="equalMin"> 范围是否包含最小值 </param>
        /// <param name="equalMax"> 范围是否包含最大值 </param>
        /// <returns>在范围外返回true，否则返回false</returns>
        public static bool OutRange<T>(this T value, T min, T max, bool equalMin = false, bool equalMax = true) where T : IComparable
            => equalMin.IfElse(min.CompareTo(value) >= 0, min.CompareTo(value) > 0) || equalMax.IfElse(value.CompareTo(max) >= 0, value.CompareTo(max) > 0);
        #endregion

        #region Comparation
        /// <summary> 返回最大值 </summary>
        /// <typeparam name="T">可比较类型</typeparam>
        /// <param name="value"> 原值 </param>
        /// <param name="values"> 数组 </param>
        /// <returns>原值和数组之中的最大值</returns>
        public static T Max<T>(this T value, params T[] values) where T : IComparable<T> { T max = values.Max(); return value.CompareTo(max) > 0 ? value : max; }
        /// <summary> 返回最小值 </summary>
        /// <typeparam name="T">可比较类型</typeparam>
        /// <param name="value"> 原值 </param>
        /// <param name="values"> 数组 </param>
        /// <returns>原值和数组之中的最小值</returns>
        public static T Min<T>(this T value, params T[] values) where T : IComparable<T> { T min = values.Min(); return value.CompareTo(min) < 0 ? value : min; }
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <typeparam name="T">可比较类型</typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> => value.Max(min).Min(max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static byte Clamp(this byte value, byte min, byte max, out int extra) => (byte)LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max, out int extra) => (sbyte)LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static short Clamp(this short value, short min, short max, out int extra) => (short)LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static ushort Clamp(this ushort value, ushort min, ushort max, out int extra) => (ushort)LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static int Clamp(this int value, int min, int max, out int extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static uint Clamp(this uint value, uint min, uint max, out uint extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static long Clamp(this long value, long min, long max, out long extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static ulong Clamp(this ulong value, ulong min, ulong max, out ulong extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static float Clamp(this float value, float min, float max, out float extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static double Clamp(this double value, double min, double max, out double extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) => new Vector2(LazyMath.Clamp(value.X, min.X, max.X), LazyMath.Clamp(value.Y, min.Y, max.Y));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param name="value"> 原值 </param>
        /// <param name="min"> 下限 </param>
        /// <param name="max"> 上限 </param>
        /// <param name="extra"> 差值 </param>
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max, out Vector2 extra) => LazyMath.Clamp((extra = value - (value = LazyMath.Clamp(value, min, max))) + value, min, max);
        #endregion

        #region Summation
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static byte Sum(this byte[] values, byte basic = (byte)0)
        {
            foreach (byte value in values) basic = (byte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static byte Sum(this byte basic, params byte[] values)
        {
            foreach (byte value in values) basic = (byte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static sbyte Sum(this sbyte[] values, sbyte basic = (sbyte)0)
        {
            foreach (sbyte value in values) basic = (sbyte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static sbyte Sum(this sbyte basic, params sbyte[] values)
        {
            foreach (sbyte value in values) basic = (sbyte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static short Sum(this short[] values, short basic = (short)0)
        {
            foreach (short value in values) basic = (short)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static short Sum(this short basic, params short[] values)
        {
            foreach (short value in values) basic = (short)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static ushort Sum(this ushort[] values, ushort basic = (ushort)0)
        {
            foreach (ushort value in values) basic = (ushort)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static ushort Sum(this ushort basic, params ushort[] values)
        {
            foreach (ushort value in values) basic = (ushort)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static int Sum(this int[] values, int basic = (int)0)
        {
            foreach (int value in values) basic = (int)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static int Sum(this int basic, params int[] values)
        {
            foreach (int value in values) basic = (int)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static uint Sum(this uint[] values, uint basic = (uint)0)
        {
            foreach (uint value in values) basic = (uint)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static uint Sum(this uint basic, params uint[] values)
        {
            foreach (uint value in values) basic = (uint)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static long Sum(this long[] values, long basic = (long)0)
        {
            foreach (long value in values) basic = (long)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static long Sum(this long basic, params long[] values)
        {
            foreach (long value in values) basic = (long)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static ulong Sum(this ulong[] values, ulong basic = (ulong)0)
        {
            foreach (ulong value in values) basic = (ulong)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static ulong Sum(this ulong basic, params ulong[] values)
        {
            foreach (ulong value in values) basic = (ulong)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static float Sum(this float[] values, float basic = (float)0)
        {
            foreach (float value in values) basic = (float)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static float Sum(this float basic, params float[] values)
        {
            foreach (float value in values) basic = (float)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static double Sum(this double[] values, double basic = (double)0)
        {
            foreach (double value in values) basic = (double)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static double Sum(this double basic, params double[] values)
        {
            foreach (double value in values) basic = (double)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="values"> 数组 </param>
        /// <param name="basic"> 初始值 </param>
        public static decimal Sum(this decimal[] values, decimal basic = (decimal)0)
        {
            foreach (decimal value in values) basic = (decimal)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param name="basic"> 初始值 </param>
        /// <param name="values"> 数组 </param>
        public static decimal Sum(this decimal basic, params decimal[] values)
        {
            foreach (decimal value in values) basic = (decimal)(basic + value);
            return basic;
        }
        #endregion

        #region Vector Permutations
        public static Vector2 XX(this Vector2 vector) => new Vector2(vector.X, vector.X);
        public static Vector2 XY(this Vector2 vector) => new Vector2(vector.X, vector.Y);
        public static Vector2 YX(this Vector2 vector) => new Vector2(vector.Y, vector.X);
        public static Vector2 YY(this Vector2 vector) => new Vector2(vector.Y, vector.Y);
        public static Vector2 XX(this Vector3 vector) => new Vector2(vector.X, vector.X);
        public static Vector2 XY(this Vector3 vector) => new Vector2(vector.X, vector.Y);
        public static Vector2 XZ(this Vector3 vector) => new Vector2(vector.X, vector.Z);
        public static Vector2 YX(this Vector3 vector) => new Vector2(vector.Y, vector.X);
        public static Vector2 YY(this Vector3 vector) => new Vector2(vector.Y, vector.Y);
        public static Vector2 YZ(this Vector3 vector) => new Vector2(vector.Y, vector.Z);
        public static Vector2 ZX(this Vector3 vector) => new Vector2(vector.Z, vector.X);
        public static Vector2 ZY(this Vector3 vector) => new Vector2(vector.Z, vector.Y);
        public static Vector2 ZZ(this Vector3 vector) => new Vector2(vector.Z, vector.Z);
        public static Vector2 XX(this Vector4 vector) => new Vector2(vector.X, vector.X);
        public static Vector2 XY(this Vector4 vector) => new Vector2(vector.X, vector.Y);
        public static Vector2 XZ(this Vector4 vector) => new Vector2(vector.X, vector.Z);
        public static Vector2 XW(this Vector4 vector) => new Vector2(vector.X, vector.W);
        public static Vector2 YX(this Vector4 vector) => new Vector2(vector.Y, vector.X);
        public static Vector2 YY(this Vector4 vector) => new Vector2(vector.Y, vector.Y);
        public static Vector2 YZ(this Vector4 vector) => new Vector2(vector.Y, vector.Z);
        public static Vector2 YW(this Vector4 vector) => new Vector2(vector.Y, vector.W);
        public static Vector2 ZX(this Vector4 vector) => new Vector2(vector.Z, vector.X);
        public static Vector2 ZY(this Vector4 vector) => new Vector2(vector.Z, vector.Y);
        public static Vector2 ZZ(this Vector4 vector) => new Vector2(vector.Z, vector.Z);
        public static Vector2 ZW(this Vector4 vector) => new Vector2(vector.Z, vector.W);
        public static Vector2 WX(this Vector4 vector) => new Vector2(vector.W, vector.X);
        public static Vector2 WY(this Vector4 vector) => new Vector2(vector.W, vector.Y);
        public static Vector2 WZ(this Vector4 vector) => new Vector2(vector.W, vector.Z);
        public static Vector2 WW(this Vector4 vector) => new Vector2(vector.W, vector.W);
        public static Vector2I XX(this Vector2I vector) => new Vector2I(vector.X, vector.X);
        public static Vector2I XY(this Vector2I vector) => new Vector2I(vector.X, vector.Y);
        public static Vector2I YX(this Vector2I vector) => new Vector2I(vector.Y, vector.X);
        public static Vector2I YY(this Vector2I vector) => new Vector2I(vector.Y, vector.Y);
        public static Vector2I XX(this Vector3I vector) => new Vector2I(vector.X, vector.X);
        public static Vector2I XY(this Vector3I vector) => new Vector2I(vector.X, vector.Y);
        public static Vector2I XZ(this Vector3I vector) => new Vector2I(vector.X, vector.Z);
        public static Vector2I YX(this Vector3I vector) => new Vector2I(vector.Y, vector.X);
        public static Vector2I YY(this Vector3I vector) => new Vector2I(vector.Y, vector.Y);
        public static Vector2I YZ(this Vector3I vector) => new Vector2I(vector.Y, vector.Z);
        public static Vector2I ZX(this Vector3I vector) => new Vector2I(vector.Z, vector.X);
        public static Vector2I ZY(this Vector3I vector) => new Vector2I(vector.Z, vector.Y);
        public static Vector2I ZZ(this Vector3I vector) => new Vector2I(vector.Z, vector.Z);
        public static Vector2I XX(this Vector4I vector) => new Vector2I(vector.X, vector.X);
        public static Vector2I XY(this Vector4I vector) => new Vector2I(vector.X, vector.Y);
        public static Vector2I XZ(this Vector4I vector) => new Vector2I(vector.X, vector.Z);
        public static Vector2I XW(this Vector4I vector) => new Vector2I(vector.X, vector.W);
        public static Vector2I YX(this Vector4I vector) => new Vector2I(vector.Y, vector.X);
        public static Vector2I YY(this Vector4I vector) => new Vector2I(vector.Y, vector.Y);
        public static Vector2I YZ(this Vector4I vector) => new Vector2I(vector.Y, vector.Z);
        public static Vector2I YW(this Vector4I vector) => new Vector2I(vector.Y, vector.W);
        public static Vector2I ZX(this Vector4I vector) => new Vector2I(vector.Z, vector.X);
        public static Vector2I ZY(this Vector4I vector) => new Vector2I(vector.Z, vector.Y);
        public static Vector2I ZZ(this Vector4I vector) => new Vector2I(vector.Z, vector.Z);
        public static Vector2I ZW(this Vector4I vector) => new Vector2I(vector.Z, vector.W);
        public static Vector2I WX(this Vector4I vector) => new Vector2I(vector.W, vector.X);
        public static Vector2I WY(this Vector4I vector) => new Vector2I(vector.W, vector.Y);
        public static Vector2I WZ(this Vector4I vector) => new Vector2I(vector.W, vector.Z);
        public static Vector2I WW(this Vector4I vector) => new Vector2I(vector.W, vector.W);
        #endregion
    }
}
