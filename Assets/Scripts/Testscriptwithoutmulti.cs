using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//!DELETE THE FILE, Only for test purposes 
public class Testscriptwithoutmulti : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("x"))
        {
            Debug.Log("x");
            if (player.gameObject.layer == 7)
            {
                gameManager.ScientistScore++;
            }
            else if (player.gameObject.layer == 8)
            {
                gameManager.VirusScore++;
            }
        }
        if (Input.GetKeyUp("w"))
        {
            Debug.Log("w");

            if (player.gameObject.layer == 7)
            {
                gameManager.nbContaminatedAreaByScientist++;
            }
            else if (player.gameObject.layer == 8)
            {
                gameManager.nbContaminatedAreaByVirus++;
            }
        }
    }
}
