using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SearchApp
{
    class FileDescription
    {
        public string FullName { get; private set; }
        public string ShortName { get { return Path.GetFileName(this.FullName); } }

        public override string ToString() { return ShortName; }

        public FileDescription(string fullName) { this.FullName = fullName; }
    }
}
