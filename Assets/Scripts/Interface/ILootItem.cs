using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Interface
{
    public interface ILootItem
    {
        public string Name { get; set; }
    }

    public interface ILootItem<T> : ILootItem
    {
        public void GetWeight(T prize, out int weight);
    }
}
