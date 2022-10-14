using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;
using Photon.Pun;

public class GunBeahviour : MonoBehaviour
{
    
    public float damage = 1f;

    public float range = 50f;

    public float fireRate = 1000000f;

    public Canvas crosshair;

    public ParticleSystem muzzleFlash;

    public AudioClip gunShot;
    public AudioClip gunReload;

    public GameObject bullet;

    public GameObject bulletSpawn;

    private float timetoFire = 0f;
    public bool isReloaded = true;

    public bool isVR;

    
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
            if (!isVR)
            {
                progressBar.fillAmount += 1.0f / waitToFire * Time.deltaTime;
            }
        }
        // if (isVR)return;


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
        if(allowfire && !isVR)
        {
            progressBar.fillAmount = 0f;
        }
    }

    public bool getAllowFire()
    {
        return allowfire;
    }

    public void Shoot()
    {
        if (allowfire)
        {
            allowfire = false;
            muzzleFlash.Play();
            AudioSource.PlayClipAtPoint(gunShot, transform.position);

            GameObject tempBullet = PhotonNetwork.Instantiate("Prefabs/"+bullet.name, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            tempBullet.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * bulletSpeed ;
            tempBullet.layer = gameObject.layer;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);
            }
            isReloaded = false;
        }
    }

    public void ToggleCanvasGun(bool toggle)
    {
        crosshair.gameObject.SetActive(toggle);
    }


}
