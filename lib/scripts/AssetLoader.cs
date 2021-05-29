using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Decal_Loader.lib.scripts
{
    public static class AssetLoader
    {
        // A map of loaded asset bundles using `assetBundleName` as the key.
        private static readonly Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

        public static void PrintAssets()
        {
            foreach (KeyValuePair<string, AssetBundle> dict in assetBundles)
            {
                Debug.Log("" + dict);
            }
        }

        public static GameObject GetAsset(string assetBundleName, string objectNameToLoad)
        {
            // See if we already loaded a bundle for `assetBundleName` and if not load it.
            if (!assetBundles.ContainsKey(assetBundleName)) LoadAssetBundle(assetBundleName);

            // Get the loaded assetBundle from the map.
            var assetBundle = assetBundles[assetBundleName];

            // Load the asset from the bundle.
            var asset = assetBundle.LoadAsset<GameObject>(objectNameToLoad);

            return asset;
        }

        public static Material GetMaterial(string assetBundleName, string objectNameToLoad)
        {

            // See if we already loaded a bundle for `assetBundleName` and if not load it.
            if (!assetBundles.ContainsKey(assetBundleName)) LoadAssetBundle(assetBundleName);

            // Get the loaded assetBundle from the map.
            var assetBundle = assetBundles[assetBundleName];

            // Load the asset from the bundle.
            var asset = assetBundle.LoadAsset<Material>(objectNameToLoad);

            return asset;
        }

        public static Texture2D GetTexture(string assetBundleName, string objectNameToLoad)
        {

            // See if we already loaded a bundle for `assetBundleName` and if not load it.
            if (!assetBundles.ContainsKey(assetBundleName)) LoadAssetBundle(assetBundleName);

            // Get the loaded assetBundle from the map.
            var assetBundle = assetBundles[assetBundleName];

            // Load the asset from the bundle.
            var asset = assetBundle.LoadAsset<Texture2D>(objectNameToLoad);

            return asset;
        }

        public static void LoadAssetBundle(string assetBundleName)
        {
            // Paths.Combine can be called with any number of segments to combine
            //var filePath = Path.Combine($"{Environment.GetEnvironmentVariable("%LOCALAPPDATA%")}Low", "Volcanoid", "Volcanoids", "AssetBundles", assetBundleName);

            var filePath = assetBundleName;

            // Save the loaded assetBundle in the map using the `assetBundleName` key.
            assetBundles[assetBundleName] = AssetBundle.LoadFromFile(filePath);
        }

        // Returns the directory the calling method's DLL is in.
        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
