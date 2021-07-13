using System;
using System.Collections.Generic;
using System.IO;

namespace EmailTools
{
    public class Options
    {
        public IEnumerable<string> CCEmails { get; set; }
        public IEnumerable<string> BCCEmails { get; set; }
        public List<string> AttachmentsFilePath { get; set; } = new List<string>();
        public List<Tuple<string, Stream>> AttachmentsStream { get; set; } = new List<Tuple<string, Stream>>();
        public string AliasName { get; set; }
    }
}
