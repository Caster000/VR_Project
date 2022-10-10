using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UserManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawPoints;
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Canvas CanvasPlayer;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private GameObject BodyPlayer;
    private int currentHealth;
    private float respawnTime;
    private bool spwaned;
    
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
        //TODO to remove, Debug to test Health
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage();
        }

        if(respawnTime>0)     
        {         
            respawnTime -= Time.deltaTime;   
            double b = Mathf.RoundToInt(respawnTime); 
            TimerText.text = "Respawn in "+b.ToString ()+"s";
        }

        if (respawnTime <= 0 && !spwaned)
        {
            //Reset player health, position and shield
            TimerText.text = "";
            TimerText.enabled = false;
            Healthbar.gameObject.SetActive(true);
            CanvasPlayer.GetComponent<Image>().color = Color.clear;
            Healthbar.value = currentHealth = gameConfig.LifeNumber;
            spwaned = true;
            // transform.gameObject.GetComponent<Collider>().enabled = false;
            BodyPlayer.SetActive(true);
            //TODO Rescale shield
        }
    }

    public void TakeDamage()
    {
        Healthbar.value = --currentHealth;
        if (currentHealth <= 0)
        {
            spwaned = false;
            CanvasPlayer.GetComponent<Image>().color = Color.black;
            Healthbar.gameObject.SetActive(false);
            
            // transform.gameObject.GetComponent<Collider>().enabled = false; //TODO remove collider ??
            BodyPlayer.SetActive(false);
            
            Transform transform_spawn = spawPoints[Random.Range(0, spawPoints.Count)];
            transform.position = transform_spawn.position;
            TimerText.enabled = true;
            respawnTime = gameConfig.RespawnTime;
            return;
        }

    }
}
