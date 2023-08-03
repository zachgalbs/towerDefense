using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject enemy;
    public GameObject tower;
    public SingleOrMulti singleOrMulti;
    public HealthBar healthbar;
    public float timeToSpawn = 5f;
    public Terrain terrain;
    [HideInInspector]
    public int coins = 0;
    public int maxEnemyKillCount = 10;
    [HideInInspector]
    public int enemyKillCount = 0;
    float time = 0;
    float towerHealth = 100;
    TextMeshProUGUI text;
    bool isFirstSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        text = GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>();
        singleOrMulti = GameObject.Find("singleOrMulti").GetComponent<SingleOrMulti>();
        if (singleOrMulti.multiplayer == false)
        {
            time = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (isFirstSpawn)
        {
            SpawnEnemyIfMultiplayer();
            time = timeToSpawn;
        }
        if (time < 0)
        {
            SpawnEnemy();
            time = timeToSpawn;
        }
        text.text = "Coins: " + coins;
    }
    void SpawnEnemyIfMultiplayer()
    {
        // If the game is meant to be multiplayer and there are two people connected
        if (singleOrMulti.multiplayer = true && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            SpawnEnemy();
            isFirstSpawn = false;
        }
    }
    public void SpawnEnemy()
    {
        Vector3 terrainSize = terrain.terrainData.size; 
        Vector3 terrainPos = terrain.transform.position;
        Bounds planeBounds = new Bounds(terrainPos + terrainSize / 2, terrainSize);
        Vector3 randomPos = new Vector3(Random.Range(planeBounds.min.x, planeBounds.max.x), 0, Random.Range(planeBounds.min.z, planeBounds.max.z));

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newEnemy = PhotonNetwork.Instantiate(enemy.name, randomPos, Quaternion.identity);
            newEnemy.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
            enemy.GetComponent<EnemyBehavior>().eHealth = 100;
        }
    }
    public void TShot(float eDamage)
    {
        towerHealth -= eDamage * Time.deltaTime;
        healthbar.SetHealth(towerHealth);
        if (towerHealth <= 0)
        {
            Destroy(tower);
            EndGame();
        }
    }
    public void EndGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(3);
    }
    [PunRPC]
    public void DestroyEnemy(string enemyID)
    {
        // This will destroy the enemy for all clients.
        PhotonNetwork.Destroy(PhotonView.Find(int.Parse(enemyID)).gameObject);
    }
}
