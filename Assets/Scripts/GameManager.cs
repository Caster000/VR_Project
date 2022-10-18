using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //global information
    //private int playerNumber;
    private bool isWin;
    private bool isStarted;
    private string winner;


    // variables related to contamination area
     public int nbContaminationArea;
     public int nbContaminatedAreaByScientist;
     public int nbContaminatedAreaByVirus;

    
    
    [Header("Contamination Area")]
    public GameObject contaminationAreaPrefab;
    public Transform contaminationAreaParent;

    //variables related to player contamination (kill)
    public int scientistScore;
    public int virusScore;

  
    [Header("Throwable Object")]
    public GameObject throwableObjectPrefab;
    public Transform throwableObjectParent;

    
    [Header("Spawner")]
    public GameObject spawnAreaPrefab;
    public Transform spawnAreaParent;
    public List<Vector3> spawnAreaList = new List<Vector3>();

    //Audio source
    [Header("Audio source")]

    public AudioSource VictorySound;
    public AudioSource DefeatSound;

    //UI
    [Header("User Interface")]
    public CanvasUIScript _CanvasUIScript;
    public bool isCanvasEnabled;
    
    // public Canvas canvasEndText;
    // public TextMeshProUGUI endText;
    // public TextMeshProUGUI virusScoreText;
    // public TextMeshProUGUI scientistScoreText;
    // public Slider scientistSlider;
    // public Slider virusSlider;
    // public TextMeshProUGUI contaminationAreaScientistText;
    // public TextMeshProUGUI contaminationAreaVirusText;
    // public TextMeshProUGUI contaminationAreaNeutralText;
    private LevelConfig levelConfig;
    private GameConfig gameConfig;

    public static GameManager Instance { get; private set; }

    static public string spawnArea = "SpawnArea";
    static public string throwableObject = "ThrowableObject";
    static public string contaminationArea = "ContaminationArea";


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

        if (NetworkManager.isMulti)
        {
            ReadServerConfig();
        }
        else
        {
            ReadConfigFile();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GeneratingObject();
        CountContaminationArea();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanvasEnabled)
        {
            // Setup the canvas Ui with Gameconfig variables
            SetupCanvasUI();
            isCanvasEnabled = false;
            // isStarted = true;
        }
        if (!isWin && isStarted)
        {
            //Todo victory condition from Gameconfig to add
            if (scientistScore == gameConfig.NbContaminatedPlayerToVictory || virusScore == gameConfig.NbContaminatedPlayerToVictory)
            {
                isWin = true;
                winner = scientistScore > virusScore ? "Scientist" : "Virus";
                EndGame(winner);

            }
            if (nbContaminatedAreaByScientist == nbContaminationArea || nbContaminatedAreaByVirus == nbContaminationArea)
            {
                isWin = true;
                winner = nbContaminatedAreaByScientist > nbContaminatedAreaByVirus ? "Scientist" : "Virus";
                EndGame(winner);
            }
        }
       
    }

    public void SetupCanvasUI()
    {
        Debug.Log("SetupCanvasUI");
        _CanvasUIScript.virusSlider.minValue = 0;
        _CanvasUIScript.scientistSlider.minValue = 0;
        _CanvasUIScript.virusSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
        _CanvasUIScript.scientistSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
        _CanvasUIScript.contaminationAreaNeutralText.text = nbContaminationArea.ToString();
    }
    //TODO Ecran de victoire / ecran de défaite en focntion de l'équipe
    public void EndGame(string winner)
    {
        if (isWin)
        {
            _CanvasUIScript.canvasEndText.gameObject.SetActive(true);
            _CanvasUIScript.endText.text = winner + "\nwin";
            _CanvasUIScript.endText.color = winner == "Scientist" ? new Color32(25, 207, 0, 237) : new Color32(255, 0, 0, 237);
        }
    }

    public void ReloadGame()
    {
        //TODO isStarted =true
    }
    public void ReadConfigFile()
    {
        levelConfig = LevelConfigLoader.Instance.levelConfig;
        gameConfig = GameConfigLoader.Instance.gameConfig;
    }

    public void ReadServerConfig()
    {
        ExitGames.Client.Photon.Hashtable serverConfig = PhotonNetwork.CurrentRoom.CustomProperties;
        LevelConfigLoader.Instance.levelConfig = JsonUtility.FromJson<LevelConfig>((string)serverConfig["LevelConfig"]);
        GameConfigLoader.Instance.gameConfig = JsonUtility.FromJson<GameConfig>((string)serverConfig["GameConfig"]);
        Debug.Log("ReadServerConfig");
        ReadConfigFile();
    }
    public void GeneratingObject()
    {
        for( int i = 0; i < levelConfig.Modifications.Count; i++)
        {
            if(levelConfig.Modifications[i].modification == spawnArea)
            {
                GameObject spawnArea = Instantiate(spawnAreaPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                spawnArea.transform.SetParent(spawnAreaParent, false);
                spawnAreaList.Add(spawnArea.transform.position);

            } if(levelConfig.Modifications[i].modification == throwableObject)
            {
                GameObject ThrowableObject = Instantiate(throwableObjectPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                ThrowableObject.transform.SetParent(throwableObjectParent, false);

            } if(levelConfig.Modifications[i].modification == contaminationArea)
            {
                GameObject ContaminationArea = Instantiate(contaminationAreaPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                ContaminationArea.transform.SetParent(contaminationAreaParent, false);

            }
        }
       
    }
    public void CountContaminationArea()
    {
        for (int i = 0; i < levelConfig.Modifications.Count; i++)
        {
            if (levelConfig.Modifications[i].modification == contaminationArea)
            {
                nbContaminationArea++;
            }
        }
    }
    public void IncreaseVirusScore()
    {
        virusScore++;
        _CanvasUIScript.virusScoreText.text = virusScore.ToString();
    }public void IncreaseScientificScore()
    {
        scientistScore++;
        _CanvasUIScript.scientistScoreText.text = scientistScore.ToString();
    }
    public void IncreaseScientificSlider()
    {
        _CanvasUIScript.scientistSlider.value += scientistScore;
    } public void IncreaseVirusSlider()
    {
        _CanvasUIScript.virusSlider.value += virusScore;
    }
    public void IncreaseContaminationAreaScientistScore()
    {
        nbContaminatedAreaByScientist++;
        _CanvasUIScript.contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();

    }
    public void IncreaseContaminationAreaVirusScore()
    {
        nbContaminatedAreaByVirus++;
        _CanvasUIScript.contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
    }
    public void DecreaseContaminationAreaNeutralScore()
    {
        nbContaminationArea--;
        _CanvasUIScript.contaminationAreaNeutralText.text = nbContaminationArea.ToString();
    }public void DecreaseContaminationAreaScientistScore()
    {
        nbContaminatedAreaByScientist--;
        _CanvasUIScript.contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();
    }public void DecreaseContaminationAreaVirusScore()
    {
        nbContaminatedAreaByVirus--;
        _CanvasUIScript.contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
    }

}
