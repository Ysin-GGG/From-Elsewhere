using LOI.DataStructures;
using LOI.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOI.Gameplay
{
    public class NPC : Entity
    {
        public override StatisticCollection Stats => throw new NotImplementedException();
        public NPC(EntityCollision2D obj, int id) : base(obj, id)
        {

        }
    }
}
