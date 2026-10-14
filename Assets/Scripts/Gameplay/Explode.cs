using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Gameplay
{
    public partial class SimpleExplode : Sprite2D
    {
        public Vector2 Size { get => this.Texture.GetSize(); set => this.Scale = value;  }
        public override void _Ready()
        {

        }
    }
}
