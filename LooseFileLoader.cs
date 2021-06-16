using System.IO;
using System.Linq;
using Base_Mod;

namespace Decal_Loader {
    public class LooseFileLoader : BaseLoader {
        public LooseFileLoader(string configPath) : base(configPath) {
        }

        protected override void Load() {
            var files = Directory.EnumerateFiles(configPath, "*", SearchOption.TopDirectoryOnly)
                                 .Where(file => file.EndsWith(".png")
                                                || file.EndsWith(".jpg")
                                                || file.EndsWith(".jpeg")
                                                || file.EndsWith(".bmp"))
                                 .ToList();

            if (files.Count == 0) return;

            var category = CreateDecalCategory("Modded");

            var log = new LogBuffer();
            foreach (var file in files) {
                CreateDecal(category, file, log);
            }
            log.Flush();
        }

        protected static void CreateDecal(DecalCategory category, string file, LogBuffer log) {
            var guid = Database.instance.GetGuidForUri(file);
            if (guid == null) {
                guid = GUID.Create();
                Database.instance.Add(file, (GUID) guid);
            }

            var texture = LoadTexture(file);
            RegisterDecal(category, (GUID) guid, texture, log);
            log.WriteLine($"Loaded decal: {file}.");
        }
    }
}