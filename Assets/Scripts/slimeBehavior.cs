using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SlimeBehavior : MonoBehaviour
{
    public float enemyDistance;
    public int eHealth = 100;
    public int eDamage;
    public GameObject coin;
    public GameObject enemy;
    GameManager gameManager;
    NavMeshAgent agent;
    Animator animator;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);
        foreach (GameObject p in players)
        {
            Debug.Log(p.name);
            if (p.scene.isLoaded && p.activeInHierarchy) // Check if the object is part of a loaded scene and is active
            {
                if (player == null)
                {
                    Debug.Log(p);
                    player = p;
                }
                else
                {
                    Vector3 d1 = player.transform.position - transform.position;
                    Vector3 d2 = p.transform.position - transform.position;
                    if (d1.magnitude > d2.magnitude)
                    {
                        player = p;
                    }
                }
            }
        }
    }
        // Update is called once per frame
        void Update()
    {
        Debug.Log("Found player: " + player.name + " at " + player.transform.position);
        if (player != null)
        {
            transform.LookAt(player.transform);
            // starts following the player
            agent.SetDestination(player.transform.position);

            if (Vector3.Distance(transform.position, player.transform.position) < enemyDistance)
            {
                animator.SetTrigger("nearPlayer");
                player.GetComponent<PlayerBehavior>().PShot(eDamage);
                GetComponent<NavMeshAgent>().velocity = Vector3.zero;
            }
        }
    }
    public void EShot(int gunDamage)
    {
        eHealth -= gunDamage;
        Debug.Log(eHealth);
        if (eHealth <= 0)
        {
            Instantiate(coin, transform.position, Quaternion.identity);
            gameManager.enemyKillCount++;
            if (gameManager.enemyKillCount >= gameManager.maxEnemyKillCount)
            {
                gameManager.EndGame();
            }
            Destroy(gameObject);
            Die();
        }
    }
    public void Die() // Call this function when the enemy is supposed to die.
    {
        // This will call the DestroyEnemy RPC on the GameManager on all clients.
        PhotonView gameManagerPhotonView = GameObject.Find("GameManager").GetComponent<PhotonView>();
        gameManagerPhotonView.RPC("DestroyEnemy", RpcTarget.All, GetComponent<PhotonView>().ViewID.ToString());
    }

}
