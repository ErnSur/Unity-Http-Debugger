using System;
using System.Collections.Generic;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class RequestCollection
    {
        public List<HDRequest> requests = new List<HDRequest>
        {
            new HDRequest
            {
            }
        };
    }
}