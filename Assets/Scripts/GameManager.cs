using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    //global information
    //private int playerNumber;
    private bool isWin;
    private string winner;
    private bool isStarted;
    // variables related to contamination area
    public int nbContaminationArea;
    public int nbContaminatedAreaByScientist;
    public int nbContaminatedAreaByVirus;

    public bool killToVictory;
    public bool contaminationVictory;



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

        if (NetworkManager.isMulti && PhotonNetwork.IsMasterClient)
        {
            ReadServerConfig();
        }
        if (!NetworkManager.isMulti)
        {
            ReadConfigFile();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GeneratingObject();
            CountContaminationArea();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        if (isCanvasEnabled)
        {
            // Setup the canvas Ui with Gameconfig variables
            SetupCanvasUI();
            isCanvasEnabled = false;
        }

       
       
            if (!isWin && (killToVictory || contaminationVictory) && PhotonNetwork.IsConnected)
            {
            LoadUI();

            //Todo victory condition from Gameconfig to add
            if (scientistScore == gameConfig.NbContaminatedPlayerToVictory || virusScore == gameConfig.NbContaminatedPlayerToVictory)
                {
                    isWin = true;
                    winner = scientistScore > virusScore ? "scientist" : "virus";
                    EndGame(winner);


                }
                if (nbContaminatedAreaByScientist == nbContaminationArea || nbContaminatedAreaByVirus == nbContaminationArea)
                {
                    isWin = true;
                    winner = nbContaminatedAreaByScientist > nbContaminatedAreaByVirus ? "scientist" : "virus";
                    EndGame(winner);


                
            }
        }
        
        if (isWin)
        {

            if (PhotonNetwork.IsMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    ReloadGame();
                }
            }
               
        }
    }

    public void SetupCanvasUI()
    {
        Debug.Log("SetupCanvasUI");
        if (_CanvasUIScript)
        {
            _CanvasUIScript.virusSlider.minValue = 0;
            _CanvasUIScript.scientistSlider.minValue = 0;
            _CanvasUIScript.virusSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
            _CanvasUIScript.scientistSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
            _CanvasUIScript.contaminationAreaNeutralText.text = nbContaminationArea.ToString();
        }

       
    }
    public void LoadUI()
    {
        if (!killToVictory && _CanvasUIScript)
        {
            _CanvasUIScript.ScientistContaminationPanel.SetActive(false);
            _CanvasUIScript.VirusContaminationPanel.SetActive(false);
        }
        if (!contaminationVictory && _CanvasUIScript)
        {
            _CanvasUIScript.ContaminationAreaPanel.SetActive(false);

        }
    }
    //TODO Ecran de victoire / ecran de défaite en focntion de l'équipe
    public void EndGame(string winner)
    {

        if (_CanvasUIScript)
        {
            _CanvasUIScript.canvasEndText.gameObject.SetActive(true);
            _CanvasUIScript.endText.fontSize = 28;
            _CanvasUIScript.endText.text = "the winner of the game are the " + winner;
            _CanvasUIScript.reloadText.text = "Press X to reload the game...";
        }
        
    }

    public void ReloadGame()
    {
           // Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene("Warmup");
    }
    public void ReadConfigFile()
    {
        levelConfig = LevelConfigLoader.Instance.levelConfig;
        gameConfig = GameConfigLoader.Instance.gameConfig;
        contaminationVictory = gameConfig.contaminationVictory;
        killToVictory = gameConfig.killToVictory;
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
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < levelConfig.Modifications.Count; i++)
            {
                if (levelConfig.Modifications[i].modification == spawnArea)
                {
                    // GameObject spawnArea = Instantiate(spawnAreaPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                    GameObject spawnArea = PhotonNetwork.InstantiateRoomObject("Prefabs/"+spawnAreaPrefab.name, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                    spawnArea.transform.SetParent(spawnAreaParent, false);
                    spawnAreaList.Add(spawnArea.transform.position);

            } if(levelConfig.Modifications[i].modification == throwableObject)
            {
                GameObject ThrowableObject = PhotonNetwork.InstantiateRoomObject("Prefabs/"+throwableObjectPrefab.name, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                ThrowableObject.transform.SetParent(throwableObjectParent, false);

                }
                if (levelConfig.Modifications[i].modification == contaminationArea)
                {
                    GameObject ContaminationArea = PhotonNetwork.InstantiateRoomObject("Prefabs/"+contaminationAreaPrefab.name, levelConfig.Modifications[i].position, Quaternion.identity);
                    ContaminationArea.transform.SetParent(contaminationAreaParent, false);

                }
            }
        }
       

    }
    public void CountContaminationArea()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < levelConfig.Modifications.Count; i++)
            {
                if (levelConfig.Modifications[i].modification == contaminationArea)
                {
                    nbContaminationArea++;
                    Debug.Log("incount"+ nbContaminationArea);
                }
            }
        }
       
    }
    public void IncreaseVirusScore()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            virusScore++;
            if (_CanvasUIScript)
            {
                _CanvasUIScript.virusScoreText.text = virusScore.ToString();
            }
        }
       
    }
    public void IncreaseScientificScore()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            scientistScore++;
            if (_CanvasUIScript)
            {
                _CanvasUIScript.scientistScoreText.text = scientistScore.ToString();
            }
        }
    }
    public void IncreaseScientificSlider()
    {
        _CanvasUIScript.scientistSlider.value += scientistScore;
    }
    public void IncreaseVirusSlider()
    {
        _CanvasUIScript.virusSlider.value += virusScore;
    }
    public void IncreaseContaminationAreaScientistScore()
    {
        nbContaminatedAreaByScientist++;
        if (_CanvasUIScript)
        {
            _CanvasUIScript.contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();
        }

    }
    public void IncreaseContaminationAreaVirusScore()
    {
        nbContaminatedAreaByVirus++;
        if (_CanvasUIScript)
        {
            _CanvasUIScript.contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
        }
    }
    public void DecreaseContaminationAreaNeutralScore()
    {
        nbContaminationArea--;
        _CanvasUIScript.contaminationAreaNeutralText.text = nbContaminationArea.ToString();
    }
    public void DecreaseContaminationAreaScientistScore()
    {
        nbContaminatedAreaByScientist--;
        if (_CanvasUIScript)
        {
            _CanvasUIScript.contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();
        }
    }
    public void DecreaseContaminationAreaVirusScore()
    {
        nbContaminatedAreaByVirus--;
        if (_CanvasUIScript)
        {
            _CanvasUIScript.contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
        }
    }

    public void UpdateUI()
    {

        if (_CanvasUIScript)
        {
            _CanvasUIScript.contaminationAreaNeutralText.text = nbContaminationArea.ToString();
            _CanvasUIScript.contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();
            _CanvasUIScript.contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
            _CanvasUIScript.scientistScoreText.text = scientistScore.ToString();
            _CanvasUIScript.virusScoreText.text = virusScore.ToString();
            _CanvasUIScript.virusSlider.value = virusScore;
            _CanvasUIScript.scientistSlider.value = scientistScore;
            _CanvasUIScript.virusSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
            _CanvasUIScript.scientistSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
        }

    }
    #region Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("virusScore" + virusScore);
            Debug.Log("scientistScore" + scientistScore);
            Debug.Log("nbContaminationArea" + nbContaminationArea);
            Debug.Log("nbContaminatedAreaByScientist" + nbContaminatedAreaByScientist);
            Debug.Log("nbContaminatedAreaByVirus" + nbContaminatedAreaByVirus);
            stream.SendNext(virusScore);
            stream.SendNext(scientistScore);
            stream.SendNext(nbContaminationArea);
            stream.SendNext(nbContaminatedAreaByScientist);
            stream.SendNext(nbContaminatedAreaByVirus);
            stream.SendNext(killToVictory);
            stream.SendNext(contaminationVictory);

        }
        else
        {
            virusScore = (int)stream.ReceiveNext();
            scientistScore = (int)stream.ReceiveNext();
            nbContaminationArea = (int)stream.ReceiveNext();
            nbContaminatedAreaByScientist = (int)stream.ReceiveNext();
            nbContaminatedAreaByVirus = (int)stream.ReceiveNext();
            killToVictory = (bool)stream.ReceiveNext();
            contaminationVictory = (bool)stream.ReceiveNext();
            Debug.Log("virusScore" + virusScore);
            Debug.Log("scientistScore" + scientistScore);
            Debug.Log("nbContaminationArea" + nbContaminationArea);
            Debug.Log("nbContaminatedAreaByScientist" + nbContaminatedAreaByScientist);
            Debug.Log("nbContaminatedAreaByVirus" + nbContaminatedAreaByVirus);
        }
    }
    #endregion
}
