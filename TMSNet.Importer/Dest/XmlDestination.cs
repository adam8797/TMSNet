using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace TMSNet.Importer.Dest
{
    public class XmlDestination : FileDestination
    {
        public XmlDestination(string path) : base(path)
        {
        }

        public override void Close()
        {
            using (var writer = new StreamWriter(Path))
            {
                var x = new XmlSerializer(typeof(List<ClassSection>));
                x.Serialize(writer, Sections);
            }
        }

    }
}