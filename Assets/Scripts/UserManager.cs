using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Random = UnityEngine.Random;


public class UserManager : MonoBehaviourPunCallbacks, IPunObservable, IPlayer
{
    public static GameObject UserMeInstance;

    [SerializeField] private List<Transform> spawPoints;
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Canvas CanvasPlayer;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private GameObject BodyPlayer;
    [SerializeField] private GameObject CameraPlayerPrefab;
    
    
    [Header("Gun objects")]
    [SerializeField] private GameObject GunObject;
    [SerializeField] private Animator GunAnim;
    private GunBeahviour gunBeahviour;
    private vThirdPersonCamera _vThirdPersonCamera;

    private int currentHealth;
    private float respawnTime;
    private bool spawned;

    private GameConfig gameConfig;
    private GameObject cameraPlayer;
    private Image canvasImage;
    
    [Header("My Components")]
    private Rigidbody _rigidbody;
    private Collider _collider;
    private vThirdPersonInput _thirdPersonInput;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;
        if (photonView.IsMine)
        {
            Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
            UserMeInstance = gameObject;
            cameraPlayer = Instantiate(CameraPlayerPrefab);
            cameraPlayer.SetActive(photonView.IsMine);
            _vThirdPersonCamera = cameraPlayer.GetComponent<vThirdPersonCamera>();
            _vThirdPersonCamera.SetMainTarget(transform);
        }
        _thirdPersonInput = GetComponent<vThirdPersonInput>();
        _thirdPersonInput.enabled = photonView.IsMine;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = !photonView.IsMine;
        _collider = GetComponent<Collider>();
        CanvasPlayer.gameObject.SetActive(photonView.IsMine);
        canvasImage = CanvasPlayer.GetComponent<Image>();
        if (GunObject.GetComponent<GunBeahviour>())
        {
            gunBeahviour = GunObject.GetComponent<GunBeahviour>();
        }

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

        // Gun inputs
        if (Input.GetButtonDown("Fire1") 
            && gunBeahviour.getAllowFire() 
            && photonView.IsMine)
        {
            gunBeahviour.Shoot(cameraPlayer);
        }
        
        if(Input.GetButtonDown("Fire2"))
        {
            Aim();

        }
        if (Input.GetButtonUp("Fire2"))
        {
            ResetCam(); 
        }

    }

    public void TakeDamage()
    {
        Debug.Log("Damage receive :"+photonView.InstantiationId);
        Healthbar.value = --currentHealth;
        if (currentHealth <= 0)
        {
            PrepareRespwan();
            return;
        }
    }
    
    public void Aim()
    {
        Debug.Log("Aim pressed");
        GunAnim.SetBool("isAiming", true);
        _vThirdPersonCamera.rightOffset = 0.3f;
        _vThirdPersonCamera.defaultDistance = 0.5f;
        _vThirdPersonCamera.height = 1.6f;
        
    }

    public void ResetCam()
    {
        Debug.Log("Aim released");
        GunAnim.SetBool("isAiming", false);
        _vThirdPersonCamera.rightOffset = 0f;
        _vThirdPersonCamera.defaultDistance = 2.5f;
        _vThirdPersonCamera.height = 1.4f;

    }

    private void PrepareRespwan()
    {
        // Canvas update
        spawned = false;
        canvasImage.color = Color.black;
        Healthbar.gameObject.SetActive(false);
        // Hide the body and colliders  
        _collider.enabled = false;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        BodyPlayer.SetActive(false);
        GunObject.SetActive(false);
       _thirdPersonInput.enabled = false;
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
        canvasImage.color = Color.clear;
        Healthbar.value = currentHealth = gameConfig.LifeNumber;
        spawned = true;
            
        //Reset player health, position and shield
        _collider.enabled = true;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        BodyPlayer.SetActive(true);
        GunObject.SetActive(true);
        _thirdPersonInput.enabled = true;
    }

    #region Photon

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(BodyPlayer.activeSelf);
            stream.SendNext(GunObject.activeSelf);
            stream.SendNext(GetComponent<Collider>().enabled);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
            BodyPlayer.SetActive((bool)stream.ReceiveNext());
            GunObject.SetActive((bool)stream.ReceiveNext());
            GetComponent<Collider>().enabled = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}
