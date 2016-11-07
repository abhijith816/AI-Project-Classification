using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    class Label
    {
        public int LabelId { get; set; }
        public List<List<int>> Pixels { get; set; }

        public int LabelValue { get; set; }
    }
}
