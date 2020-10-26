using System;
using System.Collections.Generic;
using System.Text;

namespace QSelect
{
    public class ContentAlias
    {
        public string Folder { get; }
        public string Alias { get; }

        public ContentAlias(string folder, string alias)
        {
            Folder = folder;
            Alias = alias;
        }
    }
}
