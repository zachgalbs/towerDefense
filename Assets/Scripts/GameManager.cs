using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject towerEnemy;
    public GameObject playerEnemy;
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
    bool multiplayer;
    // Start is called before the first frame update
    void Start()
    {
        multiplayer = PlayerPrefs.GetInt("IsMultiplayer") == 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        text = GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>();
        if (!multiplayer)
        {
            Debug.Log(isFirstSpawn);
            time = 5;
            isFirstSpawn = false;
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
        else if (time < 0)
        {
            SpawnEnemy();
            time = timeToSpawn;
        }
        text.text = "Coins: " + coins;
    }
    void SpawnEnemyIfMultiplayer()
    {
        // If the game is meant to be multiplayer and there are two people connected
        if (multiplayer && PhotonNetwork.CurrentRoom.PlayerCount == 2)
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
        Vector3 randomPos2 = new Vector3(Random.Range(planeBounds.min.x, planeBounds.max.x), 0, Random.Range(planeBounds.min.z, planeBounds.max.z));
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newEnemy = PhotonNetwork.Instantiate(towerEnemy.name, randomPos, Quaternion.identity);
            GameObject newEnemy2 = PhotonNetwork.Instantiate(playerEnemy.name, randomPos2, Quaternion.identity);
            newEnemy.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
            newEnemy2.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
            //towerEnemy.GetComponent<EnemyBehavior>().eHealth = 100;
            //playerEnemy.GetComponent<EnemyBehavior>().eHealth = 100;
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
