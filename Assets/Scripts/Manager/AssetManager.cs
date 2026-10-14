using Godot;
using LOI.DataStructures;
using LOI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FileAccess = Godot.FileAccess;

namespace LOI.Manager
{
    /// <summary> 资源管理器 </summary>
    public partial class AssetManager : LOIManager
    {
        public static AssetManager Instance { get; private set; }
        public static string CulturePath => @"Assets\Languages";
        public static string[] VaildCulture { get; } = { "zh-Hans", "zh-Hant", "en-US" };
        private static Dictionary<string, string> RawText => AssetManager.Culture.ContainsKey(Options.Language) ? AssetManager.Culture[Options.Language] : new Dictionary<string, string>();
        private static Dictionary<string, Dictionary<string, string>> Culture;
        private static HashSet<string> visited = new HashSet<string>();

        private ObjPool<AudioStreamPlayer2D> sounds;

        [Export]
        public Font CombatTextFont { get; private set; }

        [Export]
        public Texture2D CombatTextIcon { get; private set; }

        [Export]
        private Node AudioParent { get; set; }

        protected override void EnterTree()
        {
            this.AddChild(this.AudioParent = new Node());
            this.sounds = new ObjPool<AudioStreamPlayer2D>(this.NewAudio, 64, ObjPool<AudioStreamPlayer2D>.NodeComparer);
            AssetManager.Instance = this;
            AssetManager.Culture = new Dictionary<string, Dictionary<string, string>>();

            foreach (string locale in AssetManager.VaildCulture)
            {
                //long before = GC.GetTotalMemory(true);
                AssetManager.LoadCulture(new CultureInfo(locale)); // AssetManager.LoadJson(new CultureInfo("zh-CN");
                //Debug.Print(GC.GetTotalMemory(true) - before + "");
            }
        }

        public static bool CheckCulture(CultureInfo culture)
        {
            string path = $"user://Localization/{culture.Name}.bin";
            return FileAccess.FileExists(path);
        }

        /// <summary> 读取二进制流的本地化文件<para>读取封装为bin文件的对应地区Json文件</para> </summary>
        public static Dictionary<string, string> LoadCultureBin(CultureInfo culture)
        {
            string path = $"res://Assets/Languages/{culture.Name}.bin";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            using (FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read))
            {
                byte[] bin = file.GetBuffer((long)file.GetLength());
                JsonTextReader reader = new JsonTextReader(new StringReader(Encoding.UTF8.GetString(bin)));
                JToken token = JToken.ReadFrom(reader);
                AssetManager.AddPair(dict, Path.GetFileNameWithoutExtension(path), token);
            }
            if (!AssetManager.Culture.Values.Contains(dict)) AssetManager.Culture.Add(culture.Name, dict);
            else AssetManager.Culture[culture.Name] = dict;
            return dict;
        }


