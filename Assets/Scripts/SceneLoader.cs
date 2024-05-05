using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    private void Awake()
    {
        instance = this;
    }

    public Animator transition;
    public float transitionTime = 1f;

    public GameObject playerPrefab;
    public GameObject playerStartingPosition;

    public GameObject[] enemyPrefabs;
    private List<string> enemyNames = new List<string>();
    private List<Vector3> enemyStartingPositions = new List<Vector3>();
    private List<Quaternion> enemyStartingRotations = new List<Quaternion>();
    private List<int> enemyStartingHealthValues = new List<int>();
    private List<int> enemyDifficultyValues = new List<int>();
    private List<EnemyReward> enemyRewards = new List<EnemyReward>();    

    public GameObject exitCostPrefab;
    public Vector3 exitCostStartingPosition = new Vector3(0f, 10f, 0f);
    public Quaternion exitCostStartingRotation = Quaternion.identity;

    public GameObject tvcPrefab;

    public Camera canvasCamera;

    void Start()
    {        
        InstantiatePlayer();
        InstantiateEnemy();
        InstantiateExitCost();        
        InstantiateTVC();        
        InstantiateSlotMachine();
        InstantiateMerchant();
    }

    public void InstantiatePlayer()
    {
        GameObject proxyPlayer = GameObject.FindGameObjectWithTag("ProxyPlayer");
        if (proxyPlayer != null)
        {
            Destroy(proxyPlayer);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Instantiate(playerPrefab, playerStartingPosition.transform.position, playerStartingPosition.transform.rotation);                     
        }
        else
        {
            if (PlayerMovement.instance.moveToStartingPosition)
            {
                player.transform.position = playerStartingPosition.transform.position;
                player.transform.rotation = playerStartingPosition.transform.rotation;
                PlayerMovement.instance.moveToStartingPosition = false;
            }
        }

        if (SceneManager.GetActiveScene().name.Contains("Poker") || SceneManager.GetActiveScene().name == "FinalBossRoom")
        {
            PlayerMovement.instance.FreezePlayer();
            PlayerCamera.instance.DisablePlayerCamera();
        }
        else
        {
            PlayerMovement.instance.UnfreezePlayer();
            PlayerCamera.instance.EnablePlayerCamera();

            player = GameObject.FindGameObjectWithTag("Player");
            Camera mainCam = player.GetComponentsInChildren<Camera>().First();
            mainCam.GetUniversalAdditionalCameraData().cameraStack.Add(canvasCamera);
        }
    }

    public void InstantiateEnemy()
    {
        GameObject[] proxyEnemies = GameObject.FindGameObjectsWithTag("ProxyEnemy");
        if (proxyEnemies.Length > 0)
        {
            foreach (GameObject e in proxyEnemies)
            {
                enemyNames.Add(e.name.Replace("Proxy",""));
                enemyStartingPositions.Add(e.transform.position);
                enemyStartingRotations.Add(e.transform.rotation);
                enemyStartingHealthValues.Add(e.GetComponent<DealerHealth>().currentHealth);
                enemyDifficultyValues.Add(e.GetComponent<DealerHealth>().pcntChanceOfRandomHand);
                enemyRewards.Add(e.GetComponent<EnemyReward>());
                Destroy(e);
            }            
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            for (int i = 0; i < enemyStartingPositions.Count; i++)
            {                
                GameObject enemyPrefab = enemyPrefabs.Where(prefab => prefab.name == enemyNames[i]).ToList<GameObject>().First();
                GameObject enemy = Instantiate(enemyPrefab, enemyStartingPositions[i], enemyStartingRotations[i]);
                enemy.GetComponent<DealerHealth>().currentHealth = enemyStartingHealthValues[i];
                enemy.GetComponent<DealerHealth>().SetHealthText();
                enemy.GetComponent<DealerHealth>().pcntChanceOfRandomHand = enemyDifficultyValues[i];
                enemy.GetComponent<EnemyReward>().baseSO = enemyRewards[i].baseSO;
                enemy.GetComponent<EnemyReward>().powerCardType = enemyRewards[i].powerCardType;
                enemy.GetComponent<EnemyReward>().SetUpCard();
            }
        }
        else
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                
                if (enemies[i].GetComponent<DealerHealth>().currentHealth <= 0)
                {
                    enemies[i].GetComponent<DealerHealth>().DestroySelf();
                }
            }
        }
    }

    public void InstantiateExitCost()
    {             
        GameObject exit = GameObject.FindGameObjectWithTag("ExitCost");
        if (exit == null && SceneManager.GetActiveScene().name != "FinalBossPokerRoom")
        {
            Instantiate(exitCostPrefab, exitCostStartingPosition, exitCostStartingRotation);
            
            int exitHealthCost = int.Parse(GameObject.FindGameObjectWithTag("Exit").GetComponentInChildren<TextMeshPro>().text);

            if (ExitCost.instance != null)
            {
                ExitCost.instance.SetHealth(exitHealthCost);
            }
        }
    }

    private void InstantiateTVC()
    {
        GameObject tvc = GameObject.FindGameObjectWithTag("TVC");
        if (tvc == null)
        {
            Instantiate(tvcPrefab, Vector3.zero, Quaternion.identity);
        }

        tvc = GameObject.FindGameObjectWithTag("TVC");
        //tvc.GetComponent<Canvas>().worldCamera = canvasCamera;
    }

    private void InstantiateSlotMachine()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Poker"))
        {
            GameObject[] slotMachines = GameObject.FindGameObjectsWithTag("SlotMachine");

            if (slotMachines.Length > 0)
            {
                // Check if any slot machines have been loaded before
                if (slotMachines.Where(sm => sm.GetComponentInChildren<SlotMachine>().isTheOriginal == true).ToList().Count > 0)
                {                    
                    // Destroy new slot machines
                    List<GameObject> newSms = slotMachines.Where(sm => sm.GetComponentInChildren<SlotMachine>().isTheOriginal == false).ToList();

                    foreach (GameObject newSm in newSms)
                    {
                        Destroy(newSm);
                    }
                }
                else
                {
                    // Mark new slot machines as original
                    List<GameObject> newSms = slotMachines.Where(sm => sm.GetComponentInChildren<SlotMachine>().isTheOriginal == false).ToList();

                    foreach (GameObject newSm in newSms)
                    {
                        newSm.GetComponentInChildren<SlotMachine>().isTheOriginal = true;
                    }
                }
            }
        }
    }

    private void InstantiateMerchant()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Poker"))
        {            
            GameObject[] merchants = GameObject.FindGameObjectsWithTag("Merchant");
            if (merchants.Length > 0)            
            {
                // Check if merchant loaded before
                if (merchants.Where(merch => merch.GetComponent<Merchant>().isTheOriginal == true).ToList().Count > 0)
                {
                    // Destroy new merchant
                    GameObject newMerch = merchants.Where(merch => merch.GetComponent<Merchant>().isTheOriginal == false).ToList().First();
                    Destroy(newMerch);                    
                }
                else
                {
                    // Mark new merchant as original
                    merchants[0].GetComponent<Merchant>().isTheOriginal = true;
                }
            }            
        }
    }

    public void LoadRoom(string nextSceneName)
    {        
        if (SceneManager.GetActiveScene().name.Contains("Room") && nextSceneName.Contains("Room"))
        {            
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0)
            {
                foreach (GameObject e in enemies) { Destroy(e); }
            }

            GameObject exit = GameObject.FindGameObjectWithTag("ExitCost");
            if (exit != null) { Destroy(exit); }

            GameObject[] slotMachines = GameObject.FindGameObjectsWithTag("SlotMachine");            
            foreach (GameObject sm in slotMachines)
            {
                if (sm != null) { Destroy(sm); }
            }

            GameObject merchant = GameObject.FindGameObjectWithTag("Merchant");
            if (merchant != null) { Destroy(merchant); }

            PlayerMovement.instance.FreezePlayer();
            PlayerMovement.instance.moveToStartingPosition = true;                        
        }

        StartCoroutine(LoadNextScene(nextSceneName));
    }

    public void LoadPoker()
    {
        PlayerInventory.instance.prevScene = SceneManager.GetActiveScene().name;
        
        if (SceneManager.GetActiveScene().name.Contains("Basement"))
        {
            StartCoroutine(LoadNextScene("BasementPoker"));
        }

        if (SceneManager.GetActiveScene().name.Contains("ServiceFloor"))
        {
            StartCoroutine(LoadNextScene("ServiceFloorPoker"));
        }

        if (SceneManager.GetActiveScene().name.Contains("Casino"))
        {
            StartCoroutine(LoadNextScene("CasinoPoker"));
        }

        if (SceneManager.GetActiveScene().name.Contains("WizardTower"))
        {
            StartCoroutine(LoadNextScene("WizardTowerPoker"));
        }
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadNextScene("MainMenu"));
    }

    public void LoadCredits()
    {
        StartCoroutine(LoadNextScene("Credits"));
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        transition.SetTrigger("Start");        
        
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] slotMachines = GameObject.FindGameObjectsWithTag("SlotMachine");
        GameObject merchant = GameObject.FindGameObjectWithTag("Merchant");

        if (sceneName == "MainMenu" || sceneName == "Credits")
        {
            GameObject exit = GameObject.FindGameObjectWithTag("ExitCost");
            GameObject tvc = GameObject.FindGameObjectWithTag("TVC");
            
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null) { Destroy(enemy); }
            }

            foreach (GameObject sm in slotMachines)
            {
                if (sm != null) { Destroy(sm); }
            }

            if (player != null) { Destroy(player); }
            
            if (merchant != null) { Destroy(merchant); }

            if (exit != null) { Destroy(exit); }

            if (tvc != null) { Destroy(tvc); }
        }
        else if (sceneName.Contains("Poker"))
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    if (!enemy.IsDestroyed())
                    {
                        if (enemy.GetComponentInChildren<Animator>(true) != null)
                        {
                            enemy.GetComponentInChildren<Animator>(true).enabled = false;
                        }

                        if (enemy.GetComponentInChildren<UIDealerController>(true) != null)
                        {
                            enemy.GetComponentInChildren<UIDealerController>(true).gameObject.SetActive(false);
                        }

                        if (enemy.GetComponentInChildren<Collider>(true) != null)
                        {
                            enemy.GetComponentInChildren<Collider>(true).enabled = false;
                        }

                        if(enemy.GetComponent<DealerHealth>().currentHealth <= 0)
                        {
                            enemy.GetComponentInChildren<VisualEffect>(true).enabled = false;
                        }
                    }

                    MeshRenderer[] enemyMeshRenderers = enemy.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in enemyMeshRenderers) { mr.enabled = false; };
                    
                    SkinnedMeshRenderer[] enemySkinnedMeshRenderers = enemy.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer smr in enemySkinnedMeshRenderers) { smr.enabled = false; };                    
                }
            }

            foreach (GameObject sm in slotMachines)
            {
                if (sm != null)
                {
                    MeshRenderer[] smMeshRenderers = sm.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in smMeshRenderers) { mr.enabled = false; };
                }

                sm.GetComponentInChildren<Collider>().enabled = false;
            }

            if (merchant != null)
            {
                SkinnedMeshRenderer[] merchantSkinnedMeshRenderers = merchant.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer smr in merchantSkinnedMeshRenderers) { smr.enabled = false; };
                
                merchant.GetComponent<Collider>().enabled = false;
            }

            if (player != null)
            {
                player.GetComponentInChildren<MeshRenderer>().enabled = false;
                player.GetComponent<Interactor>().interactionPromptUI.Close();
                player.GetComponent<Interactor>().enabled = false;
                player.GetComponent<CharacterController>().enabled = false; 
            }

            PlayerMovement.instance.FreezePlayer();
            PlayerCamera.instance.DisablePlayerCamera();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    Animator enemyAnimator = enemy.GetComponentInChildren<Animator>();
                    enemyAnimator.enabled = true;

                    if (!enemy.IsDestroyed())
                    {
                        if (enemy.GetComponentInChildren<Animator>(true) != null)
                        {
                            enemy.GetComponentInChildren<Animator>(true).enabled = true;
                        }

                        if (enemy.GetComponentInChildren<UIDealerController>(true) != null)
                        {
                            enemy.GetComponentInChildren<UIDealerController>(true).gameObject.SetActive(true);
                        }

                        if (enemy.GetComponentInChildren<Collider>(true) != null)
                        {
                            enemy.GetComponentInChildren<Collider>(true).enabled = true;
                        }
                    }

                    MeshRenderer[] enemyMeshRenderers = enemy.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in enemyMeshRenderers) { mr.enabled = true; };

                    SkinnedMeshRenderer[] enemySkinnedMeshRenderers = enemy.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer smr in enemySkinnedMeshRenderers) { smr.enabled = true; };                    

                    enemy.GetComponent<DealerHealth>().SetHealthText();
                }
            }

            foreach (GameObject sm in slotMachines)
            {
                if (sm != null)
                {
                    MeshRenderer[] smMeshRenderers = sm.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in smMeshRenderers) { mr.enabled = true; };
                }

                sm.GetComponentInChildren<Collider>().enabled = true;
            }

            if (merchant != null)
            {
                SkinnedMeshRenderer[] merchantSkinnedMeshRenderers = merchant.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer smr in merchantSkinnedMeshRenderers) { smr.enabled = true; };

                merchant.GetComponent<Collider>().enabled = true;
            }

            if (player != null)
            {
                player.GetComponentInChildren<MeshRenderer>().enabled = true;
                player.GetComponent<Interactor>().enabled = true;
                player.GetComponent<CharacterController>().enabled = true;
            }            
        }
    }        
}