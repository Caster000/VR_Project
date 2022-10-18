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
    static public int nbContaminatedAreaByScientist;
    static public int nbContaminatedAreaByVirus;

    
    
    [Header("Contamination Area")]
    public GameObject contaminationAreaPrefab;
    public Transform contaminationAreaParent;

    //variables related to player contamination (kill)
    static public int ScientistScore;
    static public int VirusScore;

  
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

    private LevelConfig levelConfig;
    private GameConfig gameConfig;

    static public string spawnArea = "SpawnArea";
    static public string throwableObject = "ThrowableObject";
    static public string contaminationArea = "ContaminationArea";


    void Awake()
    {
        ReadConfigFile();
    }
    // Start is called before the first frame update
    void Start()
    {
        CountContaminationArea();
        GeneratingObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWin)
        {
            if (ScientistScore == gameConfig.NbContaminatedPlayerToVictory || VirusScore == gameConfig.NbContaminatedPlayerToVictory)
            {
                isWin = true;
                winner = ScientistScore > VirusScore ? "Scientist" : "Virus";
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
                spawnArea.transform.SetParent(spawnAreaParent);

            } if(levelConfig.Modifications[i].modification == throwableObject)
            {
                GameObject ThrowableObject = Instantiate(throwableObjectPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                ThrowableObject.transform.SetParent(throwableObjectParent);

            } if(levelConfig.Modifications[i].modification == contaminationArea)
            {
                GameObject ContaminationArea = Instantiate(contaminationAreaPrefab, levelConfig.Modifications[i].position, levelConfig.Modifications[i].rotation);
                ContaminationArea.transform.SetParent(contaminationAreaParent);

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

}
