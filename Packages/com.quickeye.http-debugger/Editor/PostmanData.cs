using System;
using System.Collections.Generic;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class PostmanData
    {
        public List<HDRequest> requests = new List<HDRequest>
        {
            new HDRequest
            {
            }
        };
    }
}