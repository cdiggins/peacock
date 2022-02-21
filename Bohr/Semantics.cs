using System.Collections.Generic;

namespace Bohr
{
    public static class Semantics
    {
        public static List<(string, string)> TwoWayConverters = new()
        {
            ("Point 2D", "Vector 2D"),
            ("Point 2D", "Size 2D"),
            ("Size 2D", "Vector 2D"),
            ("Line", "Interval"),
            ("Line", "Rect"),
            ("Angle", "Number"),
            ("Integer", "Number"),
            ("Integer", "Angle"),
            ("Boolean", "Number"),
            ("Boolean", "Integer"),
        };

        public static Dictionary<string, HashSet<string>> CanConvert = new();

        static Semantics()
        {
            foreach (var kv in TwoWayConverters)
            {
                if (!CanConvert.ContainsKey(kv.Item1))
                    CanConvert.Add(kv.Item1, new());
                CanConvert[kv.Item1].Add(kv.Item2);

                if (!CanConvert.ContainsKey(kv.Item2))
                    CanConvert.Add(kv.Item2, new());
                CanConvert[kv.Item2].Add(kv.Item1);
            }
        }

        public static bool CanConnect(string fromType, string toType)
        {
            if (fromType == "Any" || toType == "Any")
                return true;
            if (toType == "Array")
                return true;
            if (fromType == toType)
                return true;
            if (CanConvert.TryGetValue(fromType, out var hashSet) && hashSet.Contains(toType))
                return true;
            return false;
        }

        public static bool CanConnect(Socket source, Socket dest)
            => source.LeftOrRight ^ dest.LeftOrRight && CanConnect(source.Type, dest.Type);
    }
}
