using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Image backgroundImage;
    public Animator transition;
    public float transitionTime = 60f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNextScene("MainMenu"));
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            transitionTime = 3f;
            StartCoroutine(LoadNextScene("MainMenu"));
        }
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }  
}
