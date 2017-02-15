using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TMSNet.Importer.Dest
{
    public class JsonDestination : FileDestination
    {
        public JsonDestination(string path) : base(path)
        {
        }

        public override void Close()
        {
            File.WriteAllText(Path, JsonConvert.SerializeObject(Sections, Formatting.Indented));
        }
    }
}
