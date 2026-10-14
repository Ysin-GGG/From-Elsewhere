using Godot;
using System.Collections.Generic;
using LOI.Gameplay;
using System.ComponentModel;
using System.Linq;

namespace LOI.GameObject
{
    //[Tool]
    [Icon("res://icon.svg")]
    [Description("实体全类碰撞箱")]
    [GlobalClass]
    public partial class EntityCollision2D : CharacterBody2D
    {
        
        [Export]
        public Area2D Hit { get; set; }
        [Export]
        public Area2D Reversal { get; set; }

        [Export]
        public CollisionShape2D[] CollisionShapes { get; private set; }

        internal Entity Entity { get; set; }

        public HashSet<CollisionShape2D> BodyShapes { get; private set; }

        public EntityCollision2D(Node parent)
        {
            parent.AddChild(this);
            //if (this.BodyShapes == null) this.BodyShapes = new List<Shape2D>();
            if (this.Hit == null) this.AddChild(this.Hit = new Area2D());
            if (this.Reversal == null) this.AddChild(this.Reversal = new Area2D());
        }

        public override void _Ready()
        {
            this.BodyShapes = this.GetChildren().OfType<CollisionShape2D>().ToHashSet();
            if (this.CollisionShapes != null && this.CollisionShapes.Length > 0)
                foreach (CollisionShape2D cs2d in this.CollisionShapes)
                {
                    this.BodyShapes.Add(cs2d);
                    if(cs2d.GetParent() != this) cs2d.Reparent(this);
                }
                    
        }
        /*
        public override string[] _GetConfigurationWarnings()
        {
            List<string> warnings = new List<string>();
            if(this.Hit == null) warnings.Add("Hit未设置");
            if(this.Reversal == null) warnings.Add("Reversal未设置");
            return warnings.ToArray();
        }
        */
        public override void _PhysicsProcess(double delta)
        {
        }
    }
}
