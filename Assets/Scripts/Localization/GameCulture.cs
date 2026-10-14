using System.Collections.Generic;
using System.Globalization;

namespace LOI.Localization
{
    public class GameCulture
    {
        public string Name { get; }
        public string Key { get; }
        public int ID { get; }

        public GameCulture(string key, string name)
        {
            this.Name = name;
            this.Key = key;
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        }

        public override string ToString()
        {
            return $"{Key}[{Name}]";
        }
    }
}