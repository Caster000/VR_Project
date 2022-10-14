using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class LevelConfig
{

    private static readonly string PATH = "./ConfigFiles/LevelConfig.json";
    #region fetched variable
    public LevelConfig contaminationArea = new LevelConfig();
    public LevelConfig throwableObject = new LevelConfig();
    public LevelConfig SpawnArea = new LevelConfig();

    // Disables debug logging

    public bool NoDebug;
    /// <summary>
    /// Loads the configuration.
    /// </summary>
    public void Load()
    {
        string jsonRepresentation = TextReader.LoadResourceTextfileFromStreamingAsset(PATH);
        if(jsonRepresentation != null)
        {
            JsonUtility.FromJsonOverwrite(jsonRepresentation, this);
            Debug.unityLogger.logEnabled = !NoDebug;
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

