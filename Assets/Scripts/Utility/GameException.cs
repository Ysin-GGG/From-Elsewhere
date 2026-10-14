using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.Utility
{
    public static class GameException
    {
        private static Dictionary<string, Exception> exceptions = new Dictionary<string, Exception>();

        public static void Error(this string code)
        {
            throw GameException.exceptions[code];
        }

        public static void Warning(this string code)
        {
            GD.PushWarning(GameException.exceptions[code].Message);
        }

        public static void AddException(string code, Exception exception)
        {
            if (!GameException.exceptions.ContainsKey(code)) GameException.exceptions.Add(code, exception);
            else GameException.exceptions[code] = exception;
        }
    }
}
