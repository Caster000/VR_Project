using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
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
    private int nbContaminationArea;
    static public int nbContaminatedAreaByScientist;
    static public int nbContaminatedAreaByVirus;

    //ContaminationArea
    private List<Vector3> contaminationAreaPositions = new List<Vector3>();
    private List<Vector3> contaminationAreaRotations = new List<Vector3>();
    [Header("Contamination Area")]
    public GameObject contaminationAreaPrefab;
    public Transform contaminationAreaParent;

    //variables related to player contamination (kill)
    static public int ScientistScore;
    static public int VirusScore;
    private int nbContaminatedPlayerToVictory;

    //ThrowableObject
    private List<Vector3> throwableObjectPositions = new List<Vector3>();
    private List<Vector3> throwableObjectRotations = new List<Vector3>();
    [Header("Throwable Object")]
    public GameObject throwableObjectPrefab;
    public Transform throwableObjectParent;

    //Spawner
    private List<Vector3> spawnAreaPositions = new List<Vector3>();
    private List<Vector3> spawnAreaRotations = new List<Vector3>();
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

    void Awake()
    {
        ReadConfigFile();

    }
    // Start is called before the first frame update
    void Start()
    {
        GeneratingObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWin)
        {
            if (ScientistScore == nbContaminatedPlayerToVictory || VirusScore == nbContaminatedPlayerToVictory)
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
        nbContaminatedPlayerToVictory = 2;//levelConfig.nbContaminatedPlayerToVictory;
        nbContaminationArea = 2;//levelConfig.nbContaminationArea;
        contaminationAreaPositions = levelConfig.contaminationAreaPositions;
        contaminationAreaRotations = levelConfig.contaminationAreaRotations;
        throwableObjectPositions = levelConfig.throwableObjectPositions;
        throwableObjectRotations = levelConfig.throwableObjectRotations;
        spawnAreaPositions = levelConfig.spawnAreaPositions;
        spawnAreaRotations = levelConfig.spawnAreaRotations;
        //adding 0.01 in Y to avoid glitched texture
        for (int i = 0; i < spawnAreaPositions.Count; i++)
        {
            Vector3 temp = spawnAreaPositions[i];
            temp.y += (float)0.01;
            spawnAreaPositions[i] = temp;
        }
    }
    public void GeneratingObject()
    {
        for (int i = 0; i < contaminationAreaPositions.Count; i++)
        {

            GameObject contaminationAreaPosition = Instantiate(contaminationAreaPrefab, contaminationAreaPositions[i], Quaternion.Euler(contaminationAreaRotations[i]));
            contaminationAreaPosition.transform.SetParent(contaminationAreaParent);
        }
        for (int j = 0; j < throwableObjectPositions.Count; j++)
        {
            GameObject throwableObjectPosition = Instantiate(throwableObjectPrefab, throwableObjectPositions[j], Quaternion.Euler(throwableObjectRotations[j]));
            throwableObjectPosition.transform.SetParent(throwableObjectParent);
        }
        for (int k = 0; k < spawnAreaPositions.Count; k++)
        {

            GameObject spawnAreaPosition = Instantiate(spawnAreaPrefab, spawnAreaPositions[k], Quaternion.Euler(spawnAreaRotations[k]));
            spawnAreaPosition.transform.SetParent(spawnAreaParent);
        }
    }

}
