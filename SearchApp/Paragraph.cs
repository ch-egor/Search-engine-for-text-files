using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApp
{
    class Paragraph
    {
        public int Number { get; private set; }
        public string Content { get; private set; }
        public string Preview
        {
            get
            {
                bool shortened = false;
                int length = 200;
                if (this.Content.Length < length)
                {
                    shortened = true;
                    length = this.Content.Length;
                }
                string result = this.Content.Substring(0, length);
                if (!shortened)
                    result += "...";
                return result;
            }
        }

        public Paragraph(int number, string content)
        {
            this.Number = number;
            this.Content = content;
        }
    }
}
