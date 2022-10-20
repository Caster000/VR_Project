using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class WarmupManager : MonoBehaviour
{
    public Canvas warmupCanvas;
    public TextMeshProUGUI warmupText;
    public TextMeshProUGUI loadText;
    
    // Start is called before the first frame update
    void Start()
    {
        warmupCanvas.gameObject.SetActive(true);
        warmupCanvas.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
        warmupText.fontSize = 45;
        warmupText.text = "Warmup";
        loadText.text = "Press X when you are ready";
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ExitGames.Client.Photon.Hashtable serverConfig = PhotonNetwork.CurrentRoom.CustomProperties;

                warmupCanvas.gameObject.SetActive(false);
                Debug.Log(serverConfig);
                SceneManager.LoadScene((string)(serverConfig["SceneToLoad"]));

                //ChangeScene((string)serverConfig["SceneToLoad"]);


            }
        }
            
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(name);
    }
}
