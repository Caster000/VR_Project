using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using VR_Vs_KMS.Scripts;
using Random = UnityEngine.Random;

public class VRPlayerManager : MonoBehaviourPunCallbacks, IPunObservable, IPlayer
{
    public static GameObject UserMeInstance;
    
    [SerializeField] private List<Transform> spawPoints;
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Canvas CanvasPlayer;
    [SerializeField] private TMP_Text TimerText;
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
            FindObjectOfType<DelayedTeleportation>().teleportationProvider =
                GetComponent<TeleportationProvider>();
        }
        CameraPlayer.SetActive(photonView.IsMine);
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
       // CanvasPlayer.GetComponent<Image>().color = Color.black;
        Healthbar.gameObject.SetActive(false);
        // Hide the body and colliders  
       // GetComponent<Collider>().enabled = false;
        // GetComponent<Rigidbody>().useGravity = false;
        // GetComponent<Rigidbody>().isKinematic = true;
        // GetComponent<vThirdPersonInput>().enabled = false;
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
//        CanvasPlayer.GetComponent<Image>().color = Color.clear;
        Healthbar.value = currentHealth = gameConfig.LifeNumber;
        spawned = true;
            
        //Reset player health, position and shield
        
        // todo public values
//        GetComponent<Collider>().enabled = true;
       // GetComponent<Rigidbody>().useGravity = true;
       // GetComponent<Rigidbody>().isKinematic = false;
       // GetComponent<vThirdPersonInput>().enabled = true;
            
        //TODO Rescale shield
    }
    
    #region Photon

    // void ShootVr()
    // {
    //     photonView.RPC("PlayerShoot",RpcTarget.AllViaServer);
    // }
    // [PunRPC]
    // void PlayerShoot()
    // {
    //     gunBeahviour.Shoot();
    // }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            // stream.SendNext(BodyPlayer.activeSelf);
//            stream.SendNext(GetComponent<Collider>().enabled);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
            // BodyPlayer.SetActive((bool)stream.ReceiveNext());
           // GetComponent<Collider>().enabled = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}
