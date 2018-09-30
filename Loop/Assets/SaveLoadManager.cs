using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public static class SaveLoadManager {

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/data.gd");
        bf.Serialize(file, GameManager.instance.saveData);
        file.Close();
    }
	
    public static void Load()
    {
        Debug.Log(Application.persistentDataPath + "/data.gd");
        if (File.Exists(Application.persistentDataPath + "/data.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/data.gd", FileMode.Open);
            GameManager.instance.saveData = (SaveData)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            SaveData newSave = new SaveData();
            newSave.volumeOn = true;
            newSave.colorsUnlocked = new List<int>();
            newSave.skinsUnlocked = new List<int>();
            newSave.colorsUnlocked.Add(0);
            newSave.skinsUnlocked.Add(0);
            GameManager.instance.saveData = newSave;
        }
    }
}
