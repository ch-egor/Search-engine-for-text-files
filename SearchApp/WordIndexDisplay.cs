using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    class WordIndexDisplay
    {
        public string Word { get; private set; }
        public ObservableCollection<FileIndexDisplay> Files { get; private set; }

        public WordIndexDisplay(string word, IEnumerable<FileIndexDisplay> files)
        {
            this.Word = word;
            this.Files = new ObservableCollection<FileIndexDisplay>();
            foreach (FileIndexDisplay file in files)
                this.Files.Add(file);
        }
    }
}
