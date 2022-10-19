using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour,IPunObservable
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
                if (hit.layer == gameObject.layer)
                    return;
                player.TakeDamage();
            
            }
            ResizeShield _resizeShield = hit.GetComponent<ResizeShield>();
            if (_resizeShield)
            {
                Debug.Log("Resize shield");
                if (gameObject.layer == 7) //todo 7
                {
                    _resizeShield.ResizeRPC(new Vector3(0.1f, 0.1f, 0.1f));
                }
            }
            Destroy(gameObject);
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}