using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ThrowableBehaviour : MonoBehaviour
{

    private bool isThrown;
    private int scientistLayer;
    private Rigidbody throwableRigidbody;

    private XRGrabInteractable grabController;

    private float power = 10.0f;
    private float radius = 5.0f;
    private float upforce = 1.0f;

    [SerializeField] private float throwForce = 75f;
    [SerializeField] private ForceMode throwMode = ForceMode.Impulse;
    [SerializeField] GameObject explosionPrefab;

    private void Awake()
    {
        scientistLayer = LayerMask.NameToLayer("Scientist");
        throwableRigidbody = GetComponent<Rigidbody>();

        power = GameConfigLoader.Instance.gameConfig.ExplosionPower > 0.0f ? GameConfigLoader.Instance.gameConfig.ExplosionPower : power;
        radius = GameConfigLoader.Instance.gameConfig.RadiusExplosion > 0.0f ? GameConfigLoader.Instance.gameConfig.RadiusExplosion : radius;
        upforce = GameConfigLoader.Instance.gameConfig.ExplosionUpforce > 0.0f ? GameConfigLoader.Instance.gameConfig.ExplosionUpforce : upforce;
        grabController = GetComponent<XRGrabInteractable>();
    }

    public void Throw()
    {
        // Debug.Log(throwableRigidbody.velocity.magnitude);
        // isThrown = throwableRigidbody.velocity.magnitude > minimumThrownVelocity;
        isThrown = true;
        grabController.interactionManager.CancelInteractableSelection(grabController);
        throwableRigidbody.AddForce(grabController.transform.forward * throwForce, throwMode);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown) 
            Explode();
    }

    private void Explode()
    {
        Debug.Log("Explode");
        Transform objectTransform = gameObject.transform;
        Vector3 position = objectTransform.position;

        // Add particles
        GameObject g = Instantiate(explosionPrefab, position, objectTransform.rotation);
        Destroy(g, 3);
        
        // Push everything around & damage players
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        GameObject currentTreatedObject;
        Rigidbody currentRigidbody;
        foreach(Collider collider in hitColliders)
        {
            currentTreatedObject = collider.gameObject;
            currentRigidbody = currentTreatedObject.GetComponent<Rigidbody>();

            if(currentRigidbody)
                currentRigidbody.AddExplosionForce(power, position, radius, upforce, ForceMode.Impulse);

            if (currentTreatedObject.layer == scientistLayer) //TODO friendly fire ?
                currentTreatedObject.GetComponent<UserManager>().TakeDamage();
        }
        
        Destroy(gameObject);
    }

}
