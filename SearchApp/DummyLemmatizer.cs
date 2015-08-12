using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    class DummyLemmatizer : ILemmatizer
    {
        public string GetLemma(string word) { return word; }
        public bool IsStopWord(string word) { return false; }
    }
}
