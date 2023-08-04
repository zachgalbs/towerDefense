using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfSingleplayer : MonoBehaviour
{
    public GameObject joinButton;
    public GameObject joinInput;
    public TextMesh title;
    private void Start()
    {
        if (PlayerPrefs.GetInt("IsMultiplayer") == 0)
        {
            Destroy(joinButton);
            Destroy(joinInput);
            title.text = "Create";
        }
    }
}
