using System;
using System.Collections.Generic;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class Header
    {
        public string name;
        public string value;
        public bool enabled = true;

        public Header(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public Header(KeyValuePair<string, string> pair)
        {
            name = pair.Key;
            value = pair.Value;
        }
    }
}