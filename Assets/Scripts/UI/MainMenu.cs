using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class MainMenu : MonoBehaviour
{
    public Sprite[] sprites;
    public Image backgroundImage;
    public Animator transition;
    public float transitionTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    public void StartGame()
    {
        StartCoroutine(LoadNextScene("Room1"));
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);

    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }
}
