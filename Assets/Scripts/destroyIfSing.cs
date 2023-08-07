using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DestroyIfSingleplayer : MonoBehaviour
{
    public GameObject joinButton;
    public GameObject joinInput;
    public TMP_Text title;
    private void Start()
    {
        if (PlayerPrefs.GetInt("IsMultiplayer") == 0)
        {
            joinButton.SetActive(false);
            joinInput.SetActive(false);
            title.text = "Create";
        }
    }
}
