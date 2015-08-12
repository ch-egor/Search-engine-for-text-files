using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    class FileIndexDisplay
    {
        public string Filename { get; private set; }
        public string ShortFilename { get { return Path.GetFileName(this.Filename); } }
        public ObservableCollection<int> Lines { get; private set; }

        public FileIndexDisplay(string filename, IEnumerable<int> lines)
        {
            this.Filename = filename;
            this.Lines = new ObservableCollection<int>();
            foreach (int line in lines)
                this.Lines.Add(line);
        }
    }
}
