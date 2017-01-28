using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class ButtonHandler_UI : MonoBehaviour {
    public GameObject howTo;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RestartCurrentScene()
    {
        GameManager.theManager.RestartCurrentScene();
    }

    public void OnMainMenu()
    {
        GameManager.theManager.OnMainMenu();
    }


    public void OnMainMenu_PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnMainMenu_QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
        #else
            Application.Quit();
        #endif
    }

    public void OnMainMenu_HowTo()
    {
        if(howTo != null)
        {
            howTo.SetActive(!howTo.activeSelf);
        }
    }
}
