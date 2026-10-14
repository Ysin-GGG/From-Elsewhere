using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LOI.Utility
{
    public static class LazyUtils
    {
        public static bool IsNull(this object obj) => obj == null;
        public static bool NotNull(this object obj) => obj != null;
        public static T IfNull<T>(this object obj, T ifTrue, T ifFalse) => obj == null ? ifTrue : ifFalse; // 尽量避免对值类型使用
        public static T IfNotNull<T>(this object obj, T ifTrue, T ifFalse) => obj != null ? ifTrue : ifFalse; // 尽量避免对值类型使用
        public static bool ToBoolean(this int value) => value != 0;
        public static int ToIntOpp(this bool value) => value ? 1 : -1;
        public static int ToInt(this bool value, int ifTrue = 1, int ifFalse = 0) => value ? ifTrue : ifFalse;
        public static float ToFloat(this bool value, float ifTrue = 1f, float ifFalse = 0f) => value ? ifTrue : ifFalse;
        public static float ToPercent(this float value, float max, int round = 1) => (float)MathF.Round(100.0f * value / max, round);
        public static double ToPercent(this double value, double max, int round = 1) => (double)Math.Round(100.0d * value / max, round);
        public static T IfElse<T>(this bool value, T ifTrue, T ifFalse) => value ? ifTrue : ifFalse;
        public static T2 IfElse<T1, T2>(this T1 value, T1 equalTo, T2 ifTrue, T2 ifFalse) => value.Equals(equalTo) ? ifTrue : ifFalse;
        public static bool TrySet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) { if (dict.TryAdd(key, value)) return true; dict[key] = value; return false; }
        public static bool Any<T>(this T obj, params IEnumerable<T> values) => values.Contains(obj);
        public static bool Any<T>(this T obj, IEqualityComparer<T> comparer, params IEnumerable<T> values) => values.Contains(obj, comparer);

        public static T Enable<T>(this T node, bool ignoreVisible = false, bool ignoreProcess = false, bool ignorePhysicsProcess = false, bool ignoreInputProcess = false) where T : Node
        {
            if (node != null && Node.IsInstanceValid(node))
            {
                if (!ignoreVisible && node is Control control) { control.Visible = true; control.Show(); }
                if (!ignoreProcess) { node.SetProcess(true); node.SetProcessInternal(true); }
                if (!ignorePhysicsProcess) { node.SetPhysicsProcess(true); node.SetProcessInternal(true); }
                if (!ignoreInputProcess) { node.SetProcessInput(true); node.SetProcessUnhandledInput(true); node.SetProcessUnhandledKeyInput(true); }
            }
            return node;
        }

        public static T Disable<T>(this T node, bool ignoreVisible = false, bool ignoreProcess = false, bool ignorePhysicsProcess = false, bool ignoreInputProcess = false) where T : Node
        {
            if (node != null && Node.IsInstanceValid(node))
            {
                if (!ignoreVisible && node is Control control) { control.Visible = false; control.Hide(); }
                if (!ignoreProcess) { node.SetProcess(false); node.SetProcessInternal(false); }
                if (!ignorePhysicsProcess) { node.SetPhysicsProcess(false); node.SetProcessInternal(false); }
                if (!ignoreInputProcess) { node.SetProcessInput(false); node.SetProcessUnhandledInput(false); node.SetProcessUnhandledKeyInput(false); }
            }
            return node;
        }

        public static CollisionObject2D SetCollisionLayer(this CollisionObject2D obj, bool value, params int[] layers)
        {
            uint code = 0;
            foreach (int layer in layers) code |= 1u << layer;
            obj.CollisionLayer = value ? obj.CollisionLayer | code : obj.CollisionLayer & (~code);
            return obj;
        }

        public static CollisionObject2D SetCollisionMask(this CollisionObject2D obj, bool value, params int[] layers)
        {
            uint code = 0;
            foreach (int layer in layers) code |= 1u << layer;
            obj.CollisionMask = value ? obj.CollisionMask | code : obj.CollisionMask & (~code);
            return obj;
        }

        public static void Print(this object obj)
        {
            Debug.Print(obj?.ToString() ?? "null");
        }

        public static void Print(this object obj, string format)
        {
            Debug.Print(string.Format(format, obj?.ToString() ?? "null"));
        }

        public static Vector3I NewVector(this Vector2I xy, int z) => new Vector3I(xy.X, xy.Y, z);
        public static Vector4I NewVector(this Vector2I xy, Vector2I zw) => new Vector4I(xy.X, xy.Y, zw.X, zw.Y);
        public static Vector4I NewVector(this Vector2I xy, int z, int w) => new Vector4I(xy.X, xy.Y, z, w);
        public static Vector4I NewVector(this Vector3I xyz, int w) => new Vector4I(xyz.X, xyz.Y, xyz.Z, w);
        public static Vector3 NewVector(this Vector2 xy, float z) => new Vector3(xy.X, xy.Y, z);
        public static Vector4 NewVector(this Vector2 xy, Vector2 zw) => new Vector4(xy.X, xy.Y, zw.X, zw.Y);
        public static Vector4 NewVector(this Vector2 xy, float z, float w) => new Vector4(xy.X, xy.Y, z, w);
        public static Vector4 NewVector(this Vector3 xyz, float w) => new Vector4(xyz.X, xyz.Y, xyz.Z, w);
    }
}
