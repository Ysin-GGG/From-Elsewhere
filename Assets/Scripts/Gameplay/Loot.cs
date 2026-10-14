using LOI.DataStructures;
using LOI.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;

namespace LOI.Gameplay
{
    /// <summary> 战利品 </summary>
    public class Loot
    {
        public Item Item { get; set; }

        public int Count { get; set; }

        public Vector2 Position { get; set; }

        public Loot(Item item, int count, Vector2 position)
        {
            this.Item = item;
            this.Count = count;
            this.Position = position;
        }

        /// <summary> 战利品列表 </summary>
        /// <remarks> 包含若干个战利品单选表，每个战利品单选表独立计算掉落 </remarks>
        public class Root : List<Loot.Root.Node>
        {
            public string Name { get; private set; }
            public Rand Rand { get; }

            internal static Rand GlobalLootRand { get; } = new Rand(0);

            public Root(string name, Rand rand = null)
            {
                this.Name = name;
                this.Rand = rand ?? new Rand(Loot.Root.GlobalLootRand.NextULong());
            }

            public Loot[] GenLoot(int rarity, int times = 1)
            {
                List<Loot> loots = new List<Loot>();
                Loot loot = null;
                foreach (Loot.Root.Node node in this)
                    if (rarity.InRange(node.MinRarity, node.MaxRarity, true, true))
                        if (this.Rand.NextBool(node.LootChance))
                            if ((loot = node.GenLoot(this, rarity)).NotNull())
                                loots.Add(loot);
                return loots.Count > 0 ? loots.ToArray() : null;
            }

            public Loot[] GenLootMult(int rarity, int n = 1)
            {
                if (n == 1) return this.GenLoot(rarity);
                List<(Loot.Root.Node, int)> nodes = new List<(Loot.Root.Node, int)>();
                List<Loot> loots = new List<Loot>();
                foreach (Loot.Root.Node node in this)
                    if (rarity.InRange(node.MinRarity, node.MaxRarity, true, true))
                    {
                        nodes.Add((node, this.Sample(n, node.LootChance)));
                        throw new NotImplementedException();
                        // todo: implement node&leaf GenLootMult
                    }
                return loots.Count > 0 ? loots.ToArray() : null;
            }

            private int Sample(int n, double p)
            {
                if (n <= 0 || p <= 0.0) return 0;
                if (p >= 1.0) return n;
                int success = 0;
                int remaining = n;
                while (true)
                {
                    int failures = (this.Rand.NextDouble().Ln() / (1.0 - p).Ln()).Floor();
                    if (failures >= remaining) break;
                    success++;
                    remaining -= (failures + 1);
                    if (remaining == 0) break;
                }
                return success;
            }

            /// <summary> 战利品节点 </summary>
            /// <remarks> 
            /// 符合下列两个条件时会掉落物品，掉落的物品仅能为叶节点中的一个
            /// <para>1、随机给出的掉落概率在阈值LootChance内</para>
            /// <para>2、掉落稀有度Rarity在该项掉落稀有度范围内（MinRarity ≤ Rarity ≤ MaxRarity）</para>
            /// </remarks>
            public class Node : IEnumerable<Loot.Root.Node.Leaf>
            {
                /// <summary> 最低掉落稀有度需求 </summary>
                public int MinRarity { get; set; }
                /// <summary> 最高掉落稀有度需求 </summary>
                public int MaxRarity { get; set; }
                /// <summary> 掉落概率 </summary>
                public double LootChance { get; set; }

                private BIT Data { get; }
                private Dictionary<int, Leaf> loots;

                public Node(int minRarity, int maxRarity, double chance = 1d)
                {
                    this.MinRarity = minRarity;
                    this.MaxRarity = maxRarity;
                    this.LootChance = chance;
                    this.Data = new BIT(16);
                    this.loots = new Dictionary<int, Leaf>();
                }

                public Loot GenLoot(Root list, int rarity)
                {
                    long weight = list.Rand.NextLong(0L, this.Data.Query(this.Data.Capacity - 1));
                    int index = this.Data.SearchIndex(weight);
                    return this.loots[index].GenLoot();
                }

                public Loot GenLootMult(Root list, int rarity, int n)
                {
                    throw new NotImplementedException();
                }

                public void Remove(Loot.Root.Node.Leaf leaf) { if (leaf.Owner != null) leaf.Owner = null; }

                public void Add(Loot.Root.Node.Leaf leaf) { if (leaf.Owner != this) leaf.Owner = this; }

                public void AddRange(IEnumerable<Loot.Root.Node.Leaf> options)
                {
                    foreach (Loot.Root.Node.Leaf option in options)
                        this.Add(option);
                }

                public IEnumerator<Leaf> GetEnumerator()
                    => this.loots.Values.GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

                /// <summary> 战利品叶节点 </summary>
                /// <remarks> 记录掉落物和掉落权重，并负责更新自身在亲节点的数据 </remarks>
                public class Leaf(Loot loot, uint weight)
                {
                    public Node Owner { get => field; set => field = this.SetNode(value); }
                    /// <summary> 掉落物 </summary>
                    public Loot Loot { get; } = loot;
                    /// <summary> 原始掉落权重 </summary>
                    public uint Weight { get => field; set => field = this.SetWeight(value); } = weight;

                    private int index;

                    public Loot GenLoot(int count = 1)
                    {
                        this.Loot.Count = count;
                        return this.Loot;
                    }

                    private bool Reset()
                    {
                        if (this.Owner.NotNull())
                        {
                            this.Owner.loots.Remove(this.index);
                            this.Owner.Data.Remove(this.index);
                            //this.Owner = null;
                            this.index = -1;
                            return true;
                        }
                        else return false;
                    }

                    private int BindTo(Loot.Root.Node node)
                    {
                        if (this.Owner != node)
                        {
                            int index = node.Data.Add(this.Weight);
                            if (index >= 0)
                            {
                                node.loots.Add(index, this);
                                if (this.Owner.NotNull())
                                {
                                    this.Owner.loots.Remove(this.index);
                                    this.Owner.Data.Remove(this.index);
                                }
                                this.index = index;
                            }
                            return index;
                        }
                        return -1;
                    }

                    private Loot.Root.Node SetNode(Loot.Root.Node node)
                    {
                        if (node.IsNull()) this.Reset();
                        else this.BindTo(node);
                        return node;
                    }

                    private uint SetWeight(uint weight)
                    {
                        if (this.Owner.NotNull())
                            this.Owner.Data.Update(this.index, weight);
                        return weight;
                    }
                }
            }
        }
    }
}