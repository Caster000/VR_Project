using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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
    public bool Load()
    {
        try
        {
            string jsonRepresentation = TextReader.LoadResourceTextfileFromStreamingAsset(PATH);
            if (string.IsNullOrEmpty(jsonRepresentation)) throw new LoadingException("GameConfig.json content is not valid.");

            JsonUtility.FromJsonOverwrite(jsonRepresentation, this);
            Debug.unityLogger.logEnabled = !NoDebug;
            return true;
        } catch(Exception e)
        {
            if(e is LoadingException || e is IOException)
            {
                Debug.LogError(e.Message);
                return false;
            }
            throw e;
        }
    }

    public bool Write()
    {
        try
        {
            File.WriteAllText(PATH, JsonUtility.ToJson(this));
            return true;
        } catch(IOException ioe)
        {
            Debug.LogError(ioe.Message);
            return false;
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
