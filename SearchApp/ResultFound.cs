using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SearchApp
{
    class ResultFound
    {
        public string Filename { get; private set; }
        public string ShortFilename { get { return Path.GetFileName(this.Filename); } }
        public ObservableCollection<Paragraph> Paragraphs { get; private set; }

        public ResultFound(SearchResult searchResult, string exactString = "")
        {
            this.Filename = searchResult.Filename;
            this.Paragraphs = new ObservableCollection<Paragraph>();
            int stringNumber = 0;
            using (StreamReader reader = new StreamReader(Filename))
                while (!reader.EndOfStream)
                {
                    stringNumber++;
                    string currentString = reader.ReadLine();
                    if (searchResult.Lines.Contains(stringNumber))
                        if (exactString == "" || currentString.Contains(exactString))
                            this.Paragraphs.Add(new Paragraph(stringNumber, currentString));
                }
        }
    }
}
