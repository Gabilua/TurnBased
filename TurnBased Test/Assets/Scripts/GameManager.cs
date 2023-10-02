using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    [SerializeField] SaveFile saveFile;

    #region Data Management

    [ContextMenu("Reset Save File")]
    public void ResetSaveFile()
    {
        SaveFile emptyFile = new SaveFile();

        SaveToFile(emptyFile);
    }

    public void SaveToFile(SaveFile file)
    {
        string json = JsonUtility.ToJson(file);

        using (StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile.json"))
            writer.Write(json);
    }

    public void LoadFromFile()
    {
        if (!File.Exists(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile.json"))
        {
            ResetSaveFile();
            return;
        }

        string json = string.Empty;

        using (StreamReader reader = new StreamReader(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile.json"))
            json = reader.ReadToEnd();

        SaveFile file = JsonUtility.FromJson<SaveFile>(json);
        saveFile = file;
    }

    #endregion
}
