using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderDelay : MonoBehaviour
{
    public TrailRenderer TrailRenderer;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            TrailRenderer.enabled = true;
        }

        if (timer < 0)
        {
            TrailRenderer.enabled = false;
        }
        
    }

    public void StartTimer()
    {
        timer = 1f;
    }
    
    
}
