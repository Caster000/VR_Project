using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //global information
    public int playerNumber;
    public bool isWin;

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
    public int NbContaminatedPlayerToVictory;


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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EndGame()
    {

    }

    public void ReloadGame()
    {

    }
    public void ReadConfigFile()
    {

    }
}
