using Godot;
using LOI.Localization;
using LOI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static LOI.LegendOfInfinity;
using static LOI.Options;
using FileAccess = Godot.FileAccess;

namespace LOI
{
    public static class Options
    {
        private static int SaveDelay { get; set; } = 0;
        private static Func<float, float, float> SingleCheck { get; } = (d, v) => v.Clamp(0f, 1f);
        private static List<IOption> Registers { get; } = new List<IOption>();

        /// <summary> 左摇杆<see href="死区"/>（左拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="LeftStickDeadzone"/>.<see langword="X"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_LS_Left { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_LS_Left), nameof(Options.LeftStickDeadzone) + ".Left", 0.25f, Options.SingleCheck);

        /// <summary> 左摇杆死区（上拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="LeftStickDeadzone"/>.<see langword="Y"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_LS_Up { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_LS_Up), nameof(Options.LeftStickDeadzone) + ".Up", 0.25f, Options.SingleCheck);

        /// <summary> 左摇杆死区（右拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="LeftStickDeadzone"/>.<see langword="Z"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_LS_Right { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_LS_Right), nameof(Options.LeftStickDeadzone) + ".Right", 0.25f, Options.SingleCheck);

        /// <summary> 左摇杆死区（下拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="LeftStickDeadzone"/>.<see langword="W"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_LS_Down { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_LS_Down), nameof(Options.LeftStickDeadzone) + ".Down", 0.25f, Options.SingleCheck);

        /// <summary> 右摇杆死区（左拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="RightStickDeadzone"/>.<see langword="X"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_RS_Left { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_RS_Left), nameof(Options.RightStickDeadzone) + ".Left", 0.25f, Options.SingleCheck);

        /// <summary> 右摇杆死区（上拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="RightStickDeadzone"/>.<see langword="Y"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_RS_Up { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_RS_Up), nameof(Options.RightStickDeadzone) + ".Up", 0.25f, Options.SingleCheck);

        /// <summary> 右摇杆死区（右拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="RightStickDeadzone"/>.<see langword="Z"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_RS_Right { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_RS_Right), nameof(Options.RightStickDeadzone) + ".Right", 0.25f, Options.SingleCheck);

        /// <summary> 右摇杆死区（下拨） </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="RightStickDeadzone"/>.<see langword="W"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_RS_Down { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_RS_Down), nameof(Options.RightStickDeadzone) + ".Down", 0.25f, Options.SingleCheck);

        /// <summary> 左扳机死区 </summary>
        /// <remarks>
        /// 扳机按下的最低触发阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="TriggerDeadzone"/>.<see langword="X"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_LT { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_LT), nameof(Options.TriggerDeadzone) + ".Left", 0.25f, Options.SingleCheck);

        /// <summary> 右扳机死区 </summary>
        /// <remarks>
        /// 扳机按下的最低触发阈值，范围为0.0f-1.0f，默认值为0.25f（25%）
        /// <para> 作用等同于<see cref="TriggerDeadzone"/>.<see langword="Y"/> </para>
        /// </remarks>
        public static Option<float> Deadzone_RT { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.Deadzone_RT), nameof(Options.TriggerDeadzone) + ".Right", 0.25f, Options.SingleCheck);

