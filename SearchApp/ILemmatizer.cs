using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    interface ILemmatizer
    {
        string GetLemma(string word);
        bool IsStopWord(string word);
    }
}
