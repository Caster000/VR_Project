using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfigLoader : MonoBehaviour
{
    #region "PublicAttributes"
    public static LevelConfigLoader Instance { get; private set; }

    [SerializeField] public LevelConfig levelConfig { get; private set;  }
    #endregion

    #region "PrivateAttributes"
    private LevelConfigLoader() { }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
