using System.IO;
using Base_Mod;
using JetBrains.Annotations;

namespace Decal_Loader {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override void Init() {
            Database.Init(PersistentDataDir, GetConfigFile());

            base.Init();
        }

        public override void OnInitData() {
            var configPath = PersistentDataDir;
            if (!Directory.Exists(configPath)) return;

            new LooseFileLoader(configPath).Init();
            new AssetBundleLoader(configPath).Init();

            base.OnInitData();
        }
    }
}