        /// <summary> 音乐音量 </summary>
        /// <remarks>
        /// 范围为0.0f-1.0f，默认值为1.0f（100%）
        /// <para> 作用等同于<see cref="Volume"/>.<see langword="X"/> </para>
        /// </remarks>
        public static Option<float> MusicVolume { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.MusicVolume), nameof(Options.Volume) + ".Music", 1.0f, Options.SingleCheck);

        /// <summary> 音效音量 </summary>
        /// <remarks>
        /// 范围为0.0f-1.0f，默认值为1.0f（100%）
        /// <para> 作用等同于<see cref="Volume"/>.<see langword="Y"/> </para>
        /// </remarks>
        public static Option<float> SFXVolume { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.SFXVolume), nameof(Options.Volume) + ".SFX", 1.0f, Options.SingleCheck);

        /// <summary> 环境音量 </summary>
        /// <remarks>
        /// 范围为0.0f-1.0f，默认值为1.0f（100%）
        /// <para> 作用等同于<see cref="Volume"/>.<see langword="Z"/> </para>
        /// </remarks>
        public static Option<float> AmbientVolume { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.AmbientVolume), nameof(Options.Volume) + ".Ambient", 1.0f, Options.SingleCheck);

        /// <summary> 主音量 </summary>
        /// <remarks>
        /// 范围为0.0f-1.0f，默认值为0.5f（50%）
        /// <para> 作用等同于<see cref="Volume"/>.<see langword="W"/> </para>
        /// </remarks>
        public static Option<float> MasterVolume { get => field; set => field.Value = value; } = new Option<float>(nameof(Options.MasterVolume), nameof(Options.Volume) + ".Master", 0.5f, Options.SingleCheck);

        /// <summary> 按键双击缓冲时长（单位为毫秒） </summary>
        /// <remarks>
        /// 同一按键双击的最大有效间隔，默认为15（ms）
        /// <para>范围为1-200（ms）</para>
        /// </remarks>
        public static Option<int> DoubleClickDelay { get => field; set => field.Value = value; } = new Option<int>(nameof(Options.DoubleClickDelay), 15, (d, v) => v.Clamp(1, 200));

        /// <summary> 垂直同步 </summary>
        /// <remarks>
        /// 默认为<see langword="true"/>（启用）
        /// <para><see langword="true"/> - 同步显卡帧率与显示器刷新率，防止画面撕裂。</para>
        /// <para><see langword="false"/> - 帧率平滑，降低输入延迟</para>
        /// </remarks>
        public static Option<bool> VSync { get => field; set => field.Value = value; } = new Option<bool>(nameof(Options.VSync), true);

        /// <summary> 后台静音 </summary>
        /// <remarks>
        /// 默认为<see langword="true"/>（启用）
        /// <para><see langword="true"/> - 当游戏在后台运行时，自动静音游戏音频</para>
        /// <para><see langword="false"/> - 当游戏在后台运行时，正常播放游戏音频</para>
        /// </remarks>
        public static Option<bool> MuteOnUnfocused { get => field; set => field.Value = value; } = new Option<bool>(nameof(Options.MuteOnUnfocused), true);

        /// <summary> 显示模式 </summary>
        /// <remarks>
        /// 默认值为<see langword="Fullscreen"/>（无边框全屏），可供设置的值有以下几种：
        /// <para><see langword="BorderlessWindowed"/> - 无边框窗口</para>
        /// <para><see langword="Windowed"/> - 带边框窗口</para>
        /// <para><see langword="Fullscreen"/> - 无边框全屏</para>
        /// <para><see langword="WindowedFullscreen"/> - 带边框全屏</para>
        /// </remarks>
        public static Option<DisplayMode> DisplayMode { get => field; set => field.Value = value; } = new Option<DisplayMode>(nameof(Options.DisplayMode), LegendOfInfinity.DisplayMode.Fullscreen, (d, v) => Enum.IsDefined(v).IfElse(v, d));

        /// <summary> 画面帧率 </summary>
        /// <remarks>
        /// 默认值为<see cref="FPSLimit.Auto"/>（自动获取并设为当前显示器的刷新率）
        /// <para>可选<see cref="FPSLimit.FPS30"/>、<see cref="FPSLimit.FPS60"/>、<see cref="FPSLimit.FPS120"/>、<see cref="FPSLimit.FPS240"/>、<see cref="FPSLimit.FPS300"/></para>
        /// <para>尝试设置为其他值时，自动调整为<see cref="FPSLimit.Auto"/></para>
        /// </remarks>
        public static Option<FPSLimit> FPSLimit { get => field; set => field.Value = value; } = new Option<FPSLimit>(nameof(Options.FPSLimit), LegendOfInfinity.FPSLimit.Auto, (d, v) => Enum.IsDefined(v).IfElse(v, d));

        /// <summary> 区域语言 </summary>
        /// <remarks>
        /// 默认为<see cref="CultureInfo.CurrentCulture"/>，即当前系统使用的区域语言，若游戏不支持该区域语言，则默认使用英语（en-US）
        /// <para>您可以手动添加自己的区域语言Json文件，只需要复制一份Json文本，并将其重命名为您需要的ISO 639语言代码，即可在游戏设置中调用</para>
        /// <para>文本内容请自行调整，亦能借助AI帮您快速本地化</para>
        /// </remarks>
        public static Option<string> Language { get => field; set => field.Value = value; } = new Option<string>(nameof(Options.Language), Options.GetLocale());

        /// <summary> 游戏音量 </summary>
        /// <remarks>
        /// 包含游戏音量的各个通道，范围为0.0f-1.0f，默认值均为1.0f（100%）
        /// <para> <see langword="X"/> - 音乐音量，作用等同于<see cref="MusicVolume"/> </para> 
        /// <para> <see langword="Y"/> - 音效音量，作用等同于<see cref="SFXVolume"/> </para>
        /// <para> <see langword="Z"/> - 环境音量，作用等同于<see cref="AmbientVolume"/> </para>
        /// <para> <see langword="W"/> - 主音量，作用等同于<see cref="MasterVolume"/> </para>
        /// </remarks>
        public static Vector4 Volume { get => new Vector4(Options.MusicVolume, Options.SFXVolume, Options.AmbientVolume, Options.MasterVolume); set { Options.SaveDelay = 4; Vector4 v4 = value.Clamp(0.0f, 1.0f); Options.MusicVolume = v4.X; Options.SFXVolume = v4.Y; Options.AmbientVolume = v4.Z; Options.MasterVolume = v4.W; } }

        /// <summary> 左摇杆死区 </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，所有方向的默认值均为0.25f（25%）
        /// <para> <see langword="X"/> - 左拨死区，作用等同于<see cref="Deadzone_LS_Left"/> </para>
        /// <para> <see langword="Y"/> - 上拨死区，作用等同于<see cref="Deadzone_LS_Up"/> </para> 
        /// <para> <see langword="Z"/> - 右拨死区，作用等同于<see cref="Deadzone_LS_Right"/> </para>
        /// <para> <see langword="W"/> - 下拨死区，作用等同于<see cref="Deadzone_LS_Down"/> </para>
        /// </remarks>
        public static Vector4 LeftStickDeadzone { get => new Vector4(Options.Deadzone_LS_Left, Options.Deadzone_LS_Up, Options.Deadzone_LS_Right, Options.Deadzone_LS_Down); set { Options.SaveDelay = 4; Vector4 v4 = value.Clamp(0.0f, 1.0f); Options.Deadzone_LS_Left = v4.X; Options.Deadzone_LS_Up = v4.Y; Options.Deadzone_LS_Right = v4.Z; Options.Deadzone_LS_Down = v4.W; } }

        /// <summary> 右摇杆死区 </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，所有方向的默认值均为0.25f（25%）
        /// <para> <see langword="X"/> - 左拨死区，作用等同于<see cref="Deadzone_RS_Left"/> </para>
        /// <para> <see langword="Y"/> - 上拨死区，作用等同于<see cref="Deadzone_RS_Up"/> </para> 
        /// <para> <see langword="Z"/> - 右拨死区，作用等同于<see cref="Deadzone_RS_Right"/> </para>
        /// <para> <see langword="W"/> - 下拨死区，作用等同于<see cref="Deadzone_RS_Down"/> </para>
        /// </remarks>
        public static Vector4 RightStickDeadzone { get => new Vector4(Options.Deadzone_RS_Left, Options.Deadzone_RS_Up, Options.Deadzone_RS_Right, Options.Deadzone_RS_Down); set { Options.SaveDelay = 4; Vector4 v4 = value.Clamp(0.0f, 1.0f); Options.Deadzone_RS_Left = v4.X; Options.Deadzone_RS_Up = v4.Y; Options.Deadzone_RS_Right = v4.Z; Options.Deadzone_RS_Down = v4.W; } }

        /// <summary> 扳机死区 </summary>
        /// <remarks>
        /// 摇杆拨动触发的最低阈值，范围为0.0f-1.0f，左右扳机的默认值均为0.25f（25%）
        /// <para> <see langword="X"/> - 即左扳机死区，作用等同于<see cref="Deadzone_LT"/> </para>
        /// <para> <see langword="Y"/> - 即右扳机死区，作用等同于<see cref="Deadzone_RT"/> </para> 
        /// </remarks>
        public static Vector2 TriggerDeadzone { get => new Vector2(Options.Deadzone_LT, Options.Deadzone_RT); set { Vector2 v2 = value.Clamp(0.0f, 1.0f); Options.Deadzone_LT = v2.X; Options.Deadzone_RT = v2.Y; } }

        private static string GetLocale()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            while (info.Parent.Name.Contains("-"))
                info = info.Parent;
            // 检测游戏是否支持当前系统的区域语言，如果不支持，则默认使用英语（en-US）
            return info.Name;
        }

        /// <summary> 初始化配置文件 </summary>
        public static void Initialization()
        {
            Options.Reset();
            //Options.Load();
            //Options.Save();
            Options.Load("res://Config.json");
            Options.Save("res://Config.json");
        }

        /// <summary> 重置当前所有选项配置为默认值 </summary>
        /// <param name="saveFile">重置后立即保存配置文件的路径，若为<see langword="null"/>或为空字符串则不保存</param>
        /// <param name="whitelist">指定需要重置的配置项编号，若为<see langword="null"/>或为空数组则重置所有配置项</param>
        public static void Reset(string saveFile = null, params int[] whitelist)
        {
            if (whitelist == null || whitelist.Length == 0)
                foreach (IOption option in Options.Registers)
                    option.Reset(false);
            else foreach (int id in whitelist)
                Options.Registers[id].Reset(false);
            if (!string.IsNullOrEmpty(saveFile)) Options.Save(saveFile, whitelist);
        }

        /// <summary> 保存配置文件 </summary>
        /// <param name="path">配置文件路径</param>
        /// <param name="replace">是否替换同一路径下的原配置文件</param>
        /// <param name="whitelist">指定需要保存的配置项编号</param>
        public static void Save(string path = $"user://Config.json", params int[] whitelist)
        {
            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            if (file == null) throw new FileNotFoundException($"Failed to open config file at '{path}' (absolute path '{ProjectSettings.GlobalizePath(path)}'). Directory may not exist or is inaccessible.");
            JObject obj = new JObject();
            if (whitelist == null || whitelist.Length == 0)
                foreach (IOption option in Options.Registers)
                    obj.Merge((JObject)option.Save());
            else foreach (int id in whitelist)
                obj.Merge((JObject)Options.Registers[id].Save());
            file.StoreString(
                "// This is the config file for game.\n" +
                "// If you want to edit this file, please make sure to follow the JSON format and the comments for each option.\n" +
                "// Any invalid value will be automatically initialize config to default when loading.\n"
                + Regex.Replace(obj.ToString(Formatting.Indented), @"(""\w+""\s*:\s*)\[([\s\S]*?)\](,)?", match =>
                {
                    string prefix = match.Groups[1].Value; // 索引 
                    string inner = match.Groups[2].Value; // 原数据
                    string separator = match.Groups[3].Success ? match.Groups[3].Value : ""; // 分隔符
                    string result = Regex.Replace(inner, @"\s+", " ").Trim(); // 压缩
                    if (result.EndsWith(',')) result = result.Substring(0, result.Length - 1); // 修饰末尾分隔符
                    return $"{prefix}[{string.IsNullOrEmpty(result).IfElse(" ", $" {result} ")}]{separator}";
                }));
            //file.GetPathAbsolute().Print("Save file to {0}");
            file.Close();
            // todo: 添加一次备份
        }

        /// <summary> 加载配置文件 </summary>
        /// <param name="path">配置文件路径</param>
        /// <param name="whitelist">指定需要加载的配置项编号，若为<see langword="null"/>或为空数组则加载所有配置项</param>
        public static void Load(string path = $"user://Config.json", params int[] whitelist)
        {
            try
            {
                if (FileAccess.FileExists(path))
                    using (FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read))
                    {
                        JToken token = JToken.Parse(file.GetAsText(), new() { CommentHandling = CommentHandling.Ignore });
                        if (whitelist == null || whitelist.Length == 0)
                            foreach (IOption option in Options.Registers)
                                option.Load(token);
                        else foreach (int id in whitelist)
                            Options.Registers[id].Load(token);
                        // todo: 添加一次备份
                    }
                else throw new FileNotFoundException($"Config file not found at '{path}' (absolute path '{ProjectSettings.GlobalizePath(path)}').");
            }
            catch (Exception ex)
            {
                GD.PushWarning($"Failed to load config file, regenerating config file and using default values. \nException Message: {ex.Message}");
                Options.Reset();
                Options.Save(path, whitelist);
            }
        }

        private interface IOption
        {
            public int ID { get; }
            public JToken Save();
            public void Load(JToken token);
            public void Reset(bool update = false);
        }

        /// <summary> 配置项 </summary>
        /// <remarks>
        /// 可用于保存和加载单个配置项，可自定义约束以限制设置的有效范围或格式
        /// <para>实例化时，会自动注册到<see cref="Options"/>中统一管理</para>
        /// <para>作为字段使用时，请使用<see cref="Value"/>对其赋值，而不是字段本身，否则会导致字段被覆盖</para>
        /// <para>作为属性使用时，可以实现直接对属性赋值而不覆盖属性本身，在setter中实现只对<see cref="Value"/>的赋值即可</para>
        /// </remarks>
        /// <typeparam name="T">任何支持JSON序列化的类型</typeparam>
        public class Option<T> : IOption
        {
            /// <summary> 配置项名称 </summary>
            /// <remarks>
            /// 注册时不会检测，为了避免非必要的麻烦，请尽量避免与其他配置项出现相同名称
            /// </remarks>
            public string Name { get; set; }

            /// <summary> JSON索引 </summary>
            /// <remarks>
            /// 用于在JSON中存储和检索配置项的键，可以使用'.'来分级
            /// <para>需要保证在同一JSON文件下的唯一性，否则会导致配置项覆盖或数据丢失</para>
            /// </remarks>
            public string JsonKey { get; }

            /// <summary> 配置项的默认值 </summary>
            /// <remarks>
            /// 在实例化时会对<see cref="Value"/>赋予该默认值
            /// 在重置配置项时会对<see cref="Value"/>赋予该默认值
            /// </remarks>
            public T Default { get; }

            /// <summary> 配置项的当前值 </summary>
            /// <remarks>
            /// 实例化时为默认值
            /// 修改时会触发<see cref="Modify"/>方法（若不为null）进行约束检查
            /// </remarks>
            public T Value { get => field; set => this.Set(ref field, this.Modify == null ? value : this.Modify(this.Default, value)); }

            /// <summary> 注册索引 </summary>
            /// <remarks>
            /// 内部自动分配的唯一标识符
            /// </remarks>
            public int ID { get; }

            /// <summary> 修饰方法 </summary>
            /// <remarks>
            /// 可以在修改配置值时进行如限制大小、有效值判断等一系列约束
            /// <para>传参1：默认值</para>
            /// <para>传参2：更新值</para>
            /// <para>返回值：实际更新值</para>
            /// </remarks>
            public Func<T, T, T> Modify { get; }

            public Func<T, string> InfoFormat { get; set; }

            private static string ArrayFormat(T value)
            {
                int i = 0;
                ICollection vals = value as ICollection;
                string[] strs = new string[vals.Count];
                foreach (var val in vals) 
                    strs[i++] = val.ToString();
                return $"[{string.Join(", ", strs)}]";
            } 

            /// <summary>创建并注册一个配置项 </summary>
            /// <param name="name">配置项名称，也会作为JSON索引，用于检索，必须保证唯一性</param>
            /// <param name="defValue">配置项的默认值</param>
            /// <param name="modify">修饰方法，可以在赋值时进行限制大小、有效值判断等一系列约束<para>传参1：默认值，即<see cref="Default"/></para><para>传参2：更新值，即<see cref="Value"/></para><para>返回值：实际更新值</para></param>
            public Option(string name, T defValue, Func<T, T, T> modify = null) : this(name, name, defValue, modify) { }

            /// <summary>创建并注册一个配置项 </summary>
            /// <param name="name">配置项名称，用于检索，尽可能保证唯一性</param>
            /// <param name="jsonKey">JSON索引，用于检索，必须保证唯一性，可以使用'.'来分级</param>
            /// <param name="defValue">配置项的默认值</param>
            /// <param name="modify">修饰方法，可以在赋值时进行限制大小、有效值判断等一系列约束<para>传参1：默认值，即<see cref="Default"/></para><para>传参2：更新值，即<see cref="Value"/></para><para>返回值：实际更新值</para></param>
            public Option(string name, string jsonKey, T defValue, Func<T, T, T> modify = null)
            {
                int delay = Options.SaveDelay;
                Options.SaveDelay = 2;
                this.Name = name;
                this.JsonKey = jsonKey;
                this.Default = defValue;
                this.Value = defValue;
                this.Modify = modify;
                this.InfoFormat = (defValue is ICollection).IfElse(Option<T>.ArrayFormat, (v) => v.ToString());
                this.ID = Options.Registers.Count;
                Options.Registers.Add(this);
                Options.SaveDelay = delay;
            }

            public void Reset(bool update = false)
            {
                if (update) this.Value = this.Default;
                else
                {
                    int delay = Options.SaveDelay;
                    Options.SaveDelay = 2;
                    this.Value = this.Default;
                    Options.SaveDelay = delay;
                }
            }

            public JToken Save() => this.JsonKey.IfElse(this.JsonKey, this.Get(this.JsonKey.Split('.'), 0), new JObject() { [this.JsonKey] = this.Serialization(this.Value) });

            public void Load(JToken token)
            {
                bool notFound = false;
                string[] keys = this.JsonKey.Split('.');
                T def = this.Default;
                foreach (string key in keys)
                    if (notFound = (token = token?[key]) == null)
                        break;
                string name = this.Name.IfElse(this.JsonKey, this.Name, $"{this.Name}'->'{this.JsonKey}");
                string msg = notFound.IfElse($"Config '{name}' is not found", (typeof(T).IsEnum && !Enum.IsDefined(typeof(T), token.ToObject<T>())).IfElse($"Config '{name}' has an invalid value '{token}'", null));
                if (msg == null)
                {
                    this.Value = this.Deserialization(token);
                    //Debug.Print($"Config '{name}' loaded with value '{this.ToString()}' as <{typeof(T).Name}>.");
                }
                else
                {
                    this.Value = def;
                    msg += $", using default value '{this.ToString()} as <{typeof(T).Name}>'.";
                    GD.PushWarning(msg);
                    Debug.Print(msg);
                }
            }

            protected virtual JToken Get(string[] parts, int i = 0) => i < parts.Length ? new JObject() { [parts[i]] = this.Get(parts, i + 1) } : this.Serialization(this.Value);

            private void Set(ref T field, T value)
            {
                bool save = --Options.SaveDelay == 0 || Options.SaveDelay < 0 && (object)field != (object)value;
                field = value;
                if (save) Options.Save("res://Config.json"); // todo: res->user
            }

            protected virtual JToken Serialization(T value)
            {
                if (value is ICollection arr)
                {
                    JArray jarr = new JArray();
                    foreach (object item in arr) jarr.Add(item);
                    return jarr;
                }
                return new JValue(this.Value);
            }

            protected virtual T Deserialization(JToken token) => token.ToObject<T>();

            public override string ToString() => this.InfoFormat(this.Value);

            /// <summary> 快速取值 </summary>
            /// <remarks> 将配置项的值转换为其类型的值 </remarks>
            /// <param name="option">配置项</param>
            public static implicit operator T(Option<T> option) => option.Value;

            /// <summary> 快速赋值 </summary>
            /// <remarks> 注意：该操作会将数据转换为一个临时的配置项，请勿将其直接赋予字段形式的配置项 </remarks>
            /// <param name="value">赋值数据</param>
            public static implicit operator Option<T>(T value) => new("TempOption", value);
        }
    }
}
