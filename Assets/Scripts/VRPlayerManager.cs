using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem.XR;
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
    [SerializeField] private GameObject Head;


    private List<Vector3> spawPoints;
    private int currentHealth;
    private float respawnTime;
    private bool spawned;
    private GameConfig gameConfig;
    private GameObject gunVrInstance;
    private GameObject shieldInstance;
    private Rigidbody _rigidbody;
    private LocomotionProvider _locomotionProvider;
    private List<Transform> SynchronizedChildTransform = new List<Transform>();
    private CapsuleCollider _capsuleCollider;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;
        Debug.Log(NetworkManager.isMulti);
        spawPoints = LevelConfigLoader.Instance.levelConfig.spawnAreaPositions;
        
        gunVr.GetComponent<TeleportGun>().gunspawnPoint = socket;

        if (NetworkManager.isMulti)
        {
            //Prepare Prefabs
            gunVr.GetComponent<TeleportGun>().enabled = photonView.IsMine;
            gunVr.GetComponent<XRGrabInteractable>().enabled = photonView.IsMine;
            shieldPrefab.GetComponent<Rigidbody>().isKinematic = !photonView.IsMine;
            _capsuleCollider = GetComponent<CapsuleCollider>();
            
            if (photonView.IsMine)
            {
                Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
                UserMeInstance = gameObject;
                FindObjectOfType<DelayedTeleportation>().teleportationProvider =
                    GetComponent<TeleportationProvider>(); 
                
                shieldInstance = PhotonNetwork.Instantiate("Prefabs/"+shieldPrefab.name, transform.position,Quaternion.identity);
            }

            CameraPlayer.GetComponent<Camera>().enabled = photonView.IsMine;
            CameraPlayer.GetComponent<AudioListener>().enabled = photonView.IsMine;
            CanvasVRPlayer.enabled = photonView.IsMine;
            gunVrInstance = Instantiate(gunVr, transform.position + Vector3.up,Quaternion.identity);

            CameraPlayer.GetComponent<TrackedPoseDriver>().enabled = photonView.IsMine;
            //add to list to synchronized
            SynchronizedChildTransform.Add(gunVrInstance.transform);
            SynchronizedChildTransform.Add(_capsuleCollider.transform);
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
        _capsuleCollider.center = CameraPlayer.transform.localPosition - new Vector3(0, 0.5f, 0);
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
        Debug.Log("TakeDamage RPC");
        photonView.RPC("Damage", RpcTarget.AllViaServer,1);
    }
    
    [PunRPC]
    public void Damage(int damage)
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
        Head.SetActive(false);
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
        
        Head.SetActive(true);
        _locomotionProvider.enabled = true;
            
        //TODO Rescale shield
    }
    
    #region Photon

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(Head.activeSelf);
            
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
            Head.SetActive((bool)stream.ReceiveNext());
            
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
