using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicHand : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    private Collider[] handColliders;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = (target.position - transform.position) / Time.deltaTime;
        Quaternion rotationDiff = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDiff.ToAngleAxis(out float angleInDeg, out Vector3 rotationAxis);

        Vector3 rotationDiffInDeg = angleInDeg * rotationAxis;
        rb.angularVelocity = (rotationDiffInDeg * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }


    public void GrabStop(float delay)
    {
        DisableHandCollider();
        Invoke(nameof(EnableHandCollider), delay);

    }

   
    public void EnableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = true;
        }
        
    }

    public void DisableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
        
    }
}