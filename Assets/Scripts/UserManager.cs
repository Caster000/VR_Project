using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
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
    public GameManager gameManager;
    private GameObject cameraPlayer;
    private Image canvasImage;
    
    [Header("My Components")]
    private Rigidbody _rigidbody;
    private Collider _collider;
    private vThirdPersonInput _thirdPersonInput;

    [Header("Sound")]
    public AudioClip damage;
    public AudioClip death;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;
        _thirdPersonInput = GetComponent<vThirdPersonInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        canvasImage = CanvasPlayer.GetComponent<Image>();

        if (NetworkManager.isMulti)
        {
            if (photonView.IsMine)
            {
            Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
            UserMeInstance = gameObject;
            CameraPlayerInstanciate();
            _thirdPersonInput.enabled = photonView.IsMine;
            _rigidbody.isKinematic = !photonView.IsMine;
            CanvasPlayer.gameObject.SetActive(photonView.IsMine);
            }
        }
        else
        {
            CameraPlayerInstanciate();
        }
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
        if (!photonView.IsMine && NetworkManager.isMulti) return;

        if (respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            double b = Mathf.RoundToInt(respawnTime);
            TimerText.text = "Respawn in " + b.ToString() + "s";
        }

        if (respawnTime <= 0 && !spawned)
        {
            Respawn();
        }

        // Gun inputs
        if ((Input.GetButtonDown("Fire1") 
            && gunBeahviour.getAllowFire() 
            && photonView.IsMine)
            ||
            (Input.GetButtonDown("Fire1") 
             && gunBeahviour.getAllowFire() 
             && !NetworkManager.isMulti)
            )
        {
            gunBeahviour.Shoot(cameraPlayer);
        }
        
        if((Input.GetButtonDown("Fire2") && photonView.IsMine)
           ||
           (Input.GetButtonDown("Fire2") && !NetworkManager.isMulti))
        {
            Aim();

        }
        if((Input.GetButtonUp("Fire2") && photonView.IsMine)
           ||
           (Input.GetButtonUp("Fire2") && !NetworkManager.isMulti))
        {
            ResetCam(); 
        }

        if (Input.GetButton("Leave")&& !NetworkManager.isMulti)
        {
            SceneManager.LoadScene("Menu");
        }

    }

    private void CameraPlayerInstanciate()
    {
        cameraPlayer = Instantiate(CameraPlayerPrefab);
        cameraPlayer.SetActive(true);
        _vThirdPersonCamera = cameraPlayer.GetComponent<vThirdPersonCamera>();
        _vThirdPersonCamera.SetMainTarget(transform);
    }

    public void TakeDamage()
    {
        Debug.Log("Damage receive :"+photonView.InstantiationId);
        Healthbar.value = --currentHealth;
        AudioSource.PlayClipAtPoint(damage, transform.position);
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
        AudioSource.PlayClipAtPoint(death, transform.position);
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
        if (stream.IsWriting)
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
