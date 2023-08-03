using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SingleOrMulti : MonoBehaviour
{
    public bool multiplayer = true;

    public void SingleplayerNav()
    {
        PhotonNetwork.LoadLevel("Loading");
        multiplayer = false;
    }
    public void MultiplayerNav()
    {
        PhotonNetwork.LoadLevel("Loading");
        multiplayer = true;
    }
}
