using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // making this script a singleton
    public static GameManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // variables for fading
    public CanvasGroup blackScreen;
    public float fadeDuration = 0.9f;
    
    // flag game begin
    public bool gameStarted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        blackScreen = GetComponentInChildren<CanvasGroup>();
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadEndings(bool isGoodEnding)
    {
        StartCoroutine(EndingTransition(isGoodEnding));
    }
    
    private IEnumerator EndingTransition(bool isGoodEnding)
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f));
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Endings");
        
        yield return null; // wait for the scene to load

        if (isGoodEnding)
        {
            Debug.Log("Loading good ending...");
            GameObject.Find("BE").SetActive(false);
        }
        else
        {
            Debug.Log("Loading bad ending...");
            GameObject.Find("HE").SetActive(false);
        }
        
        StartCoroutine(FadeRoutine(1f, 0f));
    }
    
    public void OnApplicationQuit()
    {
        instance = null;
    }
    
    // some fading stuff
    public void FadeToBlack()
    {
        StartCoroutine(FadeRoutine(0f, 1f));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float timer = 0f;
        blackScreen.alpha = startAlpha;

        while (timer < fadeDuration)
        {
            // lerp the alpha value based on the timer
            blackScreen.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        blackScreen.alpha = targetAlpha;
    }
}
