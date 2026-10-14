using System.Collections.Generic;
using Godot;
using LOI.Utility;

namespace LOI.Gameplay
{
    public class Item(int width, int height)
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BuyPrice { get; set; }
        public int SellPrice { get; set; }
        public int Width { get; } = width;
        public int Height { get; } = height;
        public List<Tag> Tags { get; }
        public int Stack { get; set;  }
        public int MaxStack { get; set; }

        public Vector2I Size => new Vector2I(this.Width, this.Height);

        public Color RarityColor { get; set; }

        public bool SetDefaults(int type)
        {
            if (type == 0)
            {

            }
            else if (type == 1) { }
            switch (type)
            {
                case -1: // equipments
                    return true;
            }
            return false;
        }

        public virtual bool SetDefaults() => false;
    }
}
