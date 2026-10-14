using Godot;
using LOI.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LOI.Gameplay
{
    public partial class Inventory : Resource, IEnumerable<Item>
    {
        public int Width { get; }
        public int Height { get; }
        public Vector2I Size { get; }
        public int Count => this.originPosition.Count;

        public event Action Updated;

        private Dictionary<Item, Vector2I> originPosition;
        private Item[,] items;

        public Item this[int x, int y] => x.InRange(0, this.Width) && y.InRange(0, this.Height) ? this.items[x, y] : null;

        // OriginPosition is the top-left corner of the item in the inventory grid.
        // RelativePosition is the position of the item relative to its origin position.
        // 

        public Inventory(int width, int height) 
        {
            this.Width = width;
            this.Height = height;
            this.Size = new Vector2I(width, height);
            this.originPosition = new Dictionary<Item, Vector2I>();
            this.items = new Item[width, height];
            for (int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                    this.items[i, j] = null;
        }

        public void Clear()
        {
            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                    this.items[i, j] = null;
            this.originPosition.Clear();
            this.Updated?.Invoke();
        }

        public Inventory SetItem(Item item, int x, int y)
        {
            if (this.CanSetItem(item, x, y) && !this.originPosition.ContainsKey(item))
            {
                for (int i = 0; i < item.Size.X; i++)
                    for (int j = 0; j < item.Size.Y; j++)
                        this.items[x + i, y + j] = item;
                this.originPosition.Add(item, new Vector2I(x, y));
                this.Updated?.Invoke();
            }
            return this;
        }

        public Item GetItem(int x, int y) => this.IsValidPosition(x, y) ? this.items[x, y] : null;
        public Item GetItem(Vector2I position) => this.GetItem(position.X, position.Y);

        public bool HasItem(int x, int y) => this.IsValidPosition(x, y) && this.items[x, y] != null;
        public bool HasItem(Vector2I position) => this.HasItem(position.X, position.Y);

        public bool CanSetItem(Item item, int x, int y)
        {
            if (item.NotNull() && this.IsValidSize(item.Size) && this.IsValidPosition(x, y))
            {
                if (this.IsValidPosition(x + item.Size.X - 1, y + item.Size.Y - 1))
                {
                    for (int i = 0; i < item.Size.X; i++)
                        for (int j = 0; j < item.Size.Y; j++)
                            if (this.items[x + i, y + j].NotNull() && this.items[x + i, y + j] != item)
                                return false;
                    return true;
                }
            }
            return false;
        }

        public bool CanSetItem(Rect2I rect, Item item = null, bool allowOverlap = false)
        {
            if (this.IsValidRect(rect))
            {
                bool flag = allowOverlap && item.NotNull();
                for (int i = 0; i < rect.Size.X; i++)
                    for (int j = 0; j < rect.Size.Y; j++)
                        if (this.items[rect.Position.X + i, rect.Position.Y + j] == item && flag)
                            return false;
                        else if (this.items[rect.Position.X + i, rect.Position.Y + j].IsNull())
                            return false;
                return true;
            }
            return false;
        }

        public bool MoveItem(Vector2I oldPosition, Vector2I newPosition) => this.MoveItem(oldPosition.X, oldPosition.Y, newPosition.X, newPosition.Y);

        public bool MoveItem(int x, int y, int newX, int newY)
        {
            Item item = this.GetItem(x, y);
            if (item.NotNull() && this.CanSetItem(item, newX, newY))
            {
                Vector2I pos = this.GetItemPosition(x, y);
                for (int i = 0; i < item.Size.X; i++)
                    for (int j = 0; j < item.Size.Y; j++)
                        this.items[pos.X + i, pos.Y + j] = null;

                for (int i = 0; i < item.Size.X; i++)
                    for (int j = 0; j < item.Size.Y; j++)
                        this.items[newX + i, newY + j] = item;

                this.originPosition[item] = new Vector2I(newX, newY);
                this.Updated?.Invoke();
                return true;
            }
            return false;
        }

        public bool SwapItem(Vector2I pos1, Vector2I pos2) => this.SwapItem(pos1.X, pos1.Y, pos2.X, pos2.Y);

        public bool SwapItem(int x1, int y1, int x2, int y2)
        {
            Item item1 = this.GetItem(x1, y1);
            Item item2 = this.GetItem(x2, y2);
            if (item1.NotNull() && item2.NotNull())
            {
                Vector2I pos1 = this.GetItemPosition(x1, y1);
                Vector2I pos2 = this.GetItemPosition(x2, y2);

                if (this.IsValidRect(new Rect2I(pos2, item1.Size)))
                    for (int i = 0; i < item1.Size.X; i++)
                        for (int j = 0; j < item1.Size.Y; j++)
                            if (this.items[pos2.X + i, pos2.Y + j] != item2 && this.items[pos2.X + i, pos2.Y + j].NotNull())
                                return false;

                if (this.IsValidRect(new Rect2I(pos1, item2.Size)))
                    for (int i = 0; i < item2.Size.X; i++)
                        for (int j = 0; j < item2.Size.Y; j++)
                            if (this.items[pos1.X + i, pos1.Y + j] != item1 && this.items[pos1.X + i, pos1.Y + j].NotNull())
                                return false;

                for (int i = 0; i < item1.Size.X; i++)
                    for (int j = 0; j < item1.Size.Y; j++)
                        this.items[pos1.X + i, pos1.Y + j] = null;

                for (int i = 0; i < item2.Size.X; i++)
                    for (int j = 0; j < item2.Size.Y; j++)
                        this.items[pos2.X + i, pos2.Y + j] = null;

                for (int i = 0; i < item1.Size.X; i++)
                    for (int j = 0; j < item1.Size.Y; j++)
                        this.items[pos2.X + i, pos2.Y + j] = item1;

                for (int i = 0; i < item2.Size.X; i++)
                    for (int j = 0; j < item2.Size.Y; j++)
                        this.items[pos1.X + i, pos1.Y + j] = item2;

                this.originPosition[item1] = pos2;
                this.originPosition[item2] = pos1;
                this.Updated?.Invoke();
                return true;
            }
            return false;
        }

        public bool SwapItem(Rect2I cellRect1, Rect2I cellRect2)
        {
            // todo: 
            throw new NotImplementedException();
        }

        public Vector2I GetItemPosition(Vector2I pos) => this.GetItemPosition(pos.X, pos.Y);

        public Vector2I GetItemPosition(int x, int y)
        {
            Vector2I pos = new Vector2I(x, y);
            int next;
            if (this.IsValidPosition(x, y) && this.items[x, y].NotNull())
            {
                next = pos.X - 1;
                while (next.InRange(0, this.Width) && this.items[next, pos.Y] == this.items[x, y]) { pos.X = next; next = pos.X - 1; }
                next = pos.Y - 1;
                while (next.InRange(0, this.Height) && this.items[pos.X, next] == this.items[x, y]) { pos.Y = next; next = pos.Y - 1; }
            }
            return pos;
        }

        public Vector2I GetItemPosition(Item item)
        {
            if (this.originPosition.TryGetValue(item, out Vector2I position))
                return position;
            return -Vector2I.One;
        }

        public void RemoveItem(int x, int y)
        {
            Item item = this.GetItem(x, y);
            if (item.NotNull())
            {
                Vector2I pos = this.GetItemPosition(x, y);
                for (int i = 0; i < item.Size.X; i++)
                    for (int j = 0; j < item.Size.Y; j++)
                        this.items[pos.X + i, pos.Y + j] = null;
                this.originPosition.Remove(item);
                this.Updated?.Invoke();
            }
        }

        public bool IsVaildSize(int width, int height) => width.InRange(1, this.Width, true, true) && height.InRange(1, this.Height, true, true);
        public bool IsValidSize(Vector2I size) => this.IsVaildSize(size.X, size.Y);
        public bool IsValidPosition(int x, int y) => x.InRange(0, this.Width) && y.InRange(0, this.Height);
        public bool IsValidPosition(Vector2I pos) => this.IsValidPosition(pos.X, pos.Y);
        public bool IsValidRect(Rect2I rect)
        {
            if(!this.IsValidPosition(rect.Position) ) return false;
            if(!this.IsValidSize(rect.Size) ) return false;
            if(rect.Position.X + rect.Size.X > this.Width) return false;
            if(rect.Position.Y + rect.Size.Y > this.Height) return false;
            return true;
        }

        IEnumerator<Item> IEnumerable<Item>.GetEnumerator() => this.originPosition.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.originPosition.GetEnumerator();

        private class ItemInfo
        {
            public Item Item { get; }
            public Vector2I Position { get; }
            public Vector2I Offset { get; }
            public ItemInfo(Item item, Vector2I position, Vector2I offset)
            {
                this.Item = item;
                this.Position = position;
                this.Offset = offset;
            }
        }
    }
}