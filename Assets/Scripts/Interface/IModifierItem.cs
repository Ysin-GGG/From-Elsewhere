using LOI.Gameplay;
using System.Collections;
using System.Collections.Generic;

namespace LOI.Interface
{
    public interface IModifierItem
    {
        public IList Modifiers { get; }
    }

    public interface IModifierItem<T> : IModifierItem
    {
        protected new List<Modifier<T>> Modifiers { get; }
    }
}
