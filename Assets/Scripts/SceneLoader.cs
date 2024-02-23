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
    public Vector3 playerStartingPosition = new Vector3(1.875f, 0.617f, 0);
    public Quaternion playerStartingRotation = Quaternion.AngleAxis(-90, Vector3.up);

    public GameObject enemyPrefab;
    public Vector3 enemyStartingPosition = new Vector3(-0.166f, 0.957f, -1.96f);
    public Quaternion enemyStartingRotation = Quaternion.identity;

    public GameObject exitCostPrefab;
    public Vector3 exitCostStartingPosition = new Vector3(0f, 10f, 0f);
    public Quaternion exitCostStartingRotation = Quaternion.identity;

    private void Start()
    {        
        InstantiatePlayer();
        InstantiateEnemy();
        InstantiateExitCost();
    }

    public void InstantiatePlayer()
    {
        GameObject[] proxyPlayer = GameObject.FindGameObjectsWithTag("ProxyPlayer");
        if(proxyPlayer.Length > 0)
        {            
            foreach(GameObject p in proxyPlayer)
            {
                Destroy(p);
            }           
        }

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        if (objs.Length < 1)
        {            
            Instantiate(playerPrefab, playerStartingPosition, playerStartingRotation);
        }

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PlayerMovement.instance.FreezePlayer();
            PlayerCamera.instance.DisablePlayerCamera();
        }
    }

    public void InstantiateEnemy()
    {
        GameObject[] proxyEnemies = GameObject.FindGameObjectsWithTag("ProxyEnemy");
        if (proxyEnemies.Length > 0)
        {
            foreach (GameObject e in proxyEnemies)
            {
                Destroy(e);
            }
        }

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        if (objs.Length < 1)
        {
            Instantiate(enemyPrefab, enemyStartingPosition, enemyStartingRotation);
        }
    }

    public void InstantiateExitCost()
    {
        //GameObject[] exitCost = GameObject.FindGameObjectsWithTag("ProxyExitCost");
        //if (exitCost.Length > 0)
        //{
        //    foreach (GameObject e in exitCost)
        //    {
        //        Destroy(e);
        //    }
        //}        
        
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ExitCost");
        if (objs.Length < 1)
        {
            Instantiate(exitCostPrefab, exitCostStartingPosition, exitCostStartingRotation);

            int exitHealthCost = int.Parse(GameObject.FindGameObjectWithTag("Exit").GetComponentInChildren<TextMeshProUGUI>().text);

            if (ExitCost.instance != null)
            {
                ExitCost.instance.SetHealth(exitHealthCost);
            }
        }
    }

    public void LoadRoom()
    {        
        StartCoroutine(LoadNextScene("Room1"));        
    }

    public void LoadPoker()
    {
        StartCoroutine(LoadNextScene("Poker"));
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

        if (sceneName == "Poker")
        {            
            PlayerMovement.instance.FreezePlayer();
            PlayerCamera.instance.DisablePlayerCamera();
        }
        else
        {
            PlayerMovement.instance.UnfreezePlayer();
            PlayerCamera.instance.EnablePlayerCamera();
        }
    }
}
