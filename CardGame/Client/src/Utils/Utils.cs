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

        public static string HandToString(Hand hand)
        {
            int i = 0;
            String ret = "";

            foreach (Card c in hand.Card)
            {
                ret += "[SERVER] " + i++ + " : " + c.Value.ToString() + " of " + c.Type.ToString() + "\n";
            }
            return (ret);
        }
    }
}
