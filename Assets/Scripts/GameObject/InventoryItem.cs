using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.GameObject
{
    [GlobalClass]
    public partial class InventoryItem : Resource
    {
        [Export]
        public Vector2I Position { get; set; } = Vector2I.Zero;

        [Export]
        public Color Color { get; set; } = new Color("FFFFFFFF");
    }
}
