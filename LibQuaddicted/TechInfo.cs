using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace LibQuaddicted
{
    public class TechInfo
    {
        public string ZipBaseDir { get; }
        public string CommandLine { get; }
        public List<string> StartMaps { get; }
        public List<string> Requirements { get; }

        public TechInfo(XmlNode root)
        {
            if (root.Name == "techinfo")
            {
                if (root["zipbasedir"] != null)
                {
                    ZipBaseDir = root["zipbasedir"].InnerText;
                }
                if (root["commandline"] != null)
                {
                    CommandLine = root["commandline"].InnerText;
                }
                if (root["startmap"] != null)
                {
                    StartMaps = root.ChildNodes.Cast<XmlNode>().Where(x => x.Name == "startmap").Select(x => x.InnerText).ToList();
                }
                if (root["requirements"] != null)
                {
                    Requirements = root["requirements"].ChildNodes.Cast<XmlNode>().Where(x => x.Name == "file").Select(x => x.Attributes["id"].InnerText).ToList();
                }
            }
            else
            {
                throw new ArgumentException("Node provided was not a techinfo node!");
            }
        }
    }
}
