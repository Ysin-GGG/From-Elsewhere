using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace LOI.DataStructures
{
    public class BIT
    {
        public int Count { get; private set; }

        public int Capacity { get; private set; }

        private uint?[] arr;
        private long[] tree;
        private Stack<int> stack;

        public uint? this[int index]
        {
            get => index.InRange(0, this.Capacity, true, false) ? this.arr[index] : null;
            set
            {
                if (index.InRange(0, this.Capacity, true, false) && this.arr[index].HasValue)
                    if (value.HasValue) this.Update(index, value.Value);
                    else this.Remove(index);
                else throw new InvalidOperationException("Cannot set value on null targets, use Add() first.");
            }
        }

        public long this[int startIndex, int endIndex]
        {
            get
            {
                if (0 <= startIndex && startIndex <= endIndex && endIndex < this.Capacity)
                    return this.Query(endIndex + 1) - this.Query(startIndex);
                throw new ArgumentOutOfRangeException();
            }
        }

        public BIT(int capacity = 16)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be a natural number", nameof(capacity));
            this.Capacity = capacity = 1 << 30.Min(Math.Log2(capacity).Ceiling());
            this.arr = new uint?[capacity];
            this.tree = new long[capacity + 1];
            this.stack = new Stack<int>(capacity);
            do
            {
                this.stack.Push(--capacity);
                this.arr[capacity] = null;
            }
            while (capacity > 0);
        }

        public int Add(uint value)
        {
            if (this.stack.Count == 0) this.EnsureCapacity(this.Capacity + 1);
            if (this.stack.TryPop(out int index))
            {
                this.Update(index, value);
                this.Count++;
                return index;
            }
            return -1;
        }

        public bool Remove(int index)
        {
            if (index.InRange(0, this.Capacity, true, false) && this.arr[index].HasValue)
            {
                this.stack.Push(index);
                this.Update(index, null);
                this.Count--;
                return true;
            }
            return false;
        }
        public void Update(int index, uint? value)
        {
            if (index.InRange(0, this.Capacity, true, false))
            {
                long diff = (value.HasValue ? value.Value : 0L) - (this.arr[index].HasValue ? this.arr[index].Value : 0L);
                this.arr[index++] = value;
                while (index <= this.Capacity)
                {
                    this.tree[index] += diff;
                    index += index & -index;
                }
            }
        }

        public long Query(int index)
        {
            if (index.InRange(0, this.Capacity, true, false))
            {
                long value = 0;
                index++;
                while (index > 0)
                {
                    value += this.tree[index];
                    index -= index & -index;
                }
                return value;
            }
            return 0;
        }

        public int SearchIndex(long target)
        {
            if (target.InRange(0, this.tree[this.Capacity], true, true))
            {
                int index = 0;
                int bitMask = 1 << (int)Math.Log2(this.Capacity);
                while (bitMask != 0)
                {
                    int nextIdx = index + bitMask;
                    if (nextIdx <= this.Capacity && this.tree[nextIdx] < target)
                    {
                        target -= this.tree[nextIdx];
                        index = nextIdx;
                    }
                    bitMask >>= 1;
                }
                return index;
            }
            return -1;
        }

        private int EnsureCapacity(int count)
        {
            int capacity = this.Capacity;
            this.Capacity = capacity.Max(30.Min(1 << Math.Log2(count).Ceiling()));
            if (capacity < this.Capacity)
            {
                uint?[] arr = new uint?[this.Capacity];
                long[] tree = new long[this.Capacity + 1];
                this.arr.CopyTo(arr, 0);
                this.tree.CopyTo(tree, 0);
                this.arr = arr;
                this.tree = tree;
                while (capacity < this.Capacity)
                {
                    this.stack.Push(capacity);
                    this.arr[capacity] = null;
                    this.tree[++capacity] = 0;
                }
                this.tree[capacity] = this.tree[capacity >> 1];
            }
            return this.Capacity;
        }
    }
}