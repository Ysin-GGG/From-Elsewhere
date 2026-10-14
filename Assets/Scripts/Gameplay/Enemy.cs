using Godot;
using LOI.DataStructures;
using LOI.Manager;
using LOI.Reference;
using LOI.Utility;
using System.Diagnostics;

namespace LOI.Gameplay
{
    public class Enemy(CharacterBody2D obj, int id) : Entity(obj, id)
    {
        public override EnemyStatisticCollection Stats { get; } = new EnemyStatisticCollection();
        public override bool IsHostile => true;

        protected override void Update()
        {
            if (GameManager.GameTime % 60 == 0)
            {
                double life1 = this.Life;
                double life2 = this.Stats.MaxLife;
            }
        }
    }
}
