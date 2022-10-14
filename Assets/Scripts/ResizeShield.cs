using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeShield : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
//TODO Put bullet Layer
        if (collision.gameObject.layer == 6)
        {
            gameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            
        }

        if (gameObject.transform.localScale.x < 0.5)
        {
            gameObject.SetActive(false);
        }
    }

    public void ResetShield()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.SetActive(true);
    }
    
}
