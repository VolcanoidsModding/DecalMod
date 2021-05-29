using System.IO;
using System.Linq;
using Base_Mod;
using Decal_Loader.lib;
using Decal_Loader.lib.scripts;
using JetBrains.Annotations;
using UnityEngine;

namespace Decal_Loader {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override string        ModName => "Decal-Loader";
        private            DecalCategory category;

        protected override void Init() {
            Database.Init(GetConfigPath(), GetConfigFile());

            base.Init();
        }

        protected override void OnDataSetup() {
            base.OnDataSetup();

            var loadImagePath = GetConfigPath();
            if (!Directory.Exists(loadImagePath)) return;

            category = CreateDecalCategory("Modded");

            var files = Directory.EnumerateFiles(loadImagePath, "*", SearchOption.TopDirectoryOnly)
                                 .Where(file => file.EndsWith(".png")
                                                || file.EndsWith(".jpg")
                                                || file.EndsWith(".jpeg")
                                                || file.EndsWith(".bmp"));

            foreach (var file in files) {
                CreateDecal(file);
            }

            new DecalBundles(loadImagePath);
            DecalBundles.Init();
        }

        private static DecalCategory CreateDecalCategory(string name) {
            string lcName = name.ToLower();
            var          localizedString = new LocalizedString("decals.category." + lcName, name, ".");
            var          category        = ScriptableObject.CreateInstance<DecalCategory>();
            category.name             = localizedString;
            category.NameLocalization = localizedString;
            LocalizationManager.Localize(category);

            RuntimeAssetStorage.Add(new[] {
                new AssetReference {
                    Object = category,
                    Guid   = GUID.Create(), // Doesn't matter. Category is transient.
                    Labels = new string[0]
                }
            });
            return category;
        }

        private void CreateDecal(string file) {
            var guid = Database.instance.GetGuidForUri(file);
            if (guid == null) {
                guid = GUID.Create();
                Database.instance.Add(file, (GUID) guid);
            }

            var decalResource = ScriptableObject.CreateInstance<DecalResource>();
            decalResource.Resource = LoadTexture(file);
            decalResource.Category = category;

            RuntimeAssetStorage.Add(new[] {
                new AssetReference {
                    Object = decalResource,
                    Guid   = (GUID) guid,
                    Labels = new string[0]
                }
            });
            Debug.Log($"Loaded decal: {file}.");
        }

        private Texture2D LoadTexture(string file) {
            var fileData = File.ReadAllBytes(file);
            var tex      = new Texture2D(1, 1);
            tex.LoadImage(fileData);
            return tex;
        }
    }
}