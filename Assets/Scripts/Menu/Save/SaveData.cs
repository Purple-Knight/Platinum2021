using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveData
{
    public static string directory = "/SaveData/";
    public static string fileName= "PlayersData.txt";


    public static void Save(PlayersData pd)
    {
        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(pd);
        File.WriteAllText(dir + fileName, json);
    }


    public static PlayersData Load()
    {
        string fullPath = Application.persistentDataPath + directory + fileName;
        PlayersData pd = new PlayersData();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            pd = JsonUtility.FromJson<PlayersData>(json);
        }
        else
        {
            Debug.Log("Save file do not exist");
        }
        return pd;
    }

}
