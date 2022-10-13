using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float life = 3f;
    private GameConfig gameConfig;

    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Destroy(gameObject, life);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;

        IPlayer player = hit.GetComponent<IPlayer>();
        if(player != null)
        {
            if (hit.layer == gameObject.layer && !gameConfig.friendlyFire)
                return;
            player.TakeDamage();
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}