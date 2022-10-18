using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

public class BulletBehaviour : MonoBehaviourPunCallbacks,IPunObservable
{
    public float life = 5f;
    private GameConfig gameConfig;
    private Rigidbody _rigidbody;

    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Destroy(gameObject, life);
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        IPlayer player = hit.GetComponent<IPlayer>();
        if(player != null)
        {
            //TODO uncomment
            // if (hit.layer == gameObject.layer && !gameConfig.friendlyFire)
            //     return;
            player.TakeDamage();
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}