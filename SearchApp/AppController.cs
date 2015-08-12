using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    class AppController
    {
        private IIndex index;

        public string SearchRequest { get; set; }
        public bool ExactMatch { get; set; }
        public ObservableCollection<FileDescription> Files { get; private set; }
        public ObservableCollection<ResultFound> Results { get; private set; }
        public ObservableCollection<WordIndexDisplay> IndexItems { get; private set; }

        public AppController()
        {
            // initializing properties
            this.SearchRequest = String.Empty;
            this.ExactMatch = false;
            this.Files = new ObservableCollection<FileDescription>();
            this.Results = new ObservableCollection<ResultFound>();
            this.IndexItems = new ObservableCollection<WordIndexDisplay>();
            // preparing lemmatizer and index
            index = new Index(EnglishLemmatizer.Get());
            index.FilesChanged += index_FilesChanged;
            updateListOfFiles();
        }

        private void index_FilesChanged(object sender, EventArgs e) { updateListOfFiles(); }

        private void updateListOfFiles()
        {
            Files.Clear();
            foreach (string file in index.Files)
                Files.Add(new FileDescription(file));
            IndexItems.Clear();
            WordIndexDisplay[] exportedIndex = index.Export();
            if (exportedIndex != null)
                foreach (WordIndexDisplay wordIndexDisplay in exportedIndex)
                    IndexItems.Add(wordIndexDisplay);
        }

        public void IncludeFiles(IEnumerable<string> filenames)
        {
            foreach (string filename in filenames)
                index.Include(filename);
        }

        public void ExcludeFiles(IEnumerable<string> filenames)
        {
            foreach (string filename in filenames)
                index.Exclude(filename);
        }

        public void Find()
        {
            SearchResult[] results = index.Find(this.SearchRequest);
            string exactString = "";
            if (ExactMatch)
                exactString = SearchRequest;
            this.Results.Clear();
            if (results != null)
                foreach (SearchResult result in results)
                {
                    ResultFound resultFound = new ResultFound(result, exactString);
                    if (resultFound.Paragraphs.Count > 0)
                        this.Results.Add(resultFound);
                }
        }

        public void Save() { index.Dispose(); }
    }
}
