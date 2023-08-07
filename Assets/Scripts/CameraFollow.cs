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
}
