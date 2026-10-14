using LOI.DataStructures;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LOI.Gameplay
{
    public class Dungeon
    {
        /// <summary> 掉落物掉落率 </summary>
        public Statistic<double> LootAmount { get; }
        /// <summary> 掉落物稀有度 </summary>
        public Statistic<double> LootRarity { get; }
        public Statistic<double> EnemyAmount { get; }
        public Statistic<double> EnemyStrength { get; }

        public Dungeon(Rand rand) 
        {
            this.LootRarity = new(1d, .5d, 5d, nameof(this.LootRarity));
            this.LootAmount = new(1d, 0d, 12d, nameof(this.LootAmount));
            this.EnemyAmount = new(1d, .3d, 3d, nameof(this.EnemyAmount));
            this.EnemyStrength = new(1d, .25d, 4d, nameof(this.EnemyStrength));
        }
    }
}
