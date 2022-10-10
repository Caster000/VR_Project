using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UserManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawPoints;
    [SerializeField] Slider Healthbar;

    private int currentHealth;

    private GameConfig gameConfig;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage()
    {
        Healthbar.value = --currentHealth;
        if (currentHealth <= 0)
        {
            Transform transform_spawn = spawPoints[Random.Range(0, spawPoints.Count)];
            transform.position = transform_spawn.position;
            
            //Reset player health, position and shield
            currentHealth = gameConfig.LifeNumber;
            // gameConfig.RespawnTime;//TODO
            //TODO Rescale shield
            return;
        }

    }
}
