using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string Id;
    private string coucou;
    //global information
    public int playerNumber;
    public bool isWin;
    public string winner;


    // variables related to contamination area
    public int nbContaminationArea;
    public int nbContaminatedAreaByScientist;
    public int nbContaminatedAreaByVirus;

    // TODO make object instead of list
    public List<Transform> ContaminationAreaPosition = new List<Transform>();
    public List<Quaternion> ContaminationAreaRotation = new List<Quaternion>();


    //variables related to player contamination (kill)
    public int ScientistScore;
    public int VirusScore;
    public int nbContaminatedPlayerToVictory;


    //other gameObject
    public List<Transform> ThrowableObjectPosition = new List<Transform>();
    public List<Quaternion> ThrowableObjectRotation = new List<Quaternion>();
    public List<Transform> SpawnAreatPosition = new List<Transform>();
    public List<Quaternion> SpawnAreaObjectRotation = new List<Quaternion>();

    //Audio source
    public AudioSource VictorySound;
    public AudioSource DefeatSound;

    //UI
    public Text VictoryText;
    public Text DefeatText;

    private LevelConfig levelConfig;

    void Awake()
    {
        levelConfig = LevelConfigLoader.Instance.levelConfig;
        nbContaminatedPlayerToVictory = levelConfig.nbContaminatedPlayerToVictory;
        nbContaminationArea = levelConfig.nbContaminationArea;

    }
    // Start is called before the first frame update
    void Start()
    {
        /*  Debug.Log("id" + Id);
         Debug.Log("coucou" + coucou);
         Debug.Log("NbContaminatedPlayerToVictory" + nbContaminatedPlayerToVictory); */
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

    }
}
