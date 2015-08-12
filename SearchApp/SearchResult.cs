using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    class SearchResult
    {
        public string Filename { get; private set; }
        public int[] Lines { get; private set; }

        public SearchResult(string filename, IEnumerable<int> lines)
        {
            this.Filename = filename;
            this.Lines = lines.ToArray();
        }
    }
}
