using System;
using System.Collections.Generic;

namespace QuickEye.WebTools
{
    [Serializable]
    public class Header
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

        public void Deconstruct(out string name, out string value)
        {
            name = this.name;
            value = this.value;
        }
    }
}