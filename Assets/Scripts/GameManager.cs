using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //global information
    //private int playerNumber;
    private bool isWin;
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

    //Audio source
    [Header("Audio source")]

    public AudioSource VictorySound;
    public AudioSource DefeatSound;

    //UI
    [Header("User Interface")]

    public Canvas canvasEndText;
    public TextMeshProUGUI endText;
    public TextMeshProUGUI virusScoreText;
    public TextMeshProUGUI scientistScoreText;
    public Slider scientistSlider;
    public Slider virusSlider;
    public TextMeshProUGUI contaminationAreaScientistText;
    public TextMeshProUGUI contaminationAreaVirusText;
    public TextMeshProUGUI contaminationAreaNeutralText;
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
        ReadConfigFile();
    }
    // Start is called before the first frame update
    void Start()
    {
        CountContaminationArea();
        GeneratingObject();
        virusSlider.minValue = 0;
        scientistSlider.minValue = 0;
        virusSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
        scientistSlider.maxValue = gameConfig.NbContaminatedPlayerToVictory;
        contaminationAreaNeutralText.text = nbContaminationArea.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWin)
        {
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
    //TODO Ecran de victoire / ecran de défaite en focntion de l'équipe
    public void EndGame(string winner)
    {
        if (isWin)
        {
            canvasEndText.gameObject.SetActive(true);
            endText.text = winner + "\nwin";
            endText.color = winner == "Scientist" ? new Color32(25, 207, 0, 237) : new Color32(255, 0, 0, 237);
        }
    }

    public void ReloadGame()
    {

    }
    public void ReadConfigFile()
    {
        levelConfig = LevelConfigLoader.Instance.levelConfig;
        gameConfig = GameConfigLoader.Instance.gameConfig;
    }
    public void GeneratingObject()
    {
        for( int i = 0; i < levelConfig.Modifications.Count; i++)
        {
            if(levelConfig.Modifications[i].modification == spawnArea)
            {
                GameObject spawnArea = Instantiate(spawnAreaPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                spawnArea.transform.SetParent(spawnAreaParent, false);

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
        virusScoreText.text = virusScore.ToString();
    }public void IncreaseScientificScore()
    {
        scientistScore++;
        scientistScoreText.text = scientistScore.ToString();
    }
    public void IncreaseScientificSlider()
    {
        scientistSlider.value += scientistScore;
    } public void IncreaseVirusSlider()
    {
        virusSlider.value += virusScore;
    }
    public void IncreaseContaminationAreaScientistScore()
    {
        nbContaminatedAreaByScientist++;
        contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();

    }
    public void IncreaseContaminationAreaVirusScore()
    {
        nbContaminatedAreaByVirus++;
        contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
    }
    public void DecreaseContaminationAreaNeutralScore()
    {
        nbContaminationArea--;
        contaminationAreaNeutralText.text = nbContaminationArea.ToString();
    }public void DecreaseContaminationAreaScientistScore()
    {
        nbContaminatedAreaByScientist--;
        contaminationAreaScientistText.text = nbContaminatedAreaByScientist.ToString();
    }public void DecreaseContaminationAreaVirusScore()
    {
        nbContaminatedAreaByVirus--;
        contaminationAreaVirusText.text = nbContaminatedAreaByVirus.ToString();
    }

}
