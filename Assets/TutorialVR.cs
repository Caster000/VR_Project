using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialVR : MonoBehaviour
{
    [Header("Welcome")]
    public TextMeshProUGUI welcome;
    public TextMeshProUGUI movement;
    bool welcomed = false;
    [Header("GameMechanic")]
    public TextMeshProUGUI winCondition;
    public TextMeshProUGUI capturing;
    public TextMeshProUGUI fire;
    public TextMeshProUGUI kill;
    public GameObject ScientistToKill;




    private float timer = 0f;
    private bool winconditiondone;
    private bool isInstantiate;

    // Start is called before the first frame update
    void Awake()
    {
        welcome.enabled = true;
        movement.enabled = false;
        winCondition.enabled = true;
        capturing.enabled = false;
        fire.enabled = true;
        kill.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 5 && !welcomed)
        {
            welcome.enabled = false;
            movement.enabled = true;
            welcomed = true;
        }

        if(timer >= 10 && !winconditiondone)
        {
            winCondition.enabled = false;
            capturing.enabled = true;
            winconditiondone = true;
        }

        if(timer >= 25 && winconditiondone)
        {
            fire.enabled = false;
            kill.enabled = true;
            if (!isInstantiate)
            {
                ScientistToKill.SetActive(true);
                isInstantiate = true;
            }
                
        }



        
    }
}
