using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigLoader : MonoBehaviour
{
    #region "PublicAttributes"
    // Single instance of the game config loader game object in the scene
    public static GameConfigLoader Instance { get; private set; }
    // Instance of the game config
    [SerializeField] public GameConfig gameConfig { get; private set; }
    #endregion

    #region "PrivateAttributes"
    private GameConfigLoader() { } // Forbid programmers to create a game config loader via "new" keyword (enforces singleton)
    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one gameconfig instance exists. Put exactly one in the world.");
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameConfig = new GameConfig();
        gameConfig.Load();
        gameConfig.DebugLog();
    }

}

