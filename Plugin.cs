using System;
using System.Collections;
using System.IO;
using System.Linq;
using Base_Mod;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Decal_Loader {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override string ModName => "Decal-Loader";

        protected override void Init() {
            Database.Init(GetConfigPath(), GetConfigFile());

            base.Init();
        }

        protected override void OnIslandSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            base.OnIslandSceneLoaded(scene, loadSceneMode);

            var rootGameObject = scene.GetRootGameObjects()[0];
            if (!rootGameObject.HasComponent<LocalDecalLoader>()) {
                var localDecalLoader = rootGameObject.AddComponent<LocalDecalLoader>();
                localDecalLoader.loadImagePath = GetConfigPath();
            }
        }
    }

    [UsedImplicitly]
    public class LocalDecalLoader : MonoBehaviour {
        private static readonly GUID          MISC = GUID.Parse("447cd4412885bf44d9b1a5ffaddb3c23");
        public                  string        loadImagePath;
        private                 bool          done;
        private                 DecalCategory category;

        [UsedImplicitly]
        public void Start() {
            if (done || !Directory.Exists(loadImagePath)) return;

            category = (from dResource in RuntimeAssetStorage.GetAssets<DecalResource>()
                        where dResource.Category.AssetId == MISC
                        select dResource.Category).FirstOrDefault();

            if (category == null) {
                Debug.Log("Error: No category found.");
                return;
            }

            var files = Directory.EnumerateFiles(loadImagePath, "*", SearchOption.TopDirectoryOnly)
                                 .Where(file => file.EndsWith(".png")
                                                || file.EndsWith(".jpg")
                                                || file.EndsWith(".jpeg")
                                                || file.EndsWith(".bmp"));

            foreach (var file in files) {
                var absoluteUri = new Uri(file).AbsoluteUri;
                var request     = new WWW(absoluteUri);
                StartCoroutine(WaitForRequest(request));
            }

            done = true;
        }

        private IEnumerator WaitForRequest(WWW request) {
            yield return request;

            if (request.error != null) {
                Debug.Log("Error: Request error.");
                yield break;
            }

            var uri  = request.url;
            var guid = Database.instance.GetGuidForUri(uri);
            if (guid == null) {
                Debug.Log("Making new guid.");
                guid = GUID.Create();
                Database.instance.Add(uri, (GUID) guid);
            }

            var decalResource = (DecalResource) ScriptableObject.CreateInstance(typeof(DecalResource));
            decalResource.Resource = request.texture;
            decalResource.Category = category;

            RuntimeAssetStorage.Add(new[] {
                new AssetReference {
                    Object = decalResource,
                    Guid   = (GUID) guid,
                    Labels = new string[0]
                }
            });
            Debug.Log($"Loaded decal: {uri}.");
        }
    }
}