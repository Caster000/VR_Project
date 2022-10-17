using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GameConfig
{
    private static readonly string PATH = "./GameConfig.json";

    #region "CESIRequiredVariables"
    // Initial number of times a player can be shot before dying
    public int LifeNumber;
    // Minimal delay between two shots fired
    public float DelayShoot;
    // Minimal delay between two virus teleportations
    public float DelayTeleport;
    // Viruses' shots' color
    public Color ColorShotVirus;
    // Scientists' shots' color
    public Color ColorShotKMS;
    // Number of player to contaminate before viruses win
    public int NbContaminatedPlayerToVictory;
    // Explosion radius of a contaminated object
    public float RadiusExplosion;
    // Time to stay in a zone to contaminate it
    public float TimeToAreaContamination;
    #endregion

    // Time before respawning
    public float RespawnTime;
    // Disables debug logging
    public bool NoDebug;
    // Enable friendlyFire
    public bool friendlyFire;
    
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
        } else
        {
            Debug.LogWarning("GameConfig.json content is null");
        }
    }

    /// <summary>
    /// Prints all the public variables and their content.
    /// </summary>
        public void DebugLog()
        {
            foreach(var prop in GetType().GetFields())
            {
                Debug.Log(String.Format("{0}={1}", prop.Name, prop.GetValue(this)));
            }
        }

}
