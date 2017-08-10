using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AroundMe.Core
{
    public class Photo
    {
            public string id { get; set; }
            public string owner { get; set; }
            public string secret { get; set; }
            public string server { get; set; }
            public int farm { get; set; }
            public string title { get; set; }
            public int ispublic { get; set; }
            public int isfriend { get; set; }
            public int isfamily { get; set; }
    }
}
