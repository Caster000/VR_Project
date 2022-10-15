using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;

public class GunBeahviour : MonoBehaviour
{
    
    public float damage = 1f;

    public float range = 50f;

    public float fireRate = 1000000f;

    public Camera playerCam;

    public ParticleSystem muzzleFlash;

    public AudioClip gunShot;
    public AudioClip gunReload;

    public GameObject bullet;

    public GameObject bulletSpawn;

    private float timetoFire = 0f;
    public bool isReloaded = true;

    public Animator anim;
    public GameObject PlayerCam;
    
    float time = 0f;

    
    public Image progressBar;

    public vThirdPersonCamera camScript;

    private bool allowfire = true;
    public float waitToFire = 1.5f;

    public float bulletSpeed = 10f;


    private void Awake()
    {
       camScript = PlayerCam.GetComponent<vThirdPersonCamera>();
    }

    void Start()
    {
     

    }

    
    void Update()
    {  
        
        if (!allowfire)
        {
            timetoFire += Time.deltaTime;
            progressBar.fillAmount += 1.0f / waitToFire * Time.deltaTime;
        }

        if (timetoFire >= waitToFire - 0.6f && !isReloaded)
        {
            AudioSource.PlayClipAtPoint(gunReload, transform.position);
            isReloaded = true;
        }

        if (timetoFire >= waitToFire)
        {
            
            timetoFire = 0f;
            allowfire = true;

        }
        if(allowfire)
        {
            progressBar.fillAmount = 0f;
        }
        
        
        if(Input.GetButtonDown("Fire1") && allowfire)
        {
        
            Shoot();
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

    void Shoot()
    {
        
        allowfire = false;
        muzzleFlash.Play();
        AudioSource.PlayClipAtPoint(gunShot, transform.position);
        var  tempBullet = Instantiate(bullet, bulletSpawn.transform.position, playerCam.transform.rotation);
        tempBullet.GetComponent<Rigidbody>().velocity = playerCam.transform.forward * bulletSpeed ;
        RaycastHit hit;
        if(Physics.Raycast(bulletSpawn.transform.position, bulletSpawn.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

        }
        isReloaded = false;




    }

    public void Aim()
    {
        Debug.Log("Aim pressed");
        anim.SetBool("isAiming", true);
        camScript.rightOffset = 0.3f;
        camScript.defaultDistance = 0.5f;
        camScript.height = 1.6f;
        
    }

    public void ResetCam()
    {
        Debug.Log("Aim released");
        anim.SetBool("isAiming", false);
        camScript.rightOffset = 0f;
        camScript.defaultDistance = 2.5f;
        camScript.height = 1.4f;

    }
   

}
