using System.IO;
using System.Linq;
using Base_Mod;
using JetBrains.Annotations;
using UnityEngine;

namespace Decal_Loader {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override string        ModName => "Decal-Loader";
        private            DecalCategory category;

        protected override void Init() {
            Database.Init(PersistentDataDir, GetConfigFile());

            base.Init();
        }

        public override void OnInitData() {
            var loadImagePath = PersistentDataDir;
            if (!Directory.Exists(loadImagePath)) return;

            category = CreateDecalCategory();

            var files = Directory.EnumerateFiles(loadImagePath, "*", SearchOption.TopDirectoryOnly)
                                 .Where(file => file.EndsWith(".png")
                                                || file.EndsWith(".jpg")
                                                || file.EndsWith(".jpeg")
                                                || file.EndsWith(".bmp"));

            var log = new LogBuffer();
            foreach (var file in files) {
                CreateDecal(file, log);
            }
            log.Flush();

            base.OnInitData();
        }

        private static DecalCategory CreateDecalCategory() {
            var category = CreateAndRegister<DecalCategory>(GUID.Create()); // GUID doesn't matter. Category is transient.
            category.NameLocalization = new LocalizedString("decals.category.modded", "Modded", ".");
            LocalizationManager.Localize(category);
            return category;
        }

        private void CreateDecal(string file, LogBuffer log) {
            var guid = Database.instance.GetGuidForUri(file);
            if (guid == null) {
                guid = GUID.Create();
                Database.instance.Add(file, (GUID) guid);
            }

            var decalResource = CreateAndRegister<DecalResource>((GUID) guid);
            decalResource.Resource = LoadTexture(file);
            decalResource.Category = category;

            log.WriteLine($"Loaded decal: {file}.");
        }

        private Texture2D LoadTexture(string file) {
            var fileData = File.ReadAllBytes(file);
            var tex      = new Texture2D(1, 1);
            tex.LoadImage(fileData);
            return tex;
        }
    }
}