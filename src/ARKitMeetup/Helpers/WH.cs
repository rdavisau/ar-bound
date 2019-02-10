using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ARKitMeetup.Helpers
{
    public static class WH
    {
        private static Dictionary<string, object> items = new Dictionary<string, object>();

        public static T GetOrSet<T>(string key, T set)
        {
            object item;
            if (items.TryGetValue(key, out item))
                return (T)item;

            items[key] = set;

            return set;
        }
    }
}