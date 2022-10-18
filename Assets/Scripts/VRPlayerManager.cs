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
    
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Canvas CanvasVRPlayer;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private GameObject CameraPlayer;
    [SerializeField] private GameObject gunVr;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject socket;
    [SerializeField] private Collider colliderHead;


    private List<Vector3> spawPoints;
    private int currentHealth;
    private float respawnTime;
    private bool spawned;
    private GameConfig gameConfig;
    private GameObject gunVrInstance;
    private GameObject shieldInstance;
    private Rigidbody _rigidbody;
    private LocomotionProvider _locomotionProvider;
    private PhotonTransformChildView _photonTransformChildView;
    private List<Transform> SynchronizedChildTransform = new List<Transform>();
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;
        Debug.Log(NetworkManager.isMulti);
        spawPoints = LevelConfigLoader.Instance.levelConfig.spawnAreaPositions;
        _photonTransformChildView = GetComponent<PhotonTransformChildView>();
        if (NetworkManager.isMulti)
        {
            if (photonView.IsMine)
            {
                Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
                UserMeInstance = gameObject;
                FindObjectOfType<DelayedTeleportation>().teleportationProvider =
                    GetComponent<TeleportationProvider>(); 
            }
            //Prepare Prefabs
            gunVr.GetComponent<TeleportGun>().gunspawnPoint = socket;
            gunVr.GetComponent<TeleportGun>().enabled = photonView.IsMine;
            gunVr.GetComponent<XRGrabInteractable>().enabled = photonView.IsMine;
            shieldPrefab.GetComponent<Rigidbody>().isKinematic = !photonView.IsMine;
            
            CameraPlayer.GetComponent<Camera>().enabled = photonView.IsMine;
            CameraPlayer.GetComponent<AudioListener>().enabled = photonView.IsMine;
            CanvasVRPlayer.enabled = photonView.IsMine;
            gunVrInstance = Instantiate(gunVr, transform.position + Vector3.up,Quaternion.identity);
            shieldInstance = Instantiate(shieldPrefab, transform.position,Quaternion.identity);
            
            //add to list to synchronized
            SynchronizedChildTransform.Add(gunVrInstance.transform);
            SynchronizedChildTransform.Add(shieldInstance.transform);
        }
        else
        {
            Debug.Log("instantiate Solo");
            CameraPlayer.SetActive(true);
            gunVrInstance = Instantiate(gunVr, transform.position + Vector3.up,Quaternion.identity);
            shieldInstance = Instantiate(shieldPrefab, transform.position,Quaternion.identity);
            FindObjectOfType<DelayedTeleportation>().teleportationProvider =
                GetComponent<TeleportationProvider>();
        }
        _locomotionProvider = GetComponent<LocomotionProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && NetworkManager.isMulti) return;
        
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
            if (photonView.IsMine)
            {
                PrepareRespwan();
            }
            return;
        }
    }

    private void PrepareRespwan()
    {
        // Canvas update
        spawned = false;
        CanvasVRPlayer.enabled = true;
        Healthbar.gameObject.SetActive(false);
        // Hide the body and colliders  
        colliderHead.enabled = false;
        _locomotionProvider.enabled = false;
        // Teleport to spawnPoint
        Vector3 transform_spawn = spawPoints[Random.Range(0, spawPoints.Count)];
        transform.position = transform_spawn;
        respawnTime = gameConfig.RespawnTime;
    }

    private void Respawn()
    {
        //Reset Canvas
        Healthbar.gameObject.SetActive(true);
        CanvasVRPlayer.enabled = false;
        Healthbar.value = currentHealth = gameConfig.LifeNumber;
        spawned = true;
            
        //Reset player health, position and shield
        
        colliderHead.enabled = true;
        _locomotionProvider.enabled = true;
            
        //TODO Rescale shield
    }
    
    #region Photon

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(colliderHead.enabled);
            
            for (int i = 0; i < SynchronizedChildTransform.Count; i++)
            {
                //Todo MAYBE interpolate
               
                stream.SendNext(SynchronizedChildTransform[i].localPosition);
                stream.SendNext(SynchronizedChildTransform[i].localRotation);
                stream.SendNext(SynchronizedChildTransform[i].localScale);
            }
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
            colliderHead.enabled = (bool)stream.ReceiveNext();
            
            for (int i = 0; i < SynchronizedChildTransform.Count; i++)
            {
                SynchronizedChildTransform[i].localPosition = (Vector3)stream.ReceiveNext();
                SynchronizedChildTransform[i].localRotation = (Quaternion)stream.ReceiveNext();
                SynchronizedChildTransform[i].localScale = (Vector3)stream.ReceiveNext();
            }
        }
    }

    #endregion
}
