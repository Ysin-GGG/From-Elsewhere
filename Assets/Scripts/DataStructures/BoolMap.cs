using Godot;
using System.Text;

namespace LOI.DataStructures
{
    /// <summary> 01位图 </summary>
    public struct BoolMap
    {
        /// <summary> 宽度 </summary>
        public int Width { get; private set; }
        /// <summary> 高度 </summary>
        public int Height { get; private set; }
        /// <summary> 是否为有效图 </summary>
        public bool Valid => this.Width != 0 && this.Height != 0;
        /// <summary> 是否阻止修改 </summary>
        public bool Safe { get; set; }

        private Bit16[] data;

        /// <summary> 获取或设置图中的某个值 </summary>
        /// <param name="index"> 按从左往右、从上往下的顺序，编号从0开始 </param>
        public bool this[int index]
        {
            get => this.Valid ? this.data[index / Bit16.BitLength][index % Bit16.BitLength] : false;
            set { if (this.Valid && !this.Safe) this.data[index / Bit16.BitLength][index % Bit16.BitLength] = value; } 
        }

        /// <summary> 获取或设置图中的某个值 </summary>
        /// <param name="index"> （0, 0）为图的左上角 </param>
        public bool this[int x, int y]
        {
            get => this.Valid ? this[x + y * this.Width] : false;
            set { if (this.Valid && !this.Safe) this[x + y * this.Width] = value; }
        }

        /// <summary> 设置图中的若干个值 </summary>
        /// <param name="pairs"> 若干个坐标 </param>
        public bool this[params (int, int)[] pairs]
        {
            set { if (this.Valid && !this.Safe) foreach((int, int) pair in pairs) this[pair.Item1, pair.Item2] = value; }
        }

        /// <summary> 设置图中的若干个值 </summary>
        /// <param name="pairs"> 若干个坐标 </param>
        public bool this[params Vector2I[] pairs]
        {
            set { if (this.Valid && !this.Safe) foreach(Vector2I pair in pairs) this[pair.X, pair.Y] = value; }
        }

        public BoolMap(int width, int height, byte value, bool safe = false)
        {
            this.Safe = safe;
            this.Width = width;
            this.Height = height;
            this.data = null;
            if (this.Valid)
            {
                this.data = new Bit16[this.Width * this.Height / Bit16.BitLength + 1];
                for (int i = 0; i < this.data.Length; i++) this.data[i] = 0;
                for (int i = this.Width * this.Height; i-- > 0;)
                    this.data[i / Bit16.BitLength][i % Bit16.BitLength] = value != 0;
            }
        }

        public BoolMap(int width, int height, bool value, bool safe = false)
        {
            this.Safe = safe;
            this.Width = width;
            this.Height = height;
            this.data = null;
            if (this.Valid)
            {
                this.data = new Bit16[this.Width * this.Height / Bit16.BitLength + 1];
                for (int i = 0; i < this.data.Length; i++) this.data[i] = 0;
                for (int i = this.Width * this.Height; i-- > 0;)
                    this.data[i / Bit16.BitLength][i % Bit16.BitLength] = value;
            }
        }

        public BoolMap(int width, int height, bool safe = false, params Bit16[] data)
        {
            this.Safe = safe;
            this.Width = width;
            this.Height = height;
            this.data = null;
            if (this.Valid)
            {
                this.data = new Bit16[this.Width * this.Height / Bit16.BitLength + 1];
                for (int i = 0; i < this.data.Length; i++) this.data[i] = 0;
                for (int i = 0; i < data.Length; i++)
                    this.data[i] = i < data.Length ? data[i / Bit16.BitLength] : new Bit16();
            }
        }

        public BoolMap(byte[,] map, bool safe = false)
        {
            this.Safe = safe;
            this.Width = map?.GetLength(1) ?? 0;
            this.Height = map?.GetLength(0) ?? 0;
            this.data = null;
            if (this.Valid)
            {
                this.data = new Bit16[this.Width * this.Height / Bit16.BitLength + 1];
                for (int i = 0; i < this.data.Length; i++) this.data[i] = 0;
                for (int i = 0, k = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++, k++)
                        this.data[k / Bit16.BitLength][k % Bit16.BitLength] = map[i, j] != 0;
            }
        }

        public BoolMap(bool[,] map, bool safe = false)
        {
            this.Safe = safe;
            this.Width = map?.GetLength(1) ?? 0;
            this.Height = map?.GetLength(0) ?? 0;
            this.data = null;
            if (this.Valid)
            {
                this.data = new Bit16[this.Width * this.Height / Bit16.BitLength + 1];
                for (int i = 0; i < this.data.Length; i++) this.data[i] = 0;
                for (int i = 0, k = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++, k++)
                        this.data[k / Bit16.BitLength][k % Bit16.BitLength] = map[i, j];
            }
        }

        public static implicit operator bool[,](BoolMap value)
        {
            bool[,] map = new bool[value.Width, value.Height];
            int k = 0;
            for (int i = 0; i < value.Width; i++)
                for (int j = 0; j < value.Height; i++)
                    map[i, j] = value[k++];
            return map;
        }

        public static implicit operator byte[,](BoolMap value)
        {
            byte[,] map = new byte[value.Width, value.Height];
            int k = 0;
            for (int i = 0; i < value.Width; i++)
                for (int j = 0; j < value.Height; i++)
                    map[i, j] = (byte)(value[k++] ? 1 : 0);
            return map;
        }

        public static implicit operator BoolMap(bool[,] value) => new BoolMap(value, true);
        public static implicit operator BoolMap(byte[,] value) => new BoolMap(value, true);

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < this.Height; i++)
            {
                sb.Append("[ ");
                sb.Append(this[0, i] ? 1 : 0);
                for (int j = 1; j < this.Width; j++)
                    sb.Append($",{(this[j, i] ? 1 : 0)}");
                sb.Append(" ]\n");
            }
            return sb.ToString();
        }
    }
}
