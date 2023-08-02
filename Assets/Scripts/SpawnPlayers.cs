using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

        if (player.GetComponent<PhotonView>().IsMine)
        {
            GameObject playerCamera = player.transform.GetChild(0).gameObject;
            CameraFollow cameraFollowScript = playerCamera.GetComponent<CameraFollow>();
            cameraFollowScript.target = player.transform;
        }
    }
}
