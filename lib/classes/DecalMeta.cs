using Decal_Loader.lib.scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decal_Loader.lib.classes
{
    public class DecalMeta
    {
        public string pack_name { get; set; }
        public string bundle_name { get; set; }
        public string category_name { get; set; }
        public Pack_Data pack_data { get; set; }
    }

    public class Pack_Data
    {
        public string version { get; set; }
        public string author { get; set; }
        public string meta { get; set; }
        public string description { get; set; }
    }
}
