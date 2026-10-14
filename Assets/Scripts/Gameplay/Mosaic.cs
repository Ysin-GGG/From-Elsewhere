using Godot;
using LOI.Interface;
using LOI.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LOI.Gameplay
{
    /// <summary> 路引（嵌画） </summary>
    public class Mosaic : Item, IModifierItem<Dungeon>
    {
        public const int MaxPolyominoCount = 16;

        public int UseRemain { get; private set; }
        public bool HasSealed { get; private set; }

        public List<Modifier<Dungeon>> Modifiers { get; }

        IList IModifierItem.Modifiers => this.Modifiers;

        private int sizeX;
        private int sizeY;
        private int[,] mosaic;
        private Queue<int> idle;
        private Polyomino[] polyominoes;
        private static int[] d = { 0, 1, 0, -1, 0 };

        public Mosaic(int sizeX, int sizeY) : base(sizeX, sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.mosaic = new int[sizeX, sizeY];
            this.idle = new Queue<int>(Mosaic.MaxPolyominoCount);
            this.polyominoes = new Polyomino[Mosaic.MaxPolyominoCount];
            this.Modifiers = new List<Modifier<Dungeon>>(Mosaic.MaxPolyominoCount);
        }

        public int TryAddPolyomino(Polyomino polyomino, int x, int y)
        {
            int index = -1;
            if (!polyomino.NotNull() && x.InRange(0, this.sizeX) && y.InRange(0, this.sizeY) && this.CanPlacePolyomino(polyomino, x, y) && this.idle.Count > 0)
            {
                for (int i = 0; i < Mosaic.MaxPolyominoCount && index == -1; i++)
                    if (this.polyominoes[i] == null) index = i;
                for (int i = 0; i < Polyomino.Size; i++)
                    for (int j = 0; j < Polyomino.Size; j++)
                        if (polyomino.Shape[i, j])
                            this.mosaic[x + i, y + j] = this.idle.Count;
            }
            return index;
        }

        public bool CanPlacePolyomino(Polyomino polyomino, int x, int y)
        {
            for (int i = 0; i < Polyomino.Size; i++)
                for (int j = 0; j < Polyomino.Size; j++)
                    if (polyomino.Shape[i, j])
                    {
                        int nx = x + i;
                        int ny = y + j;
                        if (nx.OutRange(0, this.sizeX, false, true) || ny.OutRange(0, this.sizeY, false, true) || this.mosaic[nx, ny] > 0)
                            return false;
                    }
            return !this.HasSealed;
        }

        public Polyomino RemovePolyomino(int x, int y)
        {
            Polyomino polyomino = null;
            if (!this.HasSealed && x.InRange(0, this.sizeX, true, false) && y.InRange(0, this.sizeY, true, false))
            {
                int index = this.mosaic[x, y];
                polyomino = this.polyominoes[index];
                this.polyominoes[index] = null;
                this.idle.Enqueue(index);
                Queue<Vector2I> queue = new Queue<Vector2I>();
                queue.Enqueue(new Vector2I(x, y));
                while (queue.Count > 0)
                {
                    Vector2I curr = queue.Dequeue();
                    int nx = curr.X;
                    int ny = curr.Y;
                    if (nx.InRange(0, this.sizeX) && ny.InRange(0, this.sizeY) && this.mosaic[nx, ny] == index)
                    {
                        this.mosaic[nx, ny] = 0;
                        for (int i = 0; i < 4;)
                        {
                            Vector2I next = curr + new Vector2I(Mosaic.d[i++], Mosaic.d[i]);
                            if (!queue.Contains(next))
                                queue.Enqueue(next);
                        }
                    }
                }
            }
            return polyomino;
        }

        public void Load(List<Modifier<Dungeon>> modifiers, int useRemain)
        {
            // todo
            this.UseRemain = useRemain;
            this.HasSealed = true;
            this.Modifiers.Clear();
            this.Modifiers.AddRange(modifiers);
        }

        public void Seal()
        {
            this.HasSealed = true;
            this.Modifiers.Clear();
            foreach (Polyomino polyomino in polyominoes)
                this.Modifiers.AddRange(polyomino.Modifiers);
        }

        public void Apply(Dungeon dungeon)
        {
            foreach (Modifier<Dungeon> modifier in this.Modifiers)
                modifier.Apply(dungeon);
            if(--this.UseRemain <= 0)
                this.UseRemain = 0;
        }
    }
}