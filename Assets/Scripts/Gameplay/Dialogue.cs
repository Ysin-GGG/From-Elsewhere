using LOI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LOI.Gameplay
{
    /// <summary> 对白 </summary>
    public class Dialogue
    {
        /// <summary> 最大可用对话选项数量 </summary>
        public const int MaxOptions = 4;
        /// <summary> 文本条目数量 </summary>
        public int TextCount => this.texts.Count;
        /// <summary> 可用对话选项数量 </summary>
        public int OptionCount => this.options.Count;
        /// <summary> 当前播放的条目编号 </summary>
        public int CurrentNumber { get; private set; } = -1;

        private List<(string, string)> texts;
        private List<(string, Dialogue, Action<int>)> options;

        public Dialogue(ICollection<(string, string)> texts = null, ICollection<(string, Dialogue, Action<int>)> options = null)
        {
            this.texts = new List<(string, string)>(texts ?? Array.Empty<(string, string)>());
            this.options = new List<(string, Dialogue, Action<int>)>(Dialogue.MaxOptions);
            foreach (var option in options)
            {
                this.options.Add(option);
                if (this.OptionCount >= Dialogue.MaxOptions) break;
            }
        }

        /// <summary> 添加一条对话条目 </summary>
        /// <param name="speaker">该条目的发言者，不能为null</param>
        /// <param name="text">该条目的对话文本，不能为null</param>
        /// <exception cref="ArgumentNullException">当<paramref name="speaker"/>或<paramref name="text"/>任一为null时抛出异常</exception>
        public void AddText(string speaker, string text) 
        {
            if (speaker == null) throw new ArgumentNullException(nameof(speaker), "Speaker cannot be null.");
            if (text == null) throw new ArgumentNullException(nameof(text), "Text cannot be null.");
            this.texts.Add((speaker, text)); 
        }

        /// <summary> 添加一个对话选项 </summary>
        /// <param name="option">选项显示的文本，不能为nul</param>
        /// <param name="next">下一段对白，为null时则结束对话</param>
        /// <param name="evt">点击该选项之后的后续事件</param>
        /// <exception cref="ArgumentNullException">当<paramref name="option"/>为null时抛出异常</exception>
        public void AddOption(string option, Dialogue next = null, Action<int> evt = null)
        {
            if (this.OptionCount >= Dialogue.MaxOptions) throw new InvalidOperationException($"Cannot add more than {Dialogue.MaxOptions} options.");
            if (option == null) throw new ArgumentNullException(nameof(option), "Option cannot be null.");
            this.options.Add((option, next, evt));
        }

        /// <summary> 按编号播放对话条目 </summary>
        /// <remarks> 对话条目编号从0开始计算（0-based），若指定编号超过文本条目数量，则触发选项显示或结束对话 </remarks>
        /// <param name="number">对话条目编号。若为负数，则基于当前编号播放下一个条目；否则直接播放指定编号的条目</param>
        /// <returns> <para>false：结束对话或显示选项；</para>true：对话未播放完毕 </returns>
        public bool ApplyText(int number = -1)
        {
            if (number < 0) this.CurrentNumber = (this.CurrentNumber + 1).Max(0);
            else this.CurrentNumber = number.Min(this.TextCount);
            if (this.CurrentNumber < this.TextCount)
            {
                var (speaker, text) = this.texts[this.CurrentNumber];
                this.OnApplyText(this.CurrentNumber, ref speaker, ref text);
                // todo: UIManager.Instance.ShowOptions(this.texts[this.CurrentNumber]);
                return true;
            }
            else
            {
                this.CurrentNumber = -1;
                string[] options = new string[this.OptionCount];
                for (int i = 0; i < options.Length; i++) options[i] = this.options[i].Item1;
                this.OnApplyOptions(ref options);
                // todo: UIManager.Instance.ShowDialogue(this.options);
                return false;
            }
        }

        /// <summary> 对应选项的触发处理 </summary>
        /// <remarks> 选项编号从0开始计算（0-based），不能超过该对话可用选项数</remarks>
        /// <param name="number">选项编号，不能超过该对话可用选项数</param>
        /// <exception cref="ArgumentOutOfRangeException">当<paramref name="number"/>为负数或大于可用选项数时抛出异常</exception>
        public void ApplyOption(int number)
        {
            if (number < 0 || number >= this.OptionCount) throw new ArgumentOutOfRangeException(nameof(number), "Option number is out of range.");
            var (option, next, evt) = this.options[number];
            this.OnApplyOption(number, ref next, ref evt);
            evt?.Invoke(number);
            next?.ApplyText();
        }

        public virtual void OnApplyText(int number, ref string speaker, ref string text) { }

        public virtual void OnApplyOption(int number, ref Dialogue next, ref Action<int> evt) { }

        public virtual void OnApplyOptions(ref string[] options) { }
    }
}