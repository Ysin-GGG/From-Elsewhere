using LOI.DataStructures;
using LOI.GameObject;
using LOI.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOI.Gameplay
{
    /// <summary> 援助体<para>Helper具备了Character一样的数据框架，且可以访问自己的归属Character</para>可以使用该类制作随从、高级弹幕等<para>通过Kill()以销毁Helper自身</para> </summary>
    public class Helper : Entity
    {
        public override StatisticCollection Stats => throw new NotImplementedException();

        public Helper(EntityCollision2D obj, int id) : base(obj, id)
        {
        }
    }
}
