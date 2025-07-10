using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public static class SaveSystem
{
    public static void SaveScores(float[] scores)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/scores.save";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        float[] data = scores;

        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static float[] LoadScores()
    {
        string path = Application.persistentDataPath + "/scores.save";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            float[] data = binaryFormatter.Deserialize(fileStream) as float[];
            fileStream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
