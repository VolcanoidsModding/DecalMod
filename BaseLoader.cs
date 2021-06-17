using System.IO;
using UnityEngine;

namespace Decal_Loader {
    public abstract class BaseLoader {
        protected readonly string configPath;

        protected BaseLoader(string configPath) {
            this.configPath = configPath;
        }

        public void Init() {
            if (!Directory.Exists(configPath)) return;
            Load();
        }

        protected abstract void Load();

        protected static DecalCategory CreateDecalCategory(string categoryName) {
            var category = GameMod.CreateAndRegister<DecalCategory>(GUID.Create()); // GUID doesn't matter. Category is transient.
            category.NameLocalization = new LocalizedString($"decals.category.{categoryName}", categoryName, ".");
            LocalizationManager.Localize(category);
            return category;
        }

        protected static void RegisterDecal(DecalCategory category, GUID guid, Texture2D texture) {
            var decalResource = GameMod.CreateAndRegister<DecalResource>(guid);
            decalResource.Resource = texture;
            decalResource.Category = category;
        }

        protected static Texture2D LoadTexture(string file) {
            var fileData = File.ReadAllBytes(file);
            var tex      = new Texture2D(1, 1);
            tex.LoadImage(fileData);
            return tex;
        }
    }
}