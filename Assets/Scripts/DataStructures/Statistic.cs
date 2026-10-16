using Godot;
using LOI.Gameplay;
using LOI.Manager;
using LOI.Reference;
using LOI.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace LOI.DataStructures
{
    /*
    public class Statistic : IEqualityComparer<Statistic>, IEnumerable<Factor>
    {
        /// <summary> 显示名 </summary>
        public string Name { get; } = string.Empty;
        /// <summary> 基础值 </summary> <remarks>注意：该值是用于计算的基础数值，并非最终数值 </remarks>
        public double DefValue { get => this.defValue; set { if (this.always) this.defValue = value; else if (this.defValue != value) { this.defValue = value; this.updated = false; } } }
        /// <summary> 最终值下限 </summary> <remarks> 默认为double.MinValue（-1.7976931348623157E+308） </remarks>
        public double MinValue { get => this.minValue; set { if (this.always) this.minValue = value; else if (this.minValue != value) { this.minValue = value; this.updated = false; } } }
        /// <summary> 最终值上限 </summary> <remarks> 默认为double.MaxValue（1.7976931348623157E+308） </remarks>
        public double MaxValue { get => this.maxValue; set { if (this.always) this.maxValue = value; else if (this.maxValue != value) { this.maxValue = value; this.updated = false; } } }
        /// <summary> 最终值 </summary>
        public double Value => this.GetValue();
        /// <summary> 常时更新<para>每次获取值时都会重新进行计算，并禁用同一对象中的事件OnModifyValue</para> 默认为false（禁用） </summary>
        public bool AlwaysUpdate { get => this.always; set => this.SetUpdateType(value); }
        /// <summary> 输出文本格式化 </summary>
        public Func<Statistic, string> Format { get; set; } = null;
        /// <summary> 值更新事件 </summary>
        /// <remarks>
        /// 尝试计算最终值时，若计算结果与原值不一致则执行事件的委托
        /// <para>若同一对象中的属性AlwaysUpdate=true（启用），该事件下的所有委托不会被执行</para>
        /// <para>传参为对象本身，但需要注意避免下述情况发生：</para>
        /// <para>- 在所有委托完成前，若其中有委托修改该传参内部的值，并尝试再一次调用GetValue，则会抛出无限递归警告，同时本次调用会跳过二次计算和事件执行</para>
        /// </remarks>
        public event Action<Statistic> OnModifyValue;

        private Dictionary<string, Factor> modifiers = new Dictionary<string, Factor>();
        private double defValue;
        private double minValue;
        private double maxValue;
        private double value;
        private bool updated;
        private bool always;
        private bool strlock;

        public bool this[Factor modifier]
        {
            get => this.modifiers.Values.Contains(modifier);
            set { if (value) this.AddFactor(modifier); else this.RemoveModifier(modifier); }
        }

        public Factor this[string name] => this.modifiers.TryGetValue(name, out Factor modifier) ? modifier : null;

        public Statistic(string name, double value, double? min = null, double? max = null)
        {
            this.Name = AssetManager.GetText("Statistic." + name);
            this.defValue = value;
            this.minValue = min ?? double.MinValue;
            this.maxValue = max ?? double.MaxValue;
            this.modifiers = new Dictionary<string, Factor>();
            this.updated = false;
            this.strlock = false;
        }

        public void Reset()
        {
            foreach (Factor key in this.modifiers.Values)
                key.OnChanged -= this.LazyFlag;
            this.modifiers.Clear();
            this.updated = false;
        }

        public bool AddFactor(Factor modifier)
        {
            if (this.modifiers.TryAdd(modifier.Key, modifier))
            {
                if (!this.always)
                {
                    this.modifiers[modifier.Key].OnChanged += this.LazyFlag;
                    this.updated = false;
                }
                return true;
            }
            return false;
        }

        public bool RemoveModifier(Factor modifier)
        {
            if (this[modifier])
            {
                if (!this.always)
                {
                    this.modifiers[modifier.Key].OnChanged -= this.LazyFlag;
                    this.updated = false;
                }
                this.modifiers.Remove(modifier.Key);
                return true;
            }
            return false;
        }

        public double GetValue()
        {
            if (this.updated) return this.value;
            this.value = this.defValue;
            foreach (Factor modifier in this.modifiers.Values)
                modifier.Apply(this.defValue, ref this.value);
            this.updated = true;
            return this.value;
        }

        public void SetValue(double defValue, double? minValue = null, double? maxValue = null)
        {
            if (defValue != this.defValue) { this.defValue = defValue; this.updated = false; }
            if (minValue != null && minValue != this.minValue) { this.minValue = minValue.Value; this.updated = false; }
            if (maxValue != null && maxValue != this.maxValue) { this.maxValue = maxValue.Value; this.updated = false; }
        }

        private void SetUpdateType(bool value)
        {
            if (this.always != value)
                if (this.always = value)
                    foreach (string key in this.modifiers.Keys)
                        this.modifiers[key].OnChanged -= this.LazyFlag;
                else foreach (string key in this.modifiers.Keys)
                        this.modifiers[key].OnChanged += this.LazyFlag;
        }

        public static Statistic operator +(Statistic stat, Factor modifier)
        {
            stat.AddFactor(modifier);
            return stat;
        }

        public static Statistic operator -(Statistic stat, Factor modifier)
        {
            stat.RemoveModifier(modifier);
            return stat;
        }

        private void LazyFlag(double val) => this.updated = false;

        public static implicit operator int(Statistic stat) => stat.GetValue().Round();
        public static implicit operator double(Statistic stat) => stat.GetValue();
        public static implicit operator string(Statistic stat) => stat.ToString();

        public override string ToString()
        {
            string str = this.GetValue().ToString("0.0%");
            if (this.Format != null && !this.strlock)
            {
                this.strlock = true;
                str = this.Format.Invoke(this);
                this.strlock = false;
            }
            return str;
        }

        public bool Equals(Statistic x, Statistic y) => x.Name == y.Name;
        public int GetHashCode([DisallowNull] Statistic obj) => obj.Name.GetHashCode();
        public IEnumerator<Factor> GetEnumerator() => this.modifiers.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.modifiers.GetEnumerator();
    }
    */
    public abstract class Statistic
    {
        public UniqueToken Key { get; }
        public string DisplayName => AssetManager.GetText(this.Key);
        public event Action<Statistic> Updated;
        private protected void OnUpdated() => this.Updated?.Invoke(this);
        public Statistic(string name)
        {
            if (name.IsNull()) name = new UniqueToken(this.GetType().Name, '_').LastToken.Token;
            this.Key = new UniqueToken($"Statistic.Vanilla.{name}", '.');
        }
    }

    public class Statistic<T>(T def, T min, T max, string name = null) : Statistic(name), IEnumerable<Factor<T>> where T : INumber<T>, IMinMaxValue<T>
    {
        public T Value { get => this.updated ? field : field = this.GetValue(); }
        public T DefValue { get => field; set { if (field != value) { field = value; this.updated = false; } } } = def;
        public T MinValue { get => field; set { if (field != value) { field = value; this.updated = false; } } } = min;
        public T MaxValue { get => field; set { if (field != value) { field = value; this.updated = false; } } } = max;
        public Factor<T> this[string name] => this.modifiers.TryGetValue(name, out Factor<T> modifier) ? modifier : null;
        private Dictionary<UniqueToken, Factor<T>> modifiers => field = this.GetModifiers(field);
        public virtual Func<Statistic<T>, string> Format { get; set; } = null;
        public event Action<T, T> ValueChanged;
        private bool updated = false;
        private bool strLock = false;

        public bool this[Factor<T> modifier]
        {
            get => this.modifiers.ContainsKey(modifier.Key);
            set { if (value) this.AddModifier(modifier); else this.RemoveModifier(modifier); }
        }

        private void LazyFlag(T adder, double multiplier) { T value = this.Value; this.updated = false; this.ValueChanged?.Invoke(value, this.Value); this.OnUpdated(); }

        private void ModifyValue(Factor<T> modifier, T def, ref T curr)
        {
            this.OnModifyValue(modifier, def, ref curr);
            curr += modifier.Adder;
            curr = T.CreateSaturating(Convert.ToDouble(curr) * modifier.Multiplier);
        }

        private T GetValue()
        {
            T val = this.DefValue;
            this.PreModifyValue(this.DefValue, ref val);
            foreach (Factor<T> modifier in this.modifiers.Values)
                this.ModifyValue(modifier, this.DefValue, ref val);
            this.PostModifyValue(this.DefValue, ref val);
            this.updated = true;
            return val.Clamp(this.MinValue, this.MaxValue);
        }

        private Dictionary<UniqueToken, Factor<T>> GetModifiers(Dictionary<UniqueToken, Factor<T>> field)
        {
            if (field == null)
            {
                field = new();
                foreach (PropertyInfo prop in this.GetAllGetOnlyProperties())
                    if (typeof(Factor<T>).IsAssignableFrom(prop.PropertyType) && prop.GetValue(this) is Factor<T> modifier)
                        if (!field.ContainsKey(modifier.Key))
                        {
                            field.Add(modifier.Key, modifier);
                            modifier.ValueChanged += this.LazyFlag;
                        }
            }
            return field;
        }


        private IEnumerable<PropertyInfo> GetAllGetOnlyProperties()
        {
            var seen = new HashSet<string>();      // 去重：按属性名
            var result = new List<PropertyInfo>();
            Type type = this.GetType();
            for (Type t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                PropertyInfo[] props = t.GetProperties(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly);      // ← 只拿本层声明的

                foreach (PropertyInfo prop in props)
                {
                    if (!seen.Add(prop.Name)) continue;   // 已经被派生类覆盖，跳过
                    // 1) 必须有 getter（含 non-public）
                    MethodInfo getter = prop.GetGetMethod(nonPublic: true);
                    if (getter is null) continue;
                    // 2) 必须没有 setter（含 non-public，含 init）
                    if (prop.GetSetMethod(nonPublic: true) is not null) continue;
                    // 3) 排除 abstract（无实现，Invoke 会崩）
                    if (getter.IsAbstract) continue;
                    // 4) 排除索引器
                    if (getter.GetParameters().Length != 0) continue;
                    result.Add(prop);
                }
            }
            return result;
        }

        public void ClearModifiers()
        {
            foreach (Factor<T> factor in this.modifiers.Values) factor.ValueChanged -= this.LazyFlag;
            this.modifiers.Clear();
            this.updated = false;
        }

        public Factor<T> AddModifier(Factor<T> modifier)
        {
            if (this.modifiers.TryAdd(modifier.Key, modifier))
            {
                modifier.ValueChanged += this.LazyFlag;
                this.updated = false;
                return modifier;
            }
            return null;
        }

        public bool RemoveModifier(Factor<T> modifier)
        {
            if (this[modifier])
            {
                modifier.ValueChanged -= this.LazyFlag;
                this.updated = false;
                this.modifiers.Remove(modifier.Key);
                return true;
            }
            return false;
        }

        public static Statistic<T> operator +(Statistic<T> stat, Factor<T> modifier)
        {
            stat.AddModifier(modifier);
            return stat;
        }

        public static Statistic<T> operator -(Statistic<T> stat, Factor<T> modifier)
        {
            stat.RemoveModifier(modifier);
            return stat;
        }

        public static implicit operator T(Statistic<T> stat) => stat.Value;

        /// <summary> 修改值预处理 <para>此方法每次更新值前只调用一次</para> </summary>
        /// <param name="def">原值</param>
        /// <param name="curr">当前结果值</param>
        protected virtual void PreModifyValue(T def, ref T curr) { }
        /// <summary> 修改值后处理 <para>此方法每次更新值后只调用一次</para>  </summary>
        /// <param name="def">原值</param>
        /// <param name="curr">当前结果值</param>
        protected virtual void PostModifyValue(T def, ref T curr) { }
        /// <summary> 修改值时处理 <para>此方法每次更新值时，对每个修改系数调用一次</para> 原生计算逻辑在此方法后调用，因此可通过直接在修改系数的 <see cref="Factor{T}.Adder"/> 和 <see cref="Factor{T}.Multiplier"/> 从而改变结果  </summary>
        protected virtual void OnModifyValue(Factor<T> modifier, T def, ref T curr) { }
        public sealed override string ToString()
        {
            string str = this.Value.ToString();
            if (!this.strLock && this.Format != null) 
            {
                this.strLock = true;
                str = this.Format.Invoke(this);
                this.strLock = false;
            }
            return str;
        }

        public IEnumerator<Factor<T>> GetEnumerator() => this.modifiers.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.modifiers.GetEnumerator();
    }

    /*
    public class StatisticCollection
    {
        public Statistic MaxLife => this[StatisticRef.Attr_MaxLife];
        public Statistic MaxMana => this[StatisticRef.Attr_MaxMana];
        public Statistic Attack => this[StatisticRef.Attr_Attack];
        public Statistic Defense => this[StatisticRef.Attr_Defense];
        public Statistic MovementSpeed => this[StatisticRef.Attr_MovementSpeed];
        public Statistic JumpStrength => this[StatisticRef.Attr_JumpStrength];
        public Statistic CriticalChance => this[StatisticRef.Critical_Chance];
        public Statistic CriticalDamage => this[StatisticRef.Critical_Damage];

        public Statistic PrimaryPow => this[StatisticRef.Power_Primary];
        public Statistic ElementalPow => this[StatisticRef.Power_Elemental];
        public Statistic SlashPow => this[StatisticRef.Power_Slash];
        public Statistic BluntPow => this[StatisticRef.Power_Blunt];
        public Statistic PiercePow => this[StatisticRef.Power_Pierce];
        public Statistic SpellPow => this[StatisticRef.Power_Spell];
        public Statistic ThermoPow => this[StatisticRef.Power_Thermo];
        public Statistic HydroPow => this[StatisticRef.Power_Hydro];
        public Statistic CyroPow => this[StatisticRef.Power_Cyro];
        public Statistic BrontoPow => this[StatisticRef.Power_Bronto];
        public Statistic AeroPow => this[StatisticRef.Power_Aero];
        public Statistic PetroPow => this[StatisticRef.Power_Petro];

        public Statistic PrimaryRes => this[StatisticRef.Resistance_Primary];
        public Statistic ElementalRes => this[StatisticRef.Resistance_Elemental];
        public Statistic SlashRes => this[StatisticRef.Resistance_Slash];
        public Statistic BluntRes => this[StatisticRef.Resistance_Blunt];
        public Statistic PierceRes => this[StatisticRef.Resistance_Pierce];
        public Statistic SpellRes => this[StatisticRef.Resistance_Spell];
        public Statistic ThermoRes => this[StatisticRef.Resistance_Thermo];
        public Statistic HydroRes => this[StatisticRef.Resistance_Hydro];
        public Statistic CyroRes => this[StatisticRef.Resistance_Cyro];
        public Statistic BrontoRes => this[StatisticRef.Resistance_Bronto];
        public Statistic AeroRes => this[StatisticRef.Resistance_Aero];
        public Statistic PetroRes => this[StatisticRef.Resistance_Petro];

        public Statistic[] StatisticArray => this.stats.Values.ToArray();
        public DataReference[] ReferenceArray => this.stats.Keys.ToArray();

        private Dictionary<DataReference, Statistic> stats = new()
        {
            { StatisticRef.Attr_MaxLife, new Statistic(StatisticRef.Attr_MaxLife, 0d, null, null) }, { StatisticRef.Attr_MaxMana, new Statistic(StatisticRef.Attr_MaxMana, 0d, null, null) },
            { StatisticRef.Attr_Attack, new Statistic(StatisticRef.Attr_Attack, 0d, null, null) }, { StatisticRef.Attr_Defense, new Statistic(StatisticRef.Attr_Defense, 0d, null, null) },
            { StatisticRef.Attr_MovementSpeed, new Statistic(StatisticRef.Attr_MovementSpeed, 0d, null, null) }, { StatisticRef.Attr_JumpStrength, new Statistic(StatisticRef.Attr_JumpStrength, 0d, null, null) },
            { StatisticRef.Critical_Chance, new Statistic(StatisticRef.Critical_Chance, 0d, null, null) }, { StatisticRef.Critical_Damage, new Statistic(StatisticRef.Critical_Damage, 0d, null, null) },
            { StatisticRef.Power_Direct, new Statistic(StatisticRef.Power_Direct, 0d, null, null) }, { StatisticRef.Resistance_Direct, new Statistic(StatisticRef.Resistance_Direct, 0d, null, null) },
            { StatisticRef.Power_Indirect, new Statistic(StatisticRef.Power_Indirect, 0d, null, null) }, { StatisticRef.Resistance_Indirect, new Statistic(StatisticRef.Resistance_Indirect, 0d, null, null) },
            { StatisticRef.Power_Primary, new Statistic(StatisticRef.Power_Primary, 0d, null, null) }, { StatisticRef.Resistance_Primary, new Statistic(StatisticRef.Resistance_Primary, 0d, null, null) },
            { StatisticRef.Power_Elemental, new Statistic(StatisticRef.Power_Elemental, 0d, null, null) }, { StatisticRef.Resistance_Elemental, new Statistic(StatisticRef.Resistance_Elemental, 0d, null, null) },
            { StatisticRef.Power_Slash, new Statistic(StatisticRef.Power_Slash, 0d, null, null) }, { StatisticRef.Resistance_Slash, new Statistic(StatisticRef.Resistance_Slash, 0d, null, null) },
            { StatisticRef.Power_Blunt, new Statistic(StatisticRef.Power_Blunt, 0d, null, null) }, { StatisticRef.Resistance_Blunt, new Statistic(StatisticRef.Resistance_Blunt, 0d, null, null) },
            { StatisticRef.Power_Pierce, new Statistic(StatisticRef.Power_Pierce, 0d, null, null) }, { StatisticRef.Resistance_Pierce, new Statistic(StatisticRef.Resistance_Pierce, 0d, null, null) },
            { StatisticRef.Power_Spell, new Statistic(StatisticRef.Power_Spell, 0d, null, null) }, { StatisticRef.Resistance_Spell, new Statistic(StatisticRef.Resistance_Spell, 0d, null, null) },
            { StatisticRef.Power_Thermo, new Statistic(StatisticRef.Power_Thermo, 0d, null, null) }, { StatisticRef.Resistance_Thermo, new Statistic(StatisticRef.Resistance_Thermo, 0d, null, null) },
            { StatisticRef.Power_Cyro, new Statistic(StatisticRef.Power_Cyro, 0d, null, null) }, { StatisticRef.Resistance_Hydro, new Statistic(StatisticRef.Resistance_Hydro, 0d, null, null) },
            { StatisticRef.Power_Hydro, new Statistic(StatisticRef.Power_Hydro, 0d, null, null) }, { StatisticRef.Resistance_Cyro, new Statistic(StatisticRef.Resistance_Cyro, 0d, null, null) },
            { StatisticRef.Power_Bronto, new Statistic(StatisticRef.Power_Bronto, 0d, null, null) }, { StatisticRef.Resistance_Bronto, new Statistic(StatisticRef.Resistance_Bronto, 0d, null, null) },
            { StatisticRef.Power_Aero, new Statistic(StatisticRef.Power_Aero, 0d, null, null) }, { StatisticRef.Resistance_Aero, new Statistic(StatisticRef.Resistance_Aero, 0d, null, null) },
            { StatisticRef.Power_Petro, new Statistic(StatisticRef.Power_Petro, 0d, null, null) }, { StatisticRef.Resistance_Petro, new Statistic(StatisticRef.Resistance_Petro, 0d, null, null) },
            { StatisticRef.Attr_Force, new Statistic(StatisticRef.Attr_Force, 0d, null, null) }, { StatisticRef.Attr_Poise, new Statistic(StatisticRef.Attr_Poise, 0d, null, null) },
        };

        public Statistic this[DataReference @ref]
        {
            get => this.stats.ContainsKey(@ref) ? this.stats[@ref] : null;
            set { if (this.stats.ContainsKey(@ref)) this.stats[@ref].SetValue(value.DefValue, value.MinValue, value.MaxValue); }
        }

        public bool AddStat(DataReference<StatisticRef> statRef, double value, double? min = null, double? max = null)
        {
            return this.stats.TryAdd(statRef, new Statistic(statRef, value, min, max));
        }

        public Statistic RemoveStat(DataReference<StatisticRef> statRef)
        {
            Statistic stat = null;
            if (this.stats.TryGetValue(statRef, out stat)) this.stats.Remove(statRef);
            return stat;
        }

        public bool ModifyStat(DataReference<StatisticRef> statRef, double newValue, double? newMin = null, double? newMax = null)
        {
            bool res = this.stats.TryGetValue(statRef, out Statistic statistic);
            statistic?.SetValue(newValue, newMin, newMax);
            return res;
        }
    }
    */
    public class CharacterStatisticCollection : StatisticCollection
    {
        #region ClassDefine
        public class Character_MaxLife(int def) : Statistic<int>(def, 0, int.MaxValue)
        {
            public Factor<int> ArmorAdder { get; } = new Factor<int>(nameof(ArmorAdder), new Color(), new Color()) { Multiplier = 2d };
        }

        public class Character_MaxMana(int def) : Statistic<int>(def, 0, 9000) { }
        public class Character_MaxStamina(int def) : Statistic<int>(def, 1000, 9000) { }
        public class Character_Attack(int def) : Statistic<int>(def, 1, int.MaxValue)
        {
            public Factor<int> SwordBreeze { get; } = new Factor<int>(nameof(SwordBreeze), new Color(), new Color()) { Multiplier = 1d };
        }
        public class Character_Defense(int def) : Statistic<int>(def, int.MinValue, int.MaxValue) { }
        public class Character_Force(int def) : Statistic<int>(def, int.MinValue, int.MaxValue) { }
        public class Character_Poise(int def) : Statistic<int>(def, int.MinValue, int.MaxValue) { }
        public class Character_MovementSpeed(float def) : Statistic<float>(def, .33f, 3f) { }
        public class Character_JumpStrength(float def) : Statistic<float>(def, .33f, 3f) { }
        public abstract class Character_Critical(float def, float min, float max, string name) : Statistic<float>(def, min, max, new UniqueToken(new UniqueToken(nameof(Character_Critical), '_').LastToken.Token, '.', new UniqueToken(name)))
        {
            public override Func<Statistic<float>, string> Format { get; set; } = (v) => string.Format("{0:0.0%} ({1:0);-0);±0})", v.Value, v.Value - v.DefValue);
            public class Chance(float def) : Character_Critical(def, 0f, 1f, nameof(Chance));
            public class Damage(float def) : Character_Critical(def, -4f, 4f, nameof(Damage));
        }

        public abstract class Character_Power(string name) : Statistic<float>(0f, -2f, 2f, new UniqueToken(new UniqueToken(nameof(Character_Power), '_').LastToken.Token, '.', new UniqueToken(name)))
        {
            public class Direct() : Character_Power(nameof(Direct));
            public class Indirect() : Character_Power(nameof(Indirect));
            public class Primary() : Character_Power(nameof(Primary));
            public class Elemental() : Character_Power(nameof(Elemental));
            public class Slash() : Character_Power(nameof(Slash));
            public class Blunt() : Character_Power(nameof(Blunt));
            public class Pierce() : Character_Power(nameof(Pierce));
            public class Spell() : Character_Power(nameof(Spell));
            public class Thermo() : Character_Power(nameof(Thermo));
            public class Hydro() : Character_Power(nameof(Hydro));
            public class Cyro() : Character_Power(nameof(Cyro));
            public class Bronto() : Character_Power(nameof(Bronto));
            public class Aero() : Character_Power(nameof(Aero));
            public class Petro() : Character_Power(nameof(Petro));
        }

        public abstract class Character_Resistance(string name) : Statistic<float>(0f, -2f, 2f, new UniqueToken(new UniqueToken(nameof(Character_Resistance), '_').LastToken.Token, '.', new UniqueToken(name)))
        {
            public class Direct() : Character_Resistance(nameof(Direct));
            public class Indirect() : Character_Resistance(nameof(Indirect));
            public class Primary() : Character_Resistance(nameof(Primary));
            public class Elemental() : Character_Resistance(nameof(Elemental));
            public class Slash() : Character_Resistance(nameof(Slash));
            public class Blunt() : Character_Resistance(nameof(Blunt));
            public class Pierce() : Character_Resistance(nameof(Pierce));
            public class Spell() : Character_Resistance(nameof(Spell));
            public class Thermo() : Character_Resistance(nameof(Thermo));
            public class Hydro() : Character_Resistance(nameof(Hydro));
            public class Cyro() : Character_Resistance(nameof(Cyro));
            public class Bronto() : Character_Resistance(nameof(Bronto));
            public class Aero() : Character_Resistance(nameof(Aero));
            public class Petro() : Character_Resistance(nameof(Petro));
        }
        #endregion

        public override Character_MaxLife MaxLife { get; } = new(3000) { Format = StatisticCollection.IntFormat };
        public override Character_MaxMana MaxMana { get; } = new(3000) { Format = StatisticCollection.IntFormat };
        public Character_MaxStamina MaxStamina { get; } = new(3000) { Format = StatisticCollection.IntFormat };
        public override Character_Attack Attack { get; } = new(1) { Format = StatisticCollection.IntFormat };
        public override Character_Defense Defense { get; } = new(0) { Format = StatisticCollection.IntFormat };
        public override Character_Force Force { get; } = new(100);
        public override Character_Poise Poise { get; } = new(100);
        public override Character_MovementSpeed MovementSpeed { get; } = new(1f);
        public override Character_JumpStrength JumpStrength { get; } = new(1f);
        public override Character_Critical.Chance CritChance { get; } = new(0f);
        public override Character_Critical.Damage CritDamage { get; } = new(.25f);
        public override Character_Power.Direct DirectPower { get; } = new();
        public override Character_Power.Indirect IndirectPower { get; } = new();
        public override Character_Power.Primary PrimaryPower { get; } = new();
        public override Character_Power.Elemental ElementalPower { get; } = new();
        public override Character_Power.Slash SlashPower { get; } = new();
        public override Character_Power.Blunt BluntPower { get; } = new();
        public override Character_Power.Pierce PiercePower { get; } = new();
        public override Character_Power.Spell SpellPower { get; } = new();
        public override Character_Power.Thermo ThermoPower { get; } = new();
        public override Character_Power.Hydro HydroPower { get; } = new();
        public override Character_Power.Cyro CyroPower { get; } = new();
        public override Character_Power.Bronto BrontoPower { get; } = new();
        public override Character_Power.Aero AeroPower { get; } = new();
        public override Character_Power.Petro PetroPower { get; } = new();
        public override Character_Resistance.Direct DirectResistance { get; } = new();
        public override Character_Resistance.Indirect IndirectResistance { get; } = new();
        public override Character_Resistance.Primary PrimaryResistance { get; } = new();
        public override Character_Resistance.Elemental ElementalResistance { get; } = new();
        public override Character_Resistance.Slash SlashResistance { get; } = new();
        public override Character_Resistance.Blunt BluntResistance { get; } = new();
        public override Character_Resistance.Pierce PierceResistance { get; } = new();
        public override Character_Resistance.Spell SpellResistance { get; } = new();
        public override Character_Resistance.Thermo ThermoResistance { get; } = new();
        public override Character_Resistance.Hydro HydroResistance { get; } = new();
        public override Character_Resistance.Cyro CyroResistance { get; } = new();
        public override Character_Resistance.Bronto BrontoResistance { get; } = new();
        public override Character_Resistance.Aero AeroResistance { get; } = new();
        public override Character_Resistance.Petro PetroResistance { get; } = new();
    }

    public class EnemyStatisticCollection : StatisticCollection
    {
        #region ClassDefine
        public class Enemy_MaxLife(int def) : Statistic<int>(def, 0, int.MaxValue) { }

        public class Enemy_MaxMana(int def) : Statistic<int>(def, 0, 9000) { }
        public class Enemy_Attack(int def) : Statistic<int>(def, 1, int.MaxValue) { }
        public class Enemy_Defense(int def) : Statistic<int>(def, int.MinValue, int.MaxValue) { }
        public class Enemy_Force(int def) : Statistic<int>(def, int.MinValue, int.MaxValue) { }
        public class Enemy_Poise(int def) : Statistic<int>(def, int.MinValue, int.MaxValue) { }
        public class Enemy_MovementSpeed(float def) : Statistic<float>(def, .33f, 3f) { }
        public class Enemy_JumpStrength(float def) : Statistic<float>(def, .33f, 3f) { }
        public abstract class Enemy_Critical(float def, float min, float max, string name) : Statistic<float>(def, min, max, new UniqueToken(new UniqueToken(nameof(Enemy_Critical), '_').LastToken.Token, '.', new UniqueToken(name)))
        {
            //public override Func<Statistic2<float>, string> Format { get; set; } = (v) => string.Format("{0:0.0%} (+{1:0);-0);±0})", v.Value, v.Value - v.DefValue);
            public class Chance(float def) : Enemy_Critical(def, 0f, 1f, nameof(Chance));
            public class Damage(float def) : Enemy_Critical(def, -4f, 4f, nameof(Damage));
        }

        public abstract class Enemy_Power(string name) : Statistic<float>(0f, -2f, 2f, new UniqueToken(new UniqueToken(nameof(Enemy_Power), '_').LastToken.Token, '.', new UniqueToken(name)))
        {
            public class Direct() : Enemy_Power(nameof(Direct));
            public class Indirect() : Enemy_Power(nameof(Indirect));
            public class Primary() : Enemy_Power(nameof(Primary));
            public class Elemental() : Enemy_Power(nameof(Elemental));
            public class Slash() : Enemy_Power(nameof(Slash));
            public class Blunt() : Enemy_Power(nameof(Blunt));
            public class Pierce() : Enemy_Power(nameof(Pierce));
            public class Spell() : Enemy_Power(nameof(Spell));
            public class Thermo() : Enemy_Power(nameof(Thermo));
            public class Hydro() : Enemy_Power(nameof(Hydro));
            public class Cyro() : Enemy_Power(nameof(Cyro));
            public class Bronto() : Enemy_Power(nameof(Bronto));
            public class Aero() : Enemy_Power(nameof(Aero));
            public class Petro() : Enemy_Power(nameof(Petro));
        }

        public abstract class Enemy_Resistance(string name) : Statistic<float>(0f, -2f, 2f, new UniqueToken(new UniqueToken(nameof(Enemy_Resistance), '_').LastToken.Token, '.', new UniqueToken(name)))
        {
            public class Direct() : Enemy_Resistance(nameof(Direct));
            public class Indirect() : Enemy_Resistance(nameof(Indirect));
            public class Primary() : Enemy_Resistance(nameof(Primary));
            public class Elemental() : Enemy_Resistance(nameof(Elemental));
            public class Slash() : Enemy_Resistance(nameof(Slash));
            public class Blunt() : Enemy_Resistance(nameof(Blunt));
            public class Pierce() : Enemy_Resistance(nameof(Pierce));
            public class Spell() : Enemy_Resistance(nameof(Spell));
            public class Thermo() : Enemy_Resistance(nameof(Thermo));
            public class Hydro() : Enemy_Resistance(nameof(Hydro));
            public class Cyro() : Enemy_Resistance(nameof(Cyro));
            public class Bronto() : Enemy_Resistance(nameof(Bronto));
            public class Aero() : Enemy_Resistance(nameof(Aero));
            public class Petro() : Enemy_Resistance(nameof(Petro));
        }
        #endregion

        public override Enemy_MaxLife MaxLife { get; } = new(3000) { Format = StatisticCollection.IntFormat };
        public override Enemy_MaxMana MaxMana { get; } = new(3000) { Format = StatisticCollection.IntFormat };
        public override Enemy_Attack Attack { get; } = new(1) { Format = StatisticCollection.IntFormat };
        public override Enemy_Defense Defense { get; } = new(0) { Format = StatisticCollection.IntFormat };
        public override Enemy_Force Force { get; } = new(100);
        public override Enemy_Poise Poise { get; } = new(100);
        public override Enemy_MovementSpeed MovementSpeed { get; } = new(1f);
        public override Enemy_JumpStrength JumpStrength { get; } = new(1f);
        public override Enemy_Critical.Chance CritChance { get; } = new(0f);
        public override Enemy_Critical.Damage CritDamage { get; } = new(.25f);
        public override Enemy_Power.Direct DirectPower { get; } = new();
        public override Enemy_Power.Indirect IndirectPower { get; } = new();
        public override Enemy_Power.Primary PrimaryPower { get; } = new();
        public override Enemy_Power.Elemental ElementalPower { get; } = new();
        public override Enemy_Power.Slash SlashPower { get; } = new();
        public override Enemy_Power.Blunt BluntPower { get; } = new();
        public override Enemy_Power.Pierce PiercePower { get; } = new();
        public override Enemy_Power.Spell SpellPower { get; } = new();
        public override Enemy_Power.Thermo ThermoPower { get; } = new();
        public override Enemy_Power.Hydro HydroPower { get; } = new();
        public override Enemy_Power.Cyro CyroPower { get; } = new();
        public override Enemy_Power.Bronto BrontoPower { get; } = new();
        public override Enemy_Power.Aero AeroPower { get; } = new();
        public override Enemy_Power.Petro PetroPower { get; } = new();
        public override Enemy_Resistance.Direct DirectResistance { get; } = new();
        public override Enemy_Resistance.Indirect IndirectResistance { get; } = new();
        public override Enemy_Resistance.Primary PrimaryResistance { get; } = new();
        public override Enemy_Resistance.Elemental ElementalResistance { get; } = new();
        public override Enemy_Resistance.Slash SlashResistance { get; } = new();
        public override Enemy_Resistance.Blunt BluntResistance { get; } = new();
        public override Enemy_Resistance.Pierce PierceResistance { get; } = new();
        public override Enemy_Resistance.Spell SpellResistance { get; } = new();
        public override Enemy_Resistance.Thermo ThermoResistance { get; } = new();
        public override Enemy_Resistance.Hydro HydroResistance { get; } = new();
        public override Enemy_Resistance.Cyro CyroResistance { get; } = new();
        public override Enemy_Resistance.Bronto BrontoResistance { get; } = new();
        public override Enemy_Resistance.Aero AeroResistance { get; } = new();
        public override Enemy_Resistance.Petro PetroResistance { get; } = new();
    }

    public abstract class StatisticCollection
    {
        private bool statsDirty = true;
        private bool extraDirty = true;
        internal Dictionary<UniqueToken, Statistic> stats = new();
        internal Dictionary<UniqueToken, Statistic> extraStats = new();
        public Statistic[] Array => this.GetArray(ref field);
        public Statistic this[UniqueToken key] => this.extraStats.TryGetValue(key, out Statistic stat) ? stat : this.stats.TryGetValue(key, out stat) ? stat : null;

        public T GetStat<T>(UniqueToken key) where T : Statistic
        {
            if (this.extraStats.ContainsKey(key)) return this.extraStats[key] as T;
            if (this.stats.ContainsKey(key)) return this.stats[key] as T;
            return null;
        }
        public static readonly Func<Statistic<int>, string> IntFormat = (v) => string.Format("{0} ({1:+0;-0;±0})", v.Value, v.Value - v.DefValue);
        public static readonly Func<Statistic<float>, string> FloatFormat = (v) => string.Format("{0:0.0%} ({1:+0;-0);±0})", v.Value, v.Value - v.DefValue);

        private IEnumerable<PropertyInfo> GetAllGetOnlyProperties()
        {
            var seen = new HashSet<string>();      // 去重：按属性名
            var result = new List<PropertyInfo>();
            Type type = this.GetType();
            for (Type t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                PropertyInfo[] props = t.GetProperties((BindingFlags)0b110110);// ← 只拿本层声明的
                /*
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly
                */
                foreach (PropertyInfo prop in props)
                {
                    if (!seen.Add(prop.Name)) continue;   // 已经被派生类覆盖，跳过
                    // 1) 必须有 getter（含 non-public）
                    MethodInfo getter = prop.GetGetMethod(nonPublic: true);
                    if (getter is null) continue;
                    // 2) 必须没有 setter（含 non-public，含 init）
                    if (prop.GetSetMethod(nonPublic: true) is not null) continue;
                    // 3) 排除 abstract（无实现，Invoke 会崩）
                    if (getter.IsAbstract) continue;
                    // 4) 排除索引器
                    if (getter.GetParameters().Length != 0) continue;
                    result.Add(prop);
                }
            }
            return result;
        }

        private Statistic[] GetArray(ref Statistic[] field)
        {
            bool rebuildArray = this.statsDirty || this.extraDirty;
            if (this.statsDirty)
            {
                this.stats.Clear();
                Dictionary<UniqueToken, Statistic> dir = this.stats;
                foreach (PropertyInfo prop in this.GetAllGetOnlyProperties())
                    if (typeof(Statistic).IsAssignableFrom(prop.PropertyType) && prop.GetValue(this) is Statistic stat)
                        if (!dir.ContainsKey(stat.Key))
                            dir.Add(stat.Key, stat);
                this.statsDirty = false;
            }
            if (rebuildArray)
            {
                Dictionary<UniqueToken, Statistic> temp = new();
                foreach (var pair in this.stats)
                    if (!temp.TryAdd(pair.Key, pair.Value))
                        temp[pair.Key] = pair.Value;
                foreach (var pair in this.extraStats)
                    if (!temp.TryAdd(pair.Key, pair.Value))
                        temp[pair.Key] = pair.Value;
                field = temp.Values.ToArray();
                this.extraDirty = false;
            }
            return field;
        }
        
        public void Reset(bool ignoreBases = true, bool ignoreExtras = false) { this.statsDirty |= ignoreBases; this.extraDirty |= ignoreExtras; }
        public void AddStat(Statistic stat) { this.extraDirty = true; this.extraStats.TryAdd(stat.Key, stat); }
        public bool RemoveStat(UniqueToken key) { this.extraDirty = true; return this.extraStats.Remove(key); } // 被覆盖的原生属性可以被还原，但不能被彻底清除

        public abstract Statistic<int> MaxLife { get; }
        public abstract Statistic<int> MaxMana { get; }
        public abstract Statistic<int> Attack { get; }
        public abstract Statistic<int> Defense { get; }
        public abstract Statistic<int> Force { get; }
        public abstract Statistic<int> Poise { get; }
        public abstract Statistic<float> MovementSpeed { get; }
        public abstract Statistic<float> JumpStrength { get; }
        public abstract Statistic<float> CritChance { get; }
        public abstract Statistic<float> CritDamage { get; }
        public abstract Statistic<float> DirectPower { get; }
        public abstract Statistic<float> IndirectPower { get; }
        public abstract Statistic<float> PrimaryPower { get; }
        public abstract Statistic<float> ElementalPower { get; }
        public abstract Statistic<float> SlashPower { get; }
        public abstract Statistic<float> BluntPower { get; }
        public abstract Statistic<float> PiercePower { get; }
        public abstract Statistic<float> SpellPower { get; }
        public abstract Statistic<float> ThermoPower { get; }
        public abstract Statistic<float> HydroPower { get; }
        public abstract Statistic<float> CyroPower { get; }
        public abstract Statistic<float> BrontoPower { get; }
        public abstract Statistic<float> AeroPower { get; }
        public abstract Statistic<float> PetroPower { get; }
        public abstract Statistic<float> DirectResistance { get; }
        public abstract Statistic<float> IndirectResistance { get; }
        public abstract Statistic<float> PrimaryResistance { get; }
        public abstract Statistic<float> ElementalResistance { get; }
        public abstract Statistic<float> SlashResistance { get; }
        public abstract Statistic<float> BluntResistance { get; }
        public abstract Statistic<float> PierceResistance { get; }
        public abstract Statistic<float> SpellResistance { get; }
        public abstract Statistic<float> ThermoResistance { get; }
        public abstract Statistic<float> HydroResistance { get; }
        public abstract Statistic<float> CyroResistance { get; }
        public abstract Statistic<float> BrontoResistance { get; }
        public abstract Statistic<float> AeroResistance { get; }
        public abstract Statistic<float> PetroResistance { get; }
    }
}