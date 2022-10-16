using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //global information
    public int playerNumber;
    public bool isWin;
    public string winner;


    // variables related to contamination area
    public int nbContaminationArea;
    public int nbContaminatedAreaByScientist;
    public int nbContaminatedAreaByVirus;

    //ContaminationArea
    public List<Vector3> contaminationAreaPositions = new List<Vector3>();
    public List<Vector3> contaminationAreaRotations = new List<Vector3>();
    public GameObject contaminationAreaPrefab;
    public Transform contaminationAreaParent;

    //variables related to player contamination (kill)
    public int ScientistScore;
    public int VirusScore;
    public int nbContaminatedPlayerToVictory;


    //other gameObject
    //ThrowableObject
    public List<Vector3> throwableObjectPositions = new List<Vector3>();
    public List<Vector3> throwableObjectRotations = new List<Vector3>();
    public GameObject throwableObjectPrefab;
    public Transform throwableObjectParent;

    //Spawner
    public List<Vector3> spawnAreaPositions = new List<Vector3>();
    public List<Vector3> spawnAreaRotations = new List<Vector3>();
    public GameObject spawnAreaPrefab;
    public Transform spawnAreaParent;

    //Audio source
    public AudioSource VictorySound;
    public AudioSource DefeatSound;

    //UI
    public Text VictoryText;
    public Text DefeatText;

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
        if (ScientistScore == nbContaminatedPlayerToVictory || VirusScore == nbContaminatedPlayerToVictory)
        {
            isWin = true;
            winner = ScientistScore > VirusScore ? "ScientistWin" : "VirusWin";
            EndGame(winner);

        }
        if (nbContaminatedAreaByScientist == nbContaminationArea || nbContaminatedAreaByVirus == nbContaminationArea)
        {
            isWin = true;
            winner = nbContaminatedAreaByScientist > nbContaminatedAreaByVirus ? "ScientistWin" : "VirusWin";
            EndGame(winner);


        }
    }
    //TODO Ecran de victoire / ecran de défaite en focntion de l'équipe
    public void EndGame(string winner)
    {
        if (isWin)
        {
            Debug.Log("Victory" + winner);
        }




    }

    public void ReloadGame()
    {

    }
    public void ReadConfigFile()
    {
        levelConfig = LevelConfigLoader.Instance.levelConfig;
        nbContaminatedPlayerToVictory = levelConfig.nbContaminatedPlayerToVictory;
        nbContaminationArea = levelConfig.nbContaminationArea;
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
