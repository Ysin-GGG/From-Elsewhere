using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;

namespace LOI.DataStructures
{
	public class ObjPool<T> : IEnumerable<T>
	{
		public event Action<T> OnRequest;
		public event Action<T> OnRelease;
		public int Count => this.actived.Count;
		public int Capacity => this.deactived.Count + this.Count;
		private readonly Lock locked = new Lock();
		private Func<int, T> create;
		private HashSet<T> actived;
		private Queue<T> deactived;

		public ObjPool(Func<int, T> create, int capacity = 0, IEqualityComparer<T> cmp = null)
		{
			if (cmp == ObjPool<T>.NodeComparer)
			{
				this.create = (id) =>
				{
					T obj = create(id);
					if (obj is Node node)
					{
						node.SetMeta("PoolID", id);
						return (T)(object)node;
					}
					else return obj;
				};
			}
			else this.create = create;
			this.actived = cmp == null ? new HashSet<T>() : new HashSet<T>(cmp);
			this.deactived = new Queue<T>();
			while (capacity-- > 0) this.deactived.Enqueue(this.create(capacity));
		}

		public ObjPool(Func<int, T> create, IEnumerable<T> objs, IEqualityComparer<T> cmp = null)
		{
			if (cmp == ObjPool<T>.NodeComparer)
			{
				this.create = (id) =>
				{
					T obj = create(id);
					if (obj is Node node)
					{
						node.SetMeta("PoolID", id);
						return (T)(object)node;
					}
					else return obj;
				};
			}
			else this.create = create;
			this.actived = cmp == null ? new HashSet<T>(objs?.Count() ?? 0) : new HashSet<T>(objs?.Count() ?? 0, cmp);
			this.deactived = new Queue<T>(objs ?? Array.Empty<T>());
		}

		public T Request()
		{
			lock (this.locked)
			{
				if (!this.deactived.TryDequeue(out T obj)) obj = this.create(this.Capacity);
				this.actived.Add(obj);
				this.OnRequest?.Invoke(obj);
				return obj;
			}
		}

		public void Release(T obj)
		{
			lock (this.locked)
				if (obj != null)
					if (this.actived.Remove(obj))
					{
						this.deactived.Enqueue(obj);
						this.OnRelease?.Invoke(obj);
					}
		}

		public void Clear()
		{
			lock (this.locked)
			{
				foreach (T obj in this.actived)
				{
					this.deactived.Enqueue(obj);
					this.OnRelease?.Invoke(obj);
				}
				this.actived.Clear();
			}
		}

		public void Reset()
		{
			lock (this.locked)
			{
				int capacity = this.Capacity;
				foreach (T obj in this.actived)
					this.OnRelease?.Invoke(obj);
				this.actived.Clear();
				this.deactived.Clear();
				this.OnRequest = null;
				this.OnRelease = null;
				while (capacity-- > 0)
					this.deactived.Enqueue(this.create(capacity));
			}
		}

		public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)actived).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)actived).GetEnumerator();

		public static implicit operator T(ObjPool<T> pool) => pool.Request();

		public static implicit operator T[](ObjPool<T> pool) => pool.actived.ToArray();

		public static EqualityComparer<Node> NodeComparer { get; } = EqualityComparer<Node>.Create((e1, e2) => e1.GetMeta("PoolID").AsInt32() == e2.GetMeta("PoolID").AsInt32(), (e) => e.GetMeta("PoolID").AsInt32());
	}
}