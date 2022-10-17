using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]

public class LevelConfig
{

    public static readonly string PATH = "./LevelConfigs/";
    public int nbContaminatedPlayerToVictory;
    public int nbContaminationArea;
    //Contamination area

    public List<Vector3> contaminationAreaPositions = new List<Vector3>();
    public List<Vector3> contaminationAreaRotations = new List<Vector3>();
    // Throwable object
    public List<Vector3> throwableObjectPositions = new List<Vector3>();
    public List<Vector3> throwableObjectRotations = new List<Vector3>();

    //spawner
    public List<Vector3> spawnAreaPositions = new List<Vector3>();
    public List<Vector3> spawnAreaRotations = new List<Vector3>();


    // Disables debug logging
    public bool NoDebug;
    /// <summary>
    /// Loads the configuration.
    /// </summary>
    public void Load()
    {
        string jsonRepresentation = TextReader.LoadResourceTextfileFromStreamingAsset(PATH+"LevelConfig.json");
        if (jsonRepresentation != null)
        {
            JsonUtility.FromJsonOverwrite(jsonRepresentation, this);
            Debug.unityLogger.logEnabled = !NoDebug;
            Debug.Log("json" + jsonRepresentation);
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

