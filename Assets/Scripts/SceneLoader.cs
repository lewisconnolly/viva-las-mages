using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Quaternion playerStartingRotation = Quaternion.AngleAxis(-90, Vector3.up);

    public GameObject enemyPrefab;
    public Vector3 enemyStartingPosition = new Vector3(-0.166f, 0.957f, -1.96f);
    public Quaternion enemyStartingRotation = Quaternion.identity;
    private List<Vector3> enemyStartingPositions = new List<Vector3>();
    private List<Quaternion> enemyStartingRotations = new List<Quaternion>();
    private List<int> enemyStartingHealthValues = new List<int>();
    private List<EnemyReward> enemyRewards = new List<EnemyReward>();

    public GameObject exitCostPrefab;
    public Vector3 exitCostStartingPosition = new Vector3(0f, 10f, 0f);
    public Quaternion exitCostStartingRotation = Quaternion.identity;

    void Start()
    {        
        InstantiatePlayer();
        InstantiateEnemy();
        InstantiateExitCost();        
    }

    void Update()
    {

        if (PlayerMovement.instance.moveToStartingPosition)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                PlayerMovement.instance.UnfreezePlayer();
                PlayerMovement.instance.moveToStartingPosition = false;
            }
        }
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
            Instantiate(playerPrefab, playerStartingPosition.transform.position, playerStartingRotation);
        }
        else
        {
            if (PlayerMovement.instance.moveToStartingPosition)
            {
                player.transform.position = playerStartingPosition.transform.position;                                
            }
        }

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PlayerMovement.instance.FreezePlayer();
            PlayerCamera.instance.DisablePlayerCamera();
        }
        else
        { 
            PlayerCamera.instance.EnablePlayerCamera();
        }
    }

    public void InstantiateEnemy()
    {
        GameObject[] proxyEnemies = GameObject.FindGameObjectsWithTag("ProxyEnemy");
        if (proxyEnemies.Length > 0)
        {
            foreach (GameObject e in proxyEnemies)
            {
                enemyStartingPositions.Add(e.transform.position);
                enemyStartingRotations.Add(e.transform.rotation);
                enemyStartingHealthValues.Add(e.GetComponent<DealerHealth>().currentHealth);
                enemyRewards.Add(e.GetComponent<EnemyReward>());
                Destroy(e);
            }            
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            for (int i = 0; i < enemyStartingPositions.Count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, enemyStartingPositions[i], enemyStartingRotations[i]);
                enemy.GetComponent<DealerHealth>().currentHealth = enemyStartingHealthValues[i];
                enemy.GetComponent<EnemyReward>().baseSO = enemyRewards[i].baseSO;
                enemy.GetComponent<EnemyReward>().powerCardType = enemyRewards[i].powerCardType;
                enemy.GetComponent<EnemyReward>().SetUpCard();
            }
        }
    }

    public void InstantiateExitCost()
    {             
        GameObject exit = GameObject.FindGameObjectWithTag("ExitCost");
        if (exit == null)
        {
            Instantiate(exitCostPrefab, exitCostStartingPosition, exitCostStartingRotation);
            
            int exitHealthCost = int.Parse(GameObject.FindGameObjectWithTag("Exit").GetComponentInChildren<TextMeshPro>().text);

            if (ExitCost.instance != null)
            {
                ExitCost.instance.SetHealth(exitHealthCost);
            }
        }
    }

    public void LoadRoom(string nextSceneName)
    {        
        if (SceneManager.GetActiveScene().name.StartsWith("Room") && nextSceneName.StartsWith("Room"))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0)
            {
                foreach (GameObject e in enemies)
                {
                    Destroy(e);
                }
            }

            GameObject exit = GameObject.FindGameObjectWithTag("ExitCost");
            if (exit != null)
            {
                Destroy(exit);
            }

            PlayerMovement.instance.FreezePlayer();
            PlayerMovement.instance.moveToStartingPosition = true;            
        }

        StartCoroutine(LoadNextScene(nextSceneName));
    }

    public void LoadPoker()
    {        
        PlayerInventory.instance.prevScene = SceneManager.GetActiveScene().name;        
        StartCoroutine(LoadNextScene("Poker"));
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        transition.SetTrigger("Start");        
        
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (sceneName == "Poker")
        {                       
            if (enemy != null)
            {
                enemy.GetComponentInChildren<MeshRenderer>().enabled = false;
                enemy.GetComponentInChildren<UIDealerController>().gameObject.SetActive(false);                
            }

            if (player != null)
            {
                player.GetComponentInChildren<MeshRenderer>().enabled = false;
                player.GetComponent<Interactor>().interactionPromptUI.Close();
                player.GetComponent<Interactor>().enabled = false;

            }

            PlayerMovement.instance.FreezePlayer();
            PlayerCamera.instance.DisablePlayerCamera();
        }
        else
        {            
            if (enemy != null)
            {
                enemy.GetComponentInChildren<MeshRenderer>().enabled = true;
                enemy.GetComponentInChildren<UIDealerController>(true).gameObject.SetActive(true);
            }

            if (player != null)
            {
                player.GetComponentInChildren<MeshRenderer>().enabled = true;
                player.GetComponent<Interactor>().enabled = true;
            }            
        }
    }    
}
