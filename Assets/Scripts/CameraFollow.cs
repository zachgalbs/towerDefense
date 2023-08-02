using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        if (transform.parent.GetComponent<PhotonView>().IsMine == false)
        {
            // If this camera does not belong to the local player, disable it.
            GetComponent<Camera>().enabled = false;
        }
    }
    private void FixedUpdate()
    {
        if (target == null)
        {
            Debug.Log("Target is null");
            Debug.Log(gameObject.name);
        }

        Vector3 desiredPosition = transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
