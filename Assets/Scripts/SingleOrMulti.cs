using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SingleOrMulti : MonoBehaviour
{
    public void SingleplayerNav()
    {
        PhotonNetwork.LoadLevel("Loading");
        PlayerPrefs.SetInt("IsMultiplayer", 0);
        PlayerPrefs.Save();
    }
    public void MultiplayerNav()
    {
        PhotonNetwork.LoadLevel("Loading");
        PlayerPrefs.SetInt("IsMultiplayer", 1);
        PlayerPrefs.Save();
    }
}
