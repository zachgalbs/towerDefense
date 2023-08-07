using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class TurtleBehavior : MonoBehaviour
{
    public float enemyDistance;
    public int eHealth = 100;
    public int eDamage;
    public GameObject coin;
    public GameObject enemy;
    public Transform tower;
    GameManager gameManager;
    NavMeshAgent agent;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        tower = GameObject.Find("Tower").GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        if (tower != null)
        {
            Vector3 direction = tower.transform.position - transform.position;
            direction.y = 0; // Ignore differences in height
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(-10, rotation.eulerAngles.y, 0);
            agent.SetDestination(tower.position);

            if (Vector3.Distance(transform.position, tower.position) < enemyDistance)
            {
                animator.SetTrigger("nearTower");
                gameManager.GetComponent<GameManager>().TShot(eDamage);
                GetComponent<NavMeshAgent>().velocity = Vector3.zero;
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
