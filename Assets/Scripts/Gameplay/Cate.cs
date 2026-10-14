using LOI.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Gameplay
{
    public abstract class Cate
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public int Price { get; protected set; } = 0;
        /// <summary> 提供的饱食度 </summary> <remarks> <para>默认为<see langword="0"/></para> </remarks>
        public int Satiety { get; protected set; } = 0;
        /// <summary> 酸味程度 </summary> <remarks> <para>默认为<see langword="0"/></para> </remarks>
        public int Sourness { get; protected set; } = 0;
        /// <summary> 甜味程度 </summary> <remarks> <para>默认为<see langword="0"/></para> </remarks>
        public int Sweetness { get; protected set; } = 0;
        /// <summary> 苦味程度 </summary> <remarks> <para>默认为<see langword="0"/></para> </remarks>
        public int Bitterness { get; protected set; } = 0;
        /// <summary> 辣味程度 </summary> <remarks> <para>默认为<see langword="0"/></para> </remarks>
        public int Spiciness { get; protected set; } = 0;
        /// <summary> 咸味程度 </summary> <remarks> <para>默认为<see langword="0"/></para> </remarks>
        public int Saltiness { get; protected set; } = 0;
        public virtual void OnApply(Character character) { }
    }
}
