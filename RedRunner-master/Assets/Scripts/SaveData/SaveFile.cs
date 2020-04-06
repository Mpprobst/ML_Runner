using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace RedRunner.TerrainGeneration
{
    public class SaveFile : MonoBehaviour
    {
        public string fileName = "game_data_names.json";
        public string path;
        public PlayerStats[] initialData;

        public SaveFileData data;

        private void Start()
        {
            Initialize();
            ReadData();
            Debug.Log("the save player exists");
        }

        public void Initialize()
        {
            fileName = "game_data_names.json";
            path = Application.persistentDataPath + "/" + fileName;
            Debug.Log(path);
        }

        public void SaveData()
        {
            Debug.Log("Saving...");

            SaveFileWrapper wrapper = new SaveFileWrapper();
            wrapper.data = data;

            string contents = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(path, contents);
            Debug.Log("Saved!");
        }

        public void ReadData()                  // This needs to be called when changing scenes
        {
            try
            {                                   // Checks if data is tampered
                if (File.Exists(path))          // Checks if data is lost
                {
                    string contents = File.ReadAllText(path);
                    SaveFileWrapper wrapper = JsonUtility.FromJson<SaveFileWrapper>(contents);
                    data = wrapper.data;
                    Debug.Log("reading");
                }
                else
                {
                    print("Unable to read game data. File does not exist. Creating new save file");
                    data = new SaveFileData();
                    SaveData();
                }
            }
            catch (System.Exception ex)
            {
                print(ex.Message);
            }
        }

    }

}