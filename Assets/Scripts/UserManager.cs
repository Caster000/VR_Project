using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Random = UnityEngine.Random;


public class UserManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject UserMeInstance;

    [SerializeField] private List<Transform> spawPoints;
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Canvas CanvasPlayer;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private GameObject BodyPlayer;
    [SerializeField] private GameObject CameraPlayer;
    private int currentHealth;
    private float respawnTime;
    private bool spawned;
    
    private GameConfig gameConfig;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;
        if (photonView.IsMine)
        {
            Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
            UserMeInstance = gameObject;
            Instantiate(CameraPlayer);
            CameraPlayer.SetActive(photonView.IsMine);
            CameraPlayer.GetComponent<vThirdPersonCamera>().SetMainTarget(transform);
        }
        GetComponent<vThirdPersonInput>().enabled = photonView.IsMine;
        GetComponent<Rigidbody>().isKinematic = !photonView.IsMine;
        CanvasPlayer.enabled = photonView.IsMine;
        
        //TODO Remove by loading list of spawn Points
            spawPoints = new List<Transform>();
            spawPoints.Add(GameObject.Find("SpawnArea").transform);
            spawned = true;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        if(respawnTime>0)     
        {         
            respawnTime -= Time.deltaTime;   
            double b = Mathf.RoundToInt(respawnTime); 
            TimerText.text = "Respawn in "+b.ToString ()+"s";
        }

        if (respawnTime <= 0 && !spawned)
        {
            Respawn();
        }
    }

    public void TakeDamage()
    {
        Healthbar.value = --currentHealth;
        if (currentHealth <= 0)
        {
            PrepareRespwan();
            return;
        }
    }

    private void PrepareRespwan()
    {
        // Canvas update
        spawned = false;
        CanvasPlayer.GetComponent<Image>().color = Color.black;
        Healthbar.gameObject.SetActive(false);
        // Hide the body and colliders  
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        BodyPlayer.SetActive(false);
        GetComponent<vThirdPersonInput>().enabled = false;
        // Teleport to spawnPoint
        Transform transform_spawn = spawPoints[Random.Range(0, spawPoints.Count)];
        transform.position = transform_spawn.position;
        TimerText.enabled = true;
        respawnTime = gameConfig.RespawnTime;
    }

    private void Respawn()
    {
        //Reset Canvas
        TimerText.text = "";
        TimerText.enabled = false;
        Healthbar.gameObject.SetActive(true);
        CanvasPlayer.GetComponent<Image>().color = Color.clear;
        Healthbar.value = currentHealth = gameConfig.LifeNumber;
        spawned = true;
            
        //Reset player health, position and shield
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        BodyPlayer.SetActive(true);
        GetComponent<vThirdPersonInput>().enabled = true;
            
        //TODO Rescale shield
    }

    #region Photon

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(BodyPlayer.activeSelf);
            stream.SendNext(GetComponent<Collider>().enabled);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
            BodyPlayer.SetActive((bool)stream.ReceiveNext());
            GetComponent<Collider>().enabled = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}
