using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    [DataContract]
    class Index : IIndex
    {
        public ILemmatizer Lemmatizer { get; set; }

        [DataMember]
        private int nextFileNumber = 0;

        private string filename;

        [DataMember]
        private Dictionary<int, string> files = new Dictionary<int, string>();

        public string[] Files { get { return files.Values.ToArray(); } }

        public event EventHandler FilesChanged;

        [DataMember]
        private SortedDictionary<string, SortedDictionary<int, SortedSet<int>>> words
            = new SortedDictionary<string, SortedDictionary<int, SortedSet<int>>>();

        [DataMember]
        private char[] wordSeparators = { ' ', '\t', '.', ',', ':', ';', '?', 
                                            '!', '\'', '"', '(', ')', '-', '…', 
                                            '<', '>', '[', ']', '{', '}', '#', 
                                            '$', '*', '/', '\\', '%', '=', '@', '&', 
                                            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public SortedDictionary<int, SortedSet<int>> FindWord(string word)
        {
            string processedWord = word.Trim().ToLower();
            string lemma = Lemmatizer.GetLemma(processedWord);
            if (words.ContainsKey(lemma))
                return words[lemma];
            else
                return null;
        }

        private SortedDictionary<int, SortedSet<int>> FindWord
            (string word, SortedDictionary<int, SortedSet<int>> previousResults)
        {
            SortedDictionary<int, SortedSet<int>> newResults = FindWord(word);
            if (previousResults != null)
            {
                SortedDictionary<int, SortedSet<int>> currentResults = newResults;
                newResults = new SortedDictionary<int, SortedSet<int>>();
                foreach (int fileNumber in previousResults.Keys)
                {
                    if (currentResults.ContainsKey(fileNumber))
                    {
                        newResults.Add(fileNumber, new SortedSet<int>());
                        foreach (int lineNumber in previousResults[fileNumber])
                            if (currentResults[fileNumber].Contains(lineNumber))
                                newResults[fileNumber].Add(lineNumber);
                    }
                }
            }
            return newResults;
        }

        private SearchResult[] ConvertResults(SortedDictionary<int, SortedSet<int>> resultsDict)
        {
            if (resultsDict == null)
                return null;
            List<SearchResult> searchResults = new List<SearchResult>();
            foreach (int key in resultsDict.Keys)
                if (resultsDict[key].Count > 0)
                {
                    string filename = files[key];
                    SortedSet<int> lines = resultsDict[key];
                    SearchResult searchResult = new SearchResult(filename, lines);
                    searchResults.Add(searchResult);
                }
            return searchResults.ToArray();
        }

        public SearchResult[] Find(string query)
        {
            string processedQuery = query.Trim().ToLower();
            string[] words = processedQuery.Split(new char[] { ' ' });
            SortedDictionary<int, SortedSet<int>> results = null;
            foreach (string word in words)
                if (!Lemmatizer.IsStopWord(word))
                    results = FindWord(word, results);
            return ConvertResults(results);
        }

        public void Include(string filename)
        {
            using (StreamReader input = new StreamReader(filename))
            {
                int currentStringNumber = 1;
                while (!input.EndOfStream)
                {
                    string currentString = input.ReadLine();
                    string[] words = currentString.Split(wordSeparators);
                    foreach (string word in words)
                    {
                        string processedWord = word.ToLower();
                        if (!String.IsNullOrWhiteSpace(processedWord))
                            AddItem(processedWord, filename, currentStringNumber);
                    }
                    currentStringNumber++;
                }
            }
            FilesChanged(this, new EventArgs());
        }

        public void Exclude(string filename)
        {
            if (!GetFileNumber(filename).HasValue)
                return;
            int fileNumber = GetFileNumber(filename).Value;
            foreach (SortedDictionary<int, SortedSet<int>> word in words.Values)
                word.Remove(fileNumber);
            files.Remove(fileNumber);
            List<string> removeWords = new List<string>();
            foreach (string word in words.Keys)
                if (words[word].Count == 0)
                    removeWords.Add(word);
            foreach (string word in removeWords)
                words.Remove(word);
            FilesChanged(this, new EventArgs());
        }

        private int? GetFileNumber(string file)
        {
            if (files.ContainsValue(file))
                return files.First(x => x.Value == file).Key;
            else
                return null;
        }

        private void AssignFileNumber(string file)
        {
            files.Add(nextFileNumber++, file);
        }

        private void AddItem(string word, string file, int line)
        {
            // finding lemma and checking for stopword
            string processedWord = word.Trim().ToLower();
            string lemma = Lemmatizer.GetLemma(processedWord);
            if (Lemmatizer.IsStopWord(lemma))
                return;
            // checking if lemma is already in index
            if (!words.ContainsKey(lemma))
                words.Add(lemma, new SortedDictionary<int, SortedSet<int>>());
            // looking up file number in dictionary
            SortedDictionary<int, SortedSet<int>> currentWord = words[lemma];
            if (!GetFileNumber(file).HasValue)
                AssignFileNumber(file);
            int fileNumber = GetFileNumber(file).Value;
            // checking if file is already under current word
            if (!currentWord.ContainsKey(fileNumber))
                currentWord.Add(fileNumber, new SortedSet<int>());
            // adding line number to locations
            SortedSet<int> currentFile = currentWord[fileNumber];
            currentFile.Add(line);
        }

        public void SaveToXml(string filename)
        {
            File.Delete(filename);
            using (FileStream stream = File.OpenWrite(filename))
            {
                DataContractSerializer serializer 
                    = new DataContractSerializer(typeof(Index));
                serializer.WriteObject(stream, this);
            }
        }

        public WordIndexDisplay[] Export()
        {
            List<WordIndexDisplay> exportedIndex = new List<WordIndexDisplay>();
            foreach (string word in words.Keys)
            {
                List<FileIndexDisplay> filesIndex = new List<FileIndexDisplay>();
                SortedDictionary<int, SortedSet<int>> files = this.words[word];
                foreach (int fileNumber in files.Keys)
                {
                    string filename = this.files[fileNumber];
                    SortedSet<int> lines = files[fileNumber];
                    FileIndexDisplay fileIndexDisplay = new FileIndexDisplay(filename, lines);
                    filesIndex.Add(fileIndexDisplay);
                }
                WordIndexDisplay wordIndexDisplay = new WordIndexDisplay(word, filesIndex);
                exportedIndex.Add(wordIndexDisplay);
            }
            return exportedIndex.ToArray();
        }

        public void SaveToJson(string filename)
        {
            File.Delete(filename);
            using (FileStream stream = File.OpenWrite(filename))
            {
                DataContractJsonSerializerSettings settings 
                    = new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;
                DataContractJsonSerializer serializer 
                    = new DataContractJsonSerializer(typeof(Index), settings);
                serializer.WriteObject(stream, this);
            }
        }

        public void LoadFromJson(string filename)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    DataContractJsonSerializerSettings settings
                        = new DataContractJsonSerializerSettings();
                    settings.UseSimpleDictionaryFormat = true;
                    DataContractJsonSerializer serializer
                        = new DataContractJsonSerializer(typeof(Index), settings);
                    Index index = (Index)serializer.ReadObject(stream);
                    this.files = index.files;
                    this.nextFileNumber = index.nextFileNumber;
                    this.words = index.words;
                    this.filename = filename;
                }
            }
            catch (FileNotFoundException) { }
            catch (SerializationException) { }
        }

        public void Dispose() { SaveToJson(filename); }

        public Index(ILemmatizer lemmatizer, string filename = "index.json")
        {
            this.Lemmatizer = lemmatizer;
            this.filename = filename;
            LoadFromJson(filename);
        }
    }
}
