using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ExplosionLifeTime : MonoBehaviour
{

    [SerializeField] private float lifetime = 1.5f;
    private float timeLived = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if(timeLived >= lifetime) PhotonNetwork.Destroy(gameObject);
        timeLived += Time.deltaTime;
    }
}
