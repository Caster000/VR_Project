using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UserManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawPoints;
    [SerializeField] private Slider Healthbar;
    
    
    private int currentHealth;
    
    private int maxHealth;

    // private GameConfig gameConfig;//TODO
    
    // Start is called before the first frame update//TODO
    void Awake()
    {
        //gameConfig = gameConfig.Instance; //TODO
        Healthbar.maxValue =
            Healthbar.value =
                maxHealth =
                    currentHealth = 10; //gameConfig.LifeNumber;//TODO

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (--currentHealth <= 0)
        {
            Transform transform_spawn = spawPoints[Random.Range(0, spawPoints.Count)];
            transform.position = transform_spawn.position;
            
            //Reset player health, position and shield
            // currentHealth = gameConfig.LifeNumber;//TODO
            // gameConfig.RespawnTime;//TODO
            //TODO Rescal shield
            return;
        }
        Healthbar.value = currentHealth;
    }
}
