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
        foreach (GameObject p in players)
        {
            if (p.scene.isLoaded && p.activeInHierarchy) // Check if the object is part of a loaded scene and is active
            {
                if (player == null)
                {
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
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // Ignore differences in height
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(-10, rotation.eulerAngles.y, 0);

            // starts following the player
            agent.SetDestination(player.transform.position);

            if (Vector3.Distance(transform.position, player.transform.position) < enemyDistance)
            {
                animator.SetBool("nearPlayer", true);
                player.GetComponent<PlayerBehavior>().PShot(eDamage);
                GetComponent<NavMeshAgent>().velocity = Vector3.zero;
            }
            else
            {
                animator.SetBool("nearPlayer", false);
            }
        }
    }
    public void EShot(int gunDamage)
    {
        eHealth -= gunDamage;
        if (eHealth <= 0)
        {
            Instantiate(coin, transform.position, Quaternion.identity);
            gameManager.enemyKillCount++;
            if (gameManager.enemyKillCount >= gameManager.maxEnemyKillCount)
            {
                gameManager.NextWave();
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
