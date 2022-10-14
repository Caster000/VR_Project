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

    public Camera gunCam;
    public Camera playerCam;

    public ParticleSystem muzzleFlash;

    public AudioClip gunShot;
    public AudioClip gunReload;

    public GameObject bullet;

    public GameObject bulletSpawn;

    private float timetoFire = 0f;
    public bool isReloaded = true;

    public Animator anim;

    
    float time = 0f;

    
    public Image progressBar;


    private bool allowfire = true;
    public float waitToFire = 1.5f;

    public float bulletSpeed = 10f;

   
 
    
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


}
