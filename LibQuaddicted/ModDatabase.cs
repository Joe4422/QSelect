using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace LibQuaddicted
{
    public class ModDatabase
    {
        public Dictionary<string, ModFile> ModFiles { get; }

        public ModDatabase(string url)
        {
            // Download file
            string xml;
            using (WebClient client = new WebClient())
            {
                xml = client.DownloadString(url);
            }
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            ModFiles = new Dictionary<string, ModFile>();

            if (document["files"] != null)
            {
                XmlNode files = document["files"];

                foreach (XmlNode node in files.ChildNodes)
                {
                    if (node.Name == "file")
                    {
                        ModFile file = new ModFile(node);

                        ModFiles.Add(file.Id, file);
                    }
                }
            }
        }
    }
}
