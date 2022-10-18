using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class LevelConfig
{

    public static readonly string PATH = "./LevelConfigs/";
    
    public List<UnitModification> Modifications = new List<UnitModification>();

    // Disables debug logging
    public bool NoDebug;
    /// <summary>
    /// Loads the configuration.
    /// </summary>
    public void Load(string filename)
    {
        string jsonRepresentation = TextReader.LoadResourceTextfileFromStreamingAsset(PATH+filename+".json");
        // string jsonRepresentation = TextReader.LoadResourceTextfileFromStreamingAsset(PATH+"DefaultLevelConfig.json");

        if (jsonRepresentation != null)
        {
            JsonUtility.FromJsonOverwrite(jsonRepresentation, this);
            Debug.unityLogger.logEnabled = !NoDebug;
            Debug.Log("json" + jsonRepresentation);
            Debug.Log("listcount" + Modifications.Count);
        }
        else
        {
            Debug.LogWarning("LevelConfig.json content is null");
        }
    }

    /// <summary>
    /// Prints all the public variables and their content.
    /// </summary>
    public void DebugLog()
    {
        foreach (var prop in GetType().GetFields())
        {
            Debug.Log(String.Format("{0}={1}", prop.Name, prop.GetValue(this)));
        }
    }
}

