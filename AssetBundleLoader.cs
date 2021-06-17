using System.IO;
using System.Linq;
using Base_Mod;
using JetBrains.Annotations;
using UnityEngine;

namespace Decal_Loader {
    [UsedImplicitly]
    public class AssetBundleLoader : BaseLoader {
        private static readonly byte[] UNITY_FS = {0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53}; // Asset bundle header: `UnityFS`

        public AssetBundleLoader(string configPath) : base(configPath) {
        }

        protected override void Load() {
            var files = Directory.EnumerateFiles(configPath, "*", SearchOption.TopDirectoryOnly)
                                 .Where(IsAssetBundle);

            foreach (var file in files) {
                var bundle      = AssetBundle.LoadFromFile(file);
                var allTextures = bundle.LoadAllAssets<Texture2D>().ToList();

                if (allTextures.Count == 0) return;

                var bundleName = Path.GetFileNameWithoutExtension(file);
                var category   = CreateDecalCategory(bundleName);

                var log = new LogBuffer();
                log.WriteLine($"Loading asset bundle: {bundleName}");
                foreach (var texture in allTextures) {
                    CreateDecal(category, bundleName, texture, log);
                }
                log.Flush();
            }
        }

        private bool IsAssetBundle(string file) {
            try {
                using (var reader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    var buffer = new byte[7];
                    return reader.Read(buffer, 0, buffer.Length) == 7 && buffer.SequenceEqual(UNITY_FS);
                }
            } catch (IOException) {
                return false;
            }
        }

        protected static void CreateDecal(DecalCategory category, string bundleName, Texture2D texture, LogBuffer log) {
            var name = texture.name;
            var file = bundleName + "::" + name;

            var guid = Database.instance.GetGuidForUri(file);
            if (guid == null) {
                guid = GUID.Create();
                Database.instance.Add(file, (GUID) guid);
            }

            RegisterDecal(category, (GUID) guid, texture);
            log.WriteLine($"Loaded decal: {file}.");
        }
    }
}