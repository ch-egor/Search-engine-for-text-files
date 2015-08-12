using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    interface IIndex : IDisposable
    {
        void Include(string filename);
        void Exclude(string filename);
        SearchResult[] Find(string query);
        WordIndexDisplay[] Export();

        string[] Files { get; }
        event EventHandler FilesChanged;
    }
}
