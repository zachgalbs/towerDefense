using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerBehavior : MonoBehaviour
{
    public float speed = 10f;
    public float gravity = 9.8f;
    public float lookSpeed = 2f;
    public float jumpPower = 10f;
    public float pHealth;
    public PlayerHealthBarBehavior playerHealthBar;
    public float maxPHealth = 100;
    public Camera playerCamera;
    bool doubleJump = false;
    int jumpsLeft = 0;
    CharacterController cc;
    GameManager gm;
    AudioSource[] audioSources;
    PhotonView view;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSources = GetComponents<AudioSource>();
        view = GetComponent<PhotonView>();
        playerHealthBar = GameObject.Find("playerHealth").GetComponent<PlayerHealthBarBehavior>();
        playerHealthBar.SetMaxHealth(maxPHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            // player movement
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float speedX = Input.GetAxis("Vertical") * speed;
            float speedY = Input.GetAxis("Horizontal") * speed;
            moveDirection = forward * speedX + right * speedY + moveDirection.y * Vector3.up;
            // jumping
            if (cc.isGrounded)
            {
                jumpsLeft = doubleJump ? 2 : 1;
            }
            if (Input.GetButtonDown("Jump") && (cc.isGrounded || jumpsLeft > 0))
            {
                jumpsLeft--;
                moveDirection.y = doubleJump && jumpsLeft == 0 ? jumpPower * 1.5f : jumpPower;
            }
            if (!cc.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            cc.Move(moveDirection * Time.deltaTime);

            // camera rotation
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            playerHealthBar.SetHealth(pHealth);
        }

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            gm.coins++;
            audioSources[0].Play();
        }
        if (collision.gameObject.name == "jumpBoost")
        {
            Destroy(collision.gameObject);
            doubleJump = true;
            audioSources[1].Play();
        }
    }
    public void PShot(float gunDamage)
    {
        pHealth -= gunDamage * Time.deltaTime;
        Debug.Log(pHealth);
        if (pHealth <= 0)
        {
            gm.EndGame();
        }
    }
}
