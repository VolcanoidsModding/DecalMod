using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Decal_Loader {
    public class Database {
        public static    Database                   instance { get; private set; }
        private readonly string                     configPath;
        private readonly string                     configFile;
        private          Dictionary<string, string> data; // <uri, guid>

        public static void Init(string configPath, string configFile) {
            instance = new Database(configPath, configFile);
        }

        private Database(string configPath, string configFile) {
            this.configPath = configPath;
            this.configFile = configFile;

            Load();
        }

        private void Load() {
            try {
                if (File.Exists(configFile)) {
                    var json = File.ReadAllText(configFile);
                    data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }

            if (data == null) {
                data = new Dictionary<string, string>();
            }
        }

        public GUID? GetGuidForUri(string uri) {
            uri = SanitizeUrl(uri);

            if (data.ContainsKey(uri)) return GUID.Parse(data[uri]);
            return null;
        }

        /**
         * Removes the config path from the URI if present.
         * Also replaces `\` with `/`.
         *
         * This ensures that any given file should compute to the same URI no matter where the user's app data folder is, or what OS the game is running on.
         */
        private string SanitizeUrl(string uri) {
            return uri.Replace(configPath, "").Replace(@"\", "/");
        }

        public void Add(string uri, GUID guid) {
            uri = SanitizeUrl(uri);

            data[uri] = guid.ToString();

            Save();
        }

        private void Save() {
            try {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                Directory.CreateDirectory(configPath);
                File.WriteAllText(configFile, json);
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
    }
}