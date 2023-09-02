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
    public TextMeshProUGUI waveCounter;
    public float timeToSpawn = 5f;
    public Terrain terrain;
    [HideInInspector]
    public int coins = 0;
    public int maxEnemyKillCount = 10;
    [HideInInspector]
    public int enemyKillCount = 0;
    public int waveCount;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI towerText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI towerUpgradesPurchased;
    public TextMeshProUGUI healthUpgradesPurchased;
    public TextMeshProUGUI damageUpgradesPurchased;
    public TextMeshProUGUI towerUpgradeIndicator;
    public TextMeshProUGUI healthUpgradeIndicator;
    public TextMeshProUGUI damageUpgradeIndicator;
    public int[] towerHealthIncrements = { 100, 105, 115, 130, 150, 175, 250};
    public int towerHealthIndex = 0;
    public int[] towerPrice = { 1, 10, 20, 30, 40, 50, 60 }; 
    public int[] healthIncrements = {100, 105, 115, 130, 150, 175, 250};
    public int healthIndex = 0;
    public int[] healthPrice = { 1, 10, 20, 30, 40, 50, 60 };
    public int[] damageIncrements = { 40, 45, 50, 60, 75, 90, 100};
    public int damageIndex = 0;
    public int[] damagePrice = { 1, 10, 20, 30, 40, 50, 60 };
    public GameObject panel;
    public PlayerBehavior playerBehavior;
    public PlayerHealthBarBehavior playerSlider;
    public RaycastShoot raycastShoot;
    float time = 0;
    float towerHealth = 100;
    TextMeshProUGUI text;
    bool isFirstSpawn = true; 
    bool multiplayer;
    // Start is called before the first frame update
    void Start()
    {
        coins = 5000;
        multiplayer = PlayerPrefs.GetInt("IsMultiplayer") == 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        text = GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>();
        if (!multiplayer)
        {
            Debug.Log("Is not multiplayer!");
            time = 5;
            isFirstSpawn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehavior == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            Debug.Log("playerBehavior assigned");
            playerBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
        }
        if (raycastShoot == null)
        {
            GameObject gunObject = GameObject.FindGameObjectWithTag("Gun");
            if (gunObject != null)
            {
                raycastShoot = gunObject.GetComponent<RaycastShoot>();
            }
        }
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
            Debug.Log("Two players connected, spawn enemy");
            SpawnEnemy();
            isFirstSpawn = false;
        }
    }
    public void SpawnEnemy()
    {
        Vector3 terrainSize = terrain.terrainData.size; 
        Vector3 terrainPos = terrain.transform.position;
        Bounds planeBounds = new(terrainPos + terrainSize / 2, terrainSize);
        Vector3 randomPos = new(Random.Range(planeBounds.min.x, planeBounds.max.x), 0, Random.Range(planeBounds.min.z, planeBounds.max.z));
        Vector3 randomPos2 = new(Random.Range(planeBounds.min.x, planeBounds.max.x), 0, Random.Range(planeBounds.min.z, planeBounds.max.z));
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newEnemy = PhotonNetwork.Instantiate(towerEnemy.name, randomPos, Quaternion.identity);
            GameObject newEnemy2 = PhotonNetwork.Instantiate(playerEnemy.name, randomPos2, Quaternion.identity);
            newEnemy.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
            newEnemy2.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
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
        SceneManager.LoadScene("End Game");
    }
    public void NextWave()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        maxEnemyKillCount = waveCount * 3 + 10;
        enemyKillCount = 0;
        timeToSpawn /= 1.5f;
        timeToSpawn++;
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("FreezeGameForPlayers", RpcTarget.All);
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            Destroy(coin);
            coins++;
        }
        Debug.Log("Next wave called"); 
    }
    [PunRPC]
    public void DestroyEnemy(string enemyID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // This will destroy the enemy for all clients.
            PhotonNetwork.Destroy(PhotonView.Find(int.Parse(enemyID)).gameObject);
        }
    }
    public void TowerHealthButtonHit()
    {
        // when button is hit
        if (towerHealthIndex <= towerHealthIncrements.Length - 1 && coins >= towerPrice[towerHealthIndex])
        {
            coins -= towerPrice[towerHealthIndex];
            towerText.text = "Upgrade! ($" + towerPrice[towerHealthIndex] + ")";
            towerUpgradesPurchased.text = (towerHealthIndex + 1) + "/" + towerHealthIncrements.Length;
            if (towerHealthIndex == towerHealthIncrements.Length - 1)
            {
                Debug.Log(towerHealthIndex);
                towerUpgradeIndicator.text = "" + towerHealthIncrements[towerHealthIndex];
            }
            else
            {
                towerHealthIndex++;
                towerUpgradeIndicator.text = towerHealthIncrements[towerHealthIndex] + " -> " + towerHealthIncrements[towerHealthIndex + 1];
            }
        }
        Debug.Log(towerHealthIndex);
        towerHealth = towerHealthIncrements[towerHealthIndex];
        healthbar.SetMaxHealth(towerHealth);
        
        // increase health bar size
        // screen shake triggered
    }
    public void HealthButtonHit()
    {
        if (healthIndex <= healthIncrements.Length - 1 && coins >= healthPrice[healthIndex])
        {
            coins -= healthPrice[healthIndex];
            healthText.text = "Upgrade! ($" + healthPrice[healthIndex] + ")";
            healthUpgradesPurchased.text = (healthIndex + 1) + "/" + healthIncrements.Length;
            if (healthIndex == healthIncrements.Length - 1)
            {
                healthUpgradeIndicator.text = "" + healthIncrements[healthIndex];
            }
            else
            {
                healthIndex++;
                healthUpgradeIndicator.text = healthIncrements[healthIndex] + " -> " + healthIncrements[healthIndex + 1];
            }
        }
        Debug.Log(healthIndex);
        playerBehavior.maxPHealth = healthIncrements[healthIndex];
        playerSlider.SetMaxHealth(playerBehavior.maxPHealth);
        playerBehavior.pHealth = healthIncrements[healthIndex];
    }
    public void DamageButtonHit()
    {
        if (damageIndex <= damageIncrements.Length - 1 && coins >= damagePrice[damageIndex])
        {
            coins -= damagePrice[damageIndex];
            damageText.text = "Upgrade! ($" + damagePrice[damageIndex] + ")";
            damageUpgradesPurchased.text = (damageIndex + 1) + "/" + damageIncrements.Length;
            if (damageIndex == damageIncrements.Length - 1)
            {
                damageUpgradeIndicator.text = "" + damageIncrements[damageIndex];
            }
            else
            {
                damageIndex++;
                damageUpgradeIndicator.text = damageIncrements[damageIndex] + " -> " + damageIncrements[damageIndex + 1];
            }
        }
        Debug.Log(damageIndex);
        raycastShoot.gunDamage = damageIncrements[damageIndex];
    }
    public void StartNextWave()
    {
        panel.SetActive(false);
        StartCoroutine(ShowText());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    IEnumerator ShowText()
    {
        waveCounter.gameObject.SetActive(false);
        waveCount++;
        Debug.Log("text shown");
        waveText.text = "Wave: " + waveCount;
        waveText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        waveText.gameObject.SetActive(false);
        Time.timeScale = 1;
        waveCounter.text = "Wave: " + waveCount;
        waveCounter.gameObject.SetActive(true);

    }
    [PunRPC]
    void FreezeGameForPlayers()
    {
        Time.timeScale = 0;
        panel.SetActive(true);
    }
}
