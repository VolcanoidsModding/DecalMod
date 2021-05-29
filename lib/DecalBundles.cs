using Decal_Loader.lib.classes;
using Decal_Loader.lib.scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Decal_Loader.lib
{
    class DecalBundles
    {
        private static string loadPath;
        public static readonly Dictionary<string, DecalMeta> decalMeta = new Dictionary<string, DecalMeta>();
        public DecalBundles(string path)
        {
            loadPath = path;
        }

        public static void Init()
        {
            var decalPackMeta = Directory.EnumerateFiles(Path.Combine(loadPath, "DecalPacks"), "*.json", SearchOption.TopDirectoryOnly);

            foreach (var packMeta in decalPackMeta)
            {
                var root = JsonConvert.DeserializeObject<DecalMeta>(Path.Combine(loadPath, "DecalPacks", Path.GetFileName(packMeta)));
                decalMeta[root.pack_name] = root;
            }

            foreach (var item in (from dResource in RuntimeAssetStorage.GetAssets<DecalResource>()
                                  select dResource.Category).Distinct())
            {
                Debug.Log(item);
            }

            LoadAssets();
        }

        private static void LoadAssets()
        {
            foreach (KeyValuePair<string, DecalMeta> dict in decalMeta)
            {
                AssetLoader.LoadAssetBundle(Path.Combine(loadPath, "DecalPacks", dict.Value.bundle_name));
                foreach(var obj in RuntimeAssetStorage.GetAssets<DecalCategory>())
                {
                    Debug.Log(obj);
                    if(obj.Name == dict.Value.category_name)
                    {
                        Debug.Log(obj.Name);
                    }
                }
            }
        }
    }
}
