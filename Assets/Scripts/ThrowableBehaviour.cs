using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ThrowableBehaviour : MonoBehaviourPunCallbacks
{

    private bool isThrown;
    private int scientistLayer;
    private Rigidbody throwableRigidbody;

    private XRGrabInteractable grabController;

    private float power = 10.0f;
    private float radius = 5.0f;
    private float upforce = 1.0f;
    // private AudioSource _audioSource;
        
    [SerializeField] private float throwForce = 75f;
    [SerializeField] private ForceMode throwMode = ForceMode.Impulse;
    [SerializeField] GameObject explosionPrefab;


    private void Awake()
    {
        scientistLayer = LayerMask.NameToLayer("Scientist");
        throwableRigidbody = GetComponent<Rigidbody>();
        // _audioSource = GetComponent<AudioSource>();
        
        power = GameConfigLoader.Instance.gameConfig.ExplosionPower > 0.0f ? GameConfigLoader.Instance.gameConfig.ExplosionPower : power;
        radius = GameConfigLoader.Instance.gameConfig.RadiusExplosion > 0.0f ? GameConfigLoader.Instance.gameConfig.RadiusExplosion : radius;
        upforce = GameConfigLoader.Instance.gameConfig.ExplosionUpforce > 0.0f ? GameConfigLoader.Instance.gameConfig.ExplosionUpforce : upforce;
        grabController = GetComponent<XRGrabInteractable>();
    }

    public void Throw()
    {
        isThrown = true;
        grabController.interactionManager.CancelInteractableSelection(grabController);
        throwableRigidbody.AddForce(grabController.transform.forward * throwForce, throwMode);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
            Explode();
        // photonView.RPC("Explode", RpcTarget.AllViaServer);
    }

    // [PunRPC]
    private void Explode()
    {
        // _audioSource.Play();
        Transform objectTransform = gameObject.transform;
        Vector3 position = objectTransform.position;

        // Add particles
        GameObject g = PhotonNetwork.Instantiate("Prefabs/"+explosionPrefab.name, position, objectTransform.rotation);

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
        
        PhotonNetwork.Destroy(gameObject);
    }

}
