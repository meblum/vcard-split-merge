using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCF
{
    public struct VCFProgressData
    {
        public VCFProgressData(int number, string name)
        {
            Number = number; Name = name;
        }
        public int Number { get; }
        public string Name { get; }
    }
}
