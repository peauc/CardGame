using System;
using CardGame.Protocol;

namespace Client
{
    public static class Utils
    {
        public static Boolean HasArgument(String line) {
            return (line.Contains(" ") && !(line.StartsWith(" ") == true));
        }

        public static String GetArgument(String line) {
            int pos;

            pos = line.IndexOf(" ") + 1;
            if (pos == -1) {
                return (" ");
            }
            return (line.Substring(pos));
        }

        public static Boolean HasEnoughArgument(String str, int i) {
            return (str.Split(" ").Length == i);
        }

        internal static string HandToString(Hand hand)
        {
            throw new NotImplementedException();
        }
    }
}
