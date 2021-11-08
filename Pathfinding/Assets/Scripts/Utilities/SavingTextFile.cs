using UnityEngine;
using System;


public class SavingTextFile : MonoBehaviour
{
    public static void SaveMap(string nameFile, string content)
    {        
        var dir = Application.dataPath + "/Resources/mapJSON";

        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        var locationFile = string.Format("{0}/{1}.txt", dir, nameFile);

        try
        {
            System.IO.File.WriteAllText(locationFile, content);
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing json : " + e);
        }
    }
}
