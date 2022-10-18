using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelConfigLoader : MonoBehaviour
{
    public static LevelConfigLoader Instance { get; private set; }

    [SerializeField] public LevelConfig levelConfig { get; set; }

    private LevelConfigLoader() { }
    // Start is called before the first frame update

    //    public string SceneToLoad = "LobbyScene";
    //public string SceneToLoad = "GameScene";

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

    void Start()
    {
        levelConfig = new LevelConfig();
        levelConfig.Load("DefaultLevelConfig");
        levelConfig.DebugLog();
        //SceneManager.LoadScene(SceneToLoad);
    }
}
