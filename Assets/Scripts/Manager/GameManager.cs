using Godot;
using Godot.Collections;
using LOI.Controllers;
using LOI.DataStructures;
using LOI.GameObject;
using LOI.Gameplay;
using LOI.Reference;
using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LOI.Manager
{
    public sealed partial class GameManager : LOIManager
    {
        public static GameManager Instance { get; private set; }
        public static uint GameTime { get; private set; }
        public static bool GameSecond { get { bool res = GameManager.Instance.secondUpdated ? true : GameManager.GameTime % 60 == 0; return GameManager.Instance.secondUpdated = res; } }
        public static bool GamePause { get; set; }
        /// <summary> 当前场景内所有敌人 </summary>
        public static Enemy[] Enemy => GameManager.Instance.arrEnemy = GameManager.Instance.arrEnemy ?? GameManager.Instance.enemy;
        /// <summary> 当前游戏内所有角色 </summary>
        public static Character[] Character => GameManager.Instance.arrCharacters = GameManager.Instance.arrCharacters ?? GameManager.Instance.character;
        /// <summary> 本地镜头 </summary>
        public static Camera2D LocalCamera => GameManager.Instance.localCamera;
        /// <summary> 本地角色 </summary>
        public static Character LocalCharacter { get; private set; }
        /// <summary> 当前动作模组 </summary>
        public static WeaponArt LocalWeaponArt { get; private set; }
        public static LinkedList<WeaponArt> WeaponArtList { get; } = new LinkedList<WeaponArt>();

        private ObjPool<Enemy> enemy;
        private ObjPool<Character> character;

        [Export]
        internal Node characters;

        [Export]
        internal Node enemies;

        [Export]
        private Camera2D localCamera;

        [Export]
        public Line2D trail;

        private Enemy[] arrEnemy;
        private Character[] arrCharacters;
        private bool secondUpdated;

        public event Action OnOptionsReset;

        protected override void Initialization()
        {
            LegendOfInfinity legendOfInfinity = new LegendOfInfinity();
            GameManager.LocalCharacter = this.character;
            GameManager.LocalCharacter.SetBodyCollision(new Rect2(Vector2.Zero, Vector2.One * 128f), new Rect2(new Vector2(0f, 128f), new Vector2(64f, 128f)), new Rect2(new Vector2(64f, 128f), new Vector2(128f, 128f)));
            GameManager.LocalCharacter.Spectate(this.localCamera);

            GameManager.LocalCharacter.Stats.Attack.DefValue = 1200;
            GameManager.LocalCharacter.Stats.Defense.DefValue = 400;
            GameManager.LocalCharacter.Stats.CritChance.DefValue = .1f;

            /*
            GameManager.LocalCharacter[StatisticRef.Attr_Attack].AddFactor(new DependentFactor("TestAttack1", -100));
            GameManager.LocalCharacter[StatisticRef.Attr_Attack].AddFactor(new IndependentFactor("TestAttack2", -.2));
            GameManager.LocalCharacter[StatisticRef.Critical_Chance].AddFactor(new DependentFactor("TestCritRate1", .1, false));
            GameManager.LocalCharacter[StatisticRef.Critical_Chance].AddFactor(new DependentFactor("TestCritRate2", .1, true));
            GameManager.LocalCharacter[StatisticRef.Critical_Chance].AddFactor(new IndependentFactor("TestCritRate3", .1));
            GameManager.LocalCharacter[StatisticRef.Critical_Damage].AddFactor(new DependentFactor("TestCritDmg1", .3));
            GameManager.LocalCharacter[StatisticRef.Critical_Damage].AddFactor(new IndependentFactor("TestCritDmg2", .3));
            */
            Enemy enemy = this.enemy;
            enemy.SetBodyCollision(new Rect2(Vector2.Zero, Vector2.One * 128f));
            enemy.Position = new Vector2(256f, -128f);
            enemy.Stats.MaxLife.DefValue = 3000000;
            enemy.Stats.Attack.DefValue = 1200;
            enemy.Stats.Defense.DefValue = 300;
            enemy.Stats.SpellResistance.DefValue = .1f;
            enemy.Life = 3000000;

            Options.Initialization();
        }

        protected override void EnterTree()
        {
            string init = StateRef.IdleStandRef;
            GameManager.Instance = this;
            this.enemy = new ObjPool<Enemy>(this.NewEnemy, 32, EqualityComparer<Enemy>.Create((e1, e2) => e1.ID == e2.ID, (e) => e.ID));
            this.character = new ObjPool<Character>(this.NewCharacter, 4, EqualityComparer<Character>.Create((e1, e2) => e1.ID == e2.ID, (e) => e.ID));
        }

        protected override void GameUpdate()
        {
            if (!GameManager.GamePause)
            {
                foreach (Character character in GameManager.Character) character.UpdateSelf();
                foreach (Enemy enemy in GameManager.Enemy) enemy.UpdateSelf();
                GameManager.GameTime++;
                this.secondUpdated = false;
            }
            this.arrEnemy = null;
            this.arrCharacters = null;
        }

        private Character NewCharacter(int id)
        {
            CharacterBody2D obj = new CharacterBody2D() { Name = typeof(Character).Name + id, CollisionLayer = 0, CollisionMask = 0 }.
                SetCollisionLayer(true, (int)LayerRef.Physics2D_Character.Value).
                SetCollisionMask(true, (int)LayerRef.Physics2D_Border.Value, (int)LayerRef.Physics2D_Ground.Value, (int)LayerRef.Physics2D_Wall, (int)LayerRef.Physics2D_Platform)
                as CharacterBody2D;
            obj.AddChild(new Area2D() { Name = "HitCollision", CollisionLayer = 0, CollisionMask = 0 }.
                SetCollisionLayer(true, (int)LayerRef.Physics2D_Hit.Value).
                SetCollisionMask(true, (int)LayerRef.Physics2D_Character.Value, (int)LayerRef.Physics2D_Enemy.Value, (int)LayerRef.Physics2D_Ally, (int)LayerRef.Physics2D_Helper, (int)LayerRef.Physics2D_Hit.Value));
            obj.AddChild(new Area2D() { Name = "ParryCollision", CollisionLayer = 0, CollisionMask = 0 }.
                SetCollisionLayer(true, (int)LayerRef.Physics2D_Parry.Value).
                SetCollisionMask(true, (int)LayerRef.Physics2D_Hit.Value));
            obj.AddChild(new Sprite2D() { Name = "Sprite" });
            this.characters.AddChild(obj);
            Character character = new Character(obj, id);
            return character;
        }

        private Enemy NewEnemy(int id)
        {
            CharacterBody2D obj = new CharacterBody2D() { Name = typeof(Enemy).Name + id, CollisionLayer = 0, CollisionMask = 0 }.
                SetCollisionLayer(true, (int)LayerRef.Physics2D_Enemy.Value).
                SetCollisionMask(true, (int)LayerRef.Physics2D_Border.Value, (int)LayerRef.Physics2D_Ground.Value, (int)LayerRef.Physics2D_Wall, (int)LayerRef.Physics2D_Platform)
                as CharacterBody2D;
            obj.AddChild(new Area2D() { Name = "HitCollision", CollisionLayer = 0, CollisionMask = 0 }.
                SetCollisionLayer(true, (int)LayerRef.Physics2D_Hit.Value).
                SetCollisionMask(true, (int)LayerRef.Physics2D_Character.Value, (int)LayerRef.Physics2D_Enemy.Value, (int)LayerRef.Physics2D_Ally, (int)LayerRef.Physics2D_Helper, (int)LayerRef.Physics2D_Hit.Value));
            obj.AddChild(new Area2D() { Name = "ParryCollision", CollisionLayer = 0, CollisionMask = 0 }.
                SetCollisionLayer(true, (int)LayerRef.Physics2D_Parry.Value).
                SetCollisionMask(true, (int)LayerRef.Physics2D_Hit.Value));
            obj.AddChild(new Sprite2D() { Name = "Sprite" });
            this.enemies.AddChild(obj);
            Enemy enemy = new Enemy(obj, id);
            return enemy;
        }
    }
}
