using Godot;
using LOI.Gameplay;
using LOI.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Utility
{
    public static class ContentUtils
    {
        public static int GetItem<T>() where T : Item, new() => throw new NotImplementedException();
        public static int GetEnemy<T>() where T : Enemy, new() => throw new NotImplementedException();
        public static int GetState<T>() where T : State, new() => throw new NotImplementedException();
        public static int GetProjectile<T>() where T : Projectile, new() => throw new NotImplementedException();
        public static int GetWeaponArt<T>() where T : WeaponArt, new() => throw new NotImplementedException();
        public static int GetDialogue<T>() where T : Dialogue, new() => throw new NotImplementedException();
    }
}
