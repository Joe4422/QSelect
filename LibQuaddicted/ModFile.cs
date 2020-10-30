using System;
using System.IO;
using System.Xml;

namespace LibQuaddicted
{
    public class ModFile
    {
        public enum ModType
        {
            Zero,
            SingleBSPFiles,
            PartialConversion,
            Three,
            Speedmapping,
            MiscFiles
        }

        public string Id { get; }
        public ModType Type { get; }
        public Rating Rating { get; }
        public string Author { get; }
        public string Title { get; }
        public string MD5Sum { get; }
        public uint Size { get; }
        public DateTime Date { get; }
        public string Description { get; }
        public TechInfo TechInfo { get; }

        public string Directory { get; set; }
        public bool Downloaded => Directory != null;

        public ModFile(XmlNode root)
        {
            if (root.Name == "file")
            {
                if (root.Attributes["id"] != null) Id = root.Attributes["id"].InnerText;
                if (root.Attributes["type"] != null) Type = (ModType)int.Parse(root.Attributes["type"].InnerText);
                if (root.Attributes["rating"] != null)
                {
                    string r = root.Attributes["rating"].InnerText;
                    if (r == "") Rating = new Rating(null);
                    else Rating = new Rating(int.Parse(r));
                }
                if (root["author"] != null) Author = root["author"].InnerText;
                if (root["title"] != null) Title = root["title"].InnerText;
                if (root["md5sum"] != null) MD5Sum = root["md5sum"].InnerText;
                if (root["size"] != null) Size = uint.Parse(root["size"].InnerText);
                if (root["date"] != null)
                {
                    string[] splitDate = root["date"].InnerText.Split(".", StringSplitOptions.RemoveEmptyEntries);
                    if (int.Parse(splitDate[2]) >= 96)
                    {
                        splitDate[2] = "19" + splitDate[2];
                    }
                    else
                    {
                        splitDate[2] = "20" + splitDate[2];
                    }

                    Date = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[1]), int.Parse(splitDate[0]));
                }
                if (root["description"] != null) Description = root["description"].InnerText;
                if (root["techinfo"] != null) TechInfo = new TechInfo(root["techinfo"]);
            }
            else
            {
                throw new ArgumentException("Provided XML node was not a File node!");
            }
        }
    }
}
