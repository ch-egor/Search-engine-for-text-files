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
    class EnglishLemmatizer : ILemmatizer
    {
        private static EnglishLemmatizer lemmatizer = null;

        [DataMember]
        private SortedDictionary<string, string> lemmaDict 
            = new SortedDictionary<string,string>();

        [DataMember]
        private HashSet<string> stopWords = new HashSet<string>();

        public void Include(string filename)
        {
            using (StreamReader input = new StreamReader(filename))
            {
                string currentLemma = String.Empty;
                while (!input.EndOfStream)
                {
                    string currentString = input.ReadLine();
                    string processedString = currentString.Trim().ToLower();
                    if (!String.IsNullOrWhiteSpace(currentString))
                    {
                        if (!Char.IsWhiteSpace(currentString[0]))
                            currentLemma = processedString;
                        else
                            BindToLemma(processedString, currentLemma);
                    }
                }
            }
        }

        private void BindToLemma(string word, string lemma)
        {
            lemmaDict.Add(word, lemma);
        }

        public string GetLemma(string word)
        {
            string wordToSearch = word.ToLower();
            if (lemmaDict.ContainsKey(wordToSearch))
                return lemmaDict[wordToSearch];
            else
                return wordToSearch;
        }

        public void LoadStopWords(string filename)
        {
            stopWords.Clear();
            using (StreamReader input = new StreamReader(filename))
                while (!input.EndOfStream)
                {
                    string word = input.ReadLine().Trim().ToLower();
                    stopWords.Add(word);
                }
        }

        public bool IsStopWord(string word)
        {
            return stopWords.Contains(word);
        }

        public void SaveToXml(string filename)
        {
            File.Delete(filename);
            using (FileStream stream = File.OpenWrite(filename))
            {
                DataContractSerializer serializer = 
                    new DataContractSerializer(typeof(EnglishLemmatizer));
                serializer.WriteObject(stream, this);
            }
        }

        public void SaveToJson(string filename)
        {
            File.Delete(filename);
            using (FileStream stream = File.OpenWrite(filename))
            {
                DataContractJsonSerializerSettings settings = 
                    new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;
                DataContractJsonSerializer serializer = 
                    new DataContractJsonSerializer(typeof(EnglishLemmatizer), settings);
                serializer.WriteObject(stream, this);
            }
        }

        private EnglishLemmatizer()
        {
            string[] basewrdsFiles = Directory.GetFiles("basewrds");
            foreach (string filename in basewrdsFiles)
                Include(filename);
            LoadStopWords("stopwords.txt");
        }

        public static EnglishLemmatizer Get()
        {
            if (lemmatizer == null)
                lemmatizer = new EnglishLemmatizer();
            return lemmatizer;
        }
    }
}