        /// <summary> 读取本地化文件<para>读取对应地区文件夹内的所有Json文件</para> </summary>
        [Obsolete("This method is obsolete. Call AssetManager.LoadBinText() instead. It perfer to load in Godot.")]
        public static Dictionary<string, string> LoadCulture(CultureInfo culture)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), AssetManager.CulturePath, culture.Name);
            if (!Directory.Exists(path)) return null;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>(30);
            DirectoryInfo curDir = new DirectoryInfo(path);
            while (true)
            {
                FileInfo[] files = curDir.GetFiles("*.json");
                DirectoryInfo[] dirs = curDir.GetDirectories();
                foreach (DirectoryInfo dir in dirs) queue.Enqueue(dir);
                foreach (FileInfo file in files)
                {
                    StreamReader stream = file.OpenText();
                    JsonTextReader reader = new JsonTextReader(stream);
                    JToken token = JToken.ReadFrom(reader);
                    AssetManager.AddPair(dict, Path.GetFileNameWithoutExtension(file.Name), token);
                }
                if (queue.Count > 0) curDir = queue.Dequeue();
                else break;
            }
            if (!AssetManager.Culture.Values.Contains(dict)) AssetManager.Culture.Add(culture.Name, dict);
            else AssetManager.Culture[culture.Name] = dict;
            return dict;
        }

        private static void AddPair(Dictionary<string, string> dict, string key, JToken jo)
        {
            if (jo.HasValues)
            {
                JToken[] tjo = jo.ToArray();
                foreach (JToken child in tjo)
                    AddPair(dict, key, child);
            }
            else dict.Add($"{key}.{jo.Parent.Path}".Trim('.'), jo.ToString());
        }

        /// <summary> 检索与指定键关联的文本，并递归解析其中嵌入的所有占位符 </summary>
        /// <remarks>如果与键关联的文本包含格式为 '{$...}' 的占位符，则这些占位符将被递归使用该方法进行解析</remarks>
        /// <param name="key">用于查找关联文本的键。若其不存在关联文本，方法会尝试查找一个包含该指定键作为子字符串的键</param>
        /// <returns><para>与指定键关联的已解析文本</para>否则返回原始键</returns>
        public static string GetText(string key)
        {
            if (AssetManager.visited.Contains(key)) throw new StackOverflowException("Recursive placeholder detected: " + key);
            if (AssetManager.RawText == null || string.IsNullOrEmpty(key)) return key;
            if (AssetManager.RawText.ContainsKey(key))
            {
                string text = AssetManager.RawText[key];
                int startIndex = 0, endIndex = 0;
                while ((startIndex = text.IndexOf("{$", startIndex)) >= 0 && (endIndex = text.IndexOf("}", startIndex)) > 0) // 待优化：括号匹配算法
                {
                    string nkey = text.Substring(startIndex, endIndex - startIndex + 1);
                    AssetManager.visited.Add(nkey); // 添加到缓冲区防止递归
                    string nvalue = AssetManager.GetText(nkey.Substring(2, nkey.Length - 3)); //替换时产生的新args将会保留
                    AssetManager.visited.Remove(nkey); // 从缓冲区移除以允许后续解析同一占位符（如果文本中有多个相同的占位符）
                    text = text.Replace(nkey, nvalue); // 修正调用：以避免反复执行替换
                    startIndex = endIndex = 0; // 重置索引以重新扫描文本，避免替换后的文本中出现新的占位符被遗漏
                }
                return AssetManager.RawText[key] = text;
            }
            // 若该key不存在，则放宽限制，查看是否有包含这个key的key
            foreach (string tkey in AssetManager.RawText.Keys)
                if (tkey.Contains(key)) return AssetManager.GetText(tkey);
            return key;
        }

        /// <summary> 检索与指定键关联的文本，并递归解析其中嵌入的所有占位符 </summary>
        /// <remarks>如果与键关联的文本包含格式为 '{$...}' 的占位符，则这些占位符将被递归使用该方法进行解析</remarks>
        /// <param name="key">用于查找关联文本的键。若其不存在关联文本，方法会尝试查找一个包含该指定键作为子字符串的键</param>
        /// <param name="args"> 格式化输出的参数，在文本完全解析后使用 </param>
        /// <returns><para>与指定键关联的已解析文本</para>否则返回原始键</returns>
        public static string GetText(string key, params object[] args) => string.Format(AssetManager.GetText(key), args);

        public static bool HasText(string key) => AssetManager.RawText.ContainsKey(key);

        public static int PlaySound(AudioStream sound, Vector2 position, float volume = 1f)
        {
            AudioStreamPlayer2D player = AssetManager.Instance.sounds.Request();
            player.Stream = sound;
            player.VolumeDb = volume.Clamp(-80.0f, 24.0f);
            player.Position = position;
            player.Play();
            return player.ZIndex;
        }

        private AudioStreamPlayer2D NewAudio(int id)
        {
            AudioStreamPlayer2D player = new AudioStreamPlayer2D();
            player.Name = "SoundPlayer " + id;
            player.Autoplay = false;
            player.AreaMask = 0;
            //player.Finished += () => pool.Release(player);
            this.AudioParent.AddChild(player);
            return player;
        }

        /// <summary> 音频资源最大滞留时长 </summary>
        public static long AudioUnusedDuration { get; set; }
        public static AudioListener2D Listener;
        public static Node MusicPlayer { get; private set; }
        public static Node SFXPlayer { get; private set; }
        public static Node AmbientPlayer { get; private set; }
        public static Node ConstantPlayer { get; private set; }
        private static ObjPool<AudioStreamPlayer2D> Music;
        private static ObjPool<AudioStreamPlayer2D> SFX;
        private static ObjPool<AudioStreamPlayer2D> Ambient;
        private static Dictionary<AudioStream, AudioStreamPlayer2D> ConstantAudioAsset;
        private static Dictionary<string, (long, AudioStream)> AudioAsset;
        private static LinkedList<(string, long)> AudioTimeLeft;

        /// <summary> 正在播放的音乐 </summary>
        private static AudioStreamPlayer2D music0;
        /// <summary> 正在替换的音乐 </summary>
        private static AudioStreamPlayer2D music1;
        /// <summary> 即将替换的音乐 </summary>
        private static AudioStreamPlayer2D music2;
        private static float musicProcess0 = 1f;
        private static float musicProcess1 = 0f;
        private static float musicVolume0 = 0f;
        private static float musicVolume1 = 0f;
        private static float musicFade = .005f;

        protected override void Initialization()
        {
            AssetManager.AudioUnusedDuration = 600_000_000; // 600s
            AssetManager.ConstantAudioAsset = new Dictionary<AudioStream, AudioStreamPlayer2D>();
            AssetManager.AudioAsset = new Dictionary<string, (long, AudioStream)>();
            AssetManager.AudioTimeLeft = new LinkedList<(string, long)>();

            /*
            (AssetManager.Listener = new AudioListener2D() { Name = nameof(AssetManager.Listener) }).Reparent(this);
            (AssetManager.MusicPlayer = new Node() { Name = nameof(AssetManager.MusicPlayer) }).Reparent(this);
            (AssetManager.SFXPlayer = new Node() { Name = nameof(AssetManager.SFXPlayer) }).Reparent(this);
            (AssetManager.AmbientPlayer = new Node() { Name = nameof(AssetManager.AmbientPlayer) }).Reparent(this);
            (AssetManager.ConstantPlayer = new Node() { Name = nameof(AssetManager.ConstantPlayer) }).Reparent(this);
            AssetManager.Music = new ObjPool<AudioStreamPlayer2D>((i) => AssetManager.AddSource(0), 8);
            AssetManager.SFX = new ObjPool<AudioStreamPlayer2D>((i) => AssetManager.AddSource(1), 64);
            AssetManager.Ambient = new ObjPool<AudioStreamPlayer2D>((i) => AssetManager.AddSource(2), 16);
            AssetManager.Music.OnRelease += (snd) => { if (snd.Playing) snd.Disable().Stop(); };
            */
        }

        protected override void GameUpdate()
        {
            // 音乐过渡切换
            if (AssetManager.music0 != null) AssetManager.music0.VolumeLinear = AssetManager.musicVolume0.Lerp(AssetManager.musicProcess0.Pow(2), 0f);
            if (AssetManager.music1 != null) AssetManager.music1.VolumeLinear = AssetManager.musicVolume1.Lerp(2 * AssetManager.musicProcess1 - AssetManager.musicProcess1.Pow(2), 0f);
            if (AssetManager.music2 != null)
            {
                if (AssetManager.musicProcess1 > .1f)
                {
                    //AssetManager.musicProcess0 = 1f.Min(AssetManager.musicProcess0 + AssetManager.musicFade);
                    AssetManager.musicProcess1 = 0f.Max(AssetManager.musicProcess1 - AssetManager.musicFade);
                }
                else
                {
                    if (AssetManager.music1 != null) AssetManager.Music.Release(AssetManager.music1);
                    AssetManager.music1 = AssetManager.music2;
                    AssetManager.music2 = null;
                    AssetManager.musicVolume1 = AssetManager.music1?.VolumeLinear ?? 0f;
                    if (AssetManager.music1 != null)
                    {
                        AssetManager.music1.VolumeLinear = 0f;
                        AssetManager.music1.Play();
                    }
                    //AssetManager.musicProcess0 = 1f;
                    AssetManager.musicProcess1 = 0f;
                }
            }
            else if (AssetManager.music1 != null && (AssetManager.music0 == null || AssetManager.music0.Stream != AssetManager.music1.Stream))
            {
                if (AssetManager.musicProcess0 > .1f || AssetManager.musicProcess1 < 1f)
                {
                    AssetManager.musicProcess0 = 0f.Max(AssetManager.musicProcess0 - AssetManager.musicFade);
                    AssetManager.musicProcess1 = 1f.Min(AssetManager.musicProcess1 + AssetManager.musicFade);
                }
                else
                {
                    if (AssetManager.music0 != null) AssetManager.Music.Release(AssetManager.music0);
                    AssetManager.music0 = AssetManager.music1;
                    AssetManager.music1 = null;
                    AssetManager.musicVolume0 = AssetManager.musicVolume1;
                    AssetManager.musicVolume1 = 0f;
                    AssetManager.musicProcess0 = 1f;
                    AssetManager.musicProcess1 = 0f;
                }
            }

            // 每秒释放一次长时间未用的音频资源
            if (GameManager.GameTime % 60 == 0)
            {
                int count = AssetManager.AudioTimeLeft.Count;
                long time = DateTime.Now.Ticks;
                LinkedListNode<(string, long)> node = AssetManager.AudioTimeLeft.First;
                while (node != null)
                {
                    string key = node.Value.Item1;
                    long value = node.Value.Item2;
                    if (time - value > AssetManager.AudioUnusedDuration)
                    {
                        if (AssetManager.AudioAsset[key].Item1 == value) // 释放
                        {
                            AssetManager.AudioAsset.Remove(key);
                            AssetManager.AudioTimeLeft.Remove(node);
                        }
                        else node.Value = (key, AssetManager.AudioAsset[key].Item1);
                    }
                    node = node.Next;
                }
            }
        }

        private static AudioStreamPlayer2D AddSource(int type)
        {
            AudioStreamPlayer2D source = null;
            switch (type)
            {
                case 0:
                    source = new AudioStreamPlayer2D() { Name = nameof(AssetManager.MusicPlayer) };
                    source.Finished += () => AssetManager.Music.Release(source);
                    source.Reparent(AssetManager.Instance);
                    break;
                case 1:
                    source = new AudioStreamPlayer2D() { Name = nameof(AssetManager.SFXPlayer) };
                    source.Finished += () => AssetManager.SFX.Release(source);
                    source.Reparent(AssetManager.Instance);
                    break;
                case 2:
                    source = new AudioStreamPlayer2D() { Name = nameof(AssetManager.AmbientPlayer) };
                    source.Finished += () => AssetManager.Ambient.Release(source);
                    source.Reparent(AssetManager.Instance);
                    break;
            }
            return source.Disable();
        }

        /// <summary> 播放音频（有生命周期） </summary>
        /// <param name="stream"> 音频资源 </param>
        /// <param name="volume"> 音量 </param>
        /// <param name="pitch"> 音调 </param>
        /// <param name="pan"> 声道占比（负数为左声道，正数为右声道） </param>
        public static AudioStreamPlayer2D PlaySound(AudioStream stream, float volume = 1f, float pitch = 1f, float pan = 1f)
        {
            AudioStreamPlayer2D source = AssetManager.SFX.Request();
            source.Enable();
            source.Stream = stream;
            source.VolumeLinear = volume * Options.SFXVolume * Options.MasterVolume;
            source.PitchScale = pitch;
            source.PanningStrength = pan;
            source.Play();
            return source;
        }

        /// <summary> 读取并播放音频（有生命周期） </summary>
        /// <param name="file"> 音频文件 </param>
        /// <param name="volume"> 音量 </param>
        /// <param name="pitch"> 音调 </param>
        /// <param name="pan"> 声道占比（负数为左声道，正数为右声道） </param>
        public static AudioStreamPlayer2D PlaySound(string file, float volume = 1f, float pitch = 1f, float pan = 1f)
        {
            long time = DateTime.Now.Ticks;
            if (!AssetManager.AudioAsset.ContainsKey(file))
            {
                AssetManager.AudioTimeLeft.AddLast((file, time));
                AssetManager.AudioAsset.Add(file, (time, GD.Load<AudioStream>(file)));// LKAsset.Sound.GetAudioClip(file)));
            }
            AssetManager.AudioAsset[file] = (time, AssetManager.AudioAsset[file].Item2);
            return AssetManager.PlaySound(AssetManager.AudioAsset[file].Item2, volume, pitch, pan);
        }

        /// <summary> 播放独立音频（不会重复播放） </summary>
        /// <param name="stream"> 音频资源 </param>
        /// <param name="volume"> 音量 </param>
        /// <param name="pitch"> 音调 </param>
        /// <param name="pan"> 声道占比（负数为左声道，正数为右声道） </param>
        public static bool PlayConstSound(AudioStream stream, float volume = 1f, float pitch = 1f, float pan = 1f, bool replay = false)
        {
            AudioStreamPlayer2D source = null;
            if (!AssetManager.ConstantAudioAsset.ContainsKey(stream)) // 初次播放，记入
            {
                source = new AudioStreamPlayer2D() { Name = nameof(AssetManager.AmbientPlayer) + AssetManager.ConstantPlayer.GetChildCount() };
                source.Finished += () => AssetManager.Ambient.Release(source);
                source.Reparent(AssetManager.Instance);
                source.Stream = stream;
                AssetManager.ConstantPlayer.AddChild(source);
                AssetManager.ConstantAudioAsset.Add(stream, source);
            }
            else source = AssetManager.ConstantAudioAsset[stream];
            if (source.Playing && !replay) return false;
            source.Enable();
            source.Stream = stream;
            source.VolumeLinear = volume * Options.MasterVolume;
            source.PitchScale = pitch;
            source.PanningStrength = pan;
            source.Play();
            return true;
        }

        public static void PlayMusic(AudioStream stream, float volume = 1f, float pitch = 1f, float pan = 1f, bool replay = false, bool loop = true)
        {
            AudioStreamPlayer2D source = AssetManager.Music.Request();
            source.Enable();
            source.Stream = stream;
            source.VolumeLinear = volume * Options.SFXVolume * Options.MasterVolume;
            source.PitchScale = pitch;
            source.PanningStrength = pan;
            source.Play();
            //source.loop = loop;
            if (AssetManager.music2 != null) AssetManager.Music.Release(AssetManager.music2);
            AssetManager.music2 = source;
        }
    }
}