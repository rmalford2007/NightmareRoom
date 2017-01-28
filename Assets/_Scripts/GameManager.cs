using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public delegate void GameMasterEvents(object value);

public class GameManager : MonoBehaviour
{
    float defaultChainMultiplier = 1.0f;
    float currentChainMultiplier = 1.0f;
    public float chainIncrement = 0.1f;
    public float chainTime = 2.0f;
    public static GameManager theManager;
    public GameObject UI_Win;
    public GameObject UI_Lose;
    public GameObject UI_Restart;
    public GameObject UI_Quit;
    public GameObject UI_PauseScreen;
    int innerWallCount = 0;
    float elapsedTime = 0.0f;
    bool inMultiplierChain = false;
    bool changeHint = false;
    bool isPaused = false;
    bool updatingStats = false;

    // Use this for initialization
    void Start()
    {
        inMultiplierChain = false;
        changeHint = false;
        UI_Win.SetActive(false);
        UI_Lose.SetActive(false);
        UI_Restart.SetActive(false);
        UI_Quit.SetActive(false);
        UI_PauseScreen.SetActive(false);
        
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inMultiplierChain)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= chainTime)
            {
                ResetChainMultiplier();
                inMultiplierChain = false;
            }
        }
        if(changeHint && !updatingStats)
        {
            updatingStats = true;
            StartCoroutine(UpdateStats());
        }
    }

    public void SetWallGoal(int amount)
    {
        Stats managerStats = GetComponent<Stats>();
        if(managerStats != null)
        {
            innerWallCount = amount;
            managerStats.goal.max = amount;

            managerStats.SetGoal(amount);
            managerStats.SetResource(0);
        }
        changeHint = true;
    }

    public void HintGoalChanges()
    {
        changeHint = true;
    }

    public void RestartCurrentScene()
    {
        if (isPaused)
        {
            Time.timeScale = 1.0f;
            isPaused = false;
        }
        Debug.Log("Restarting scene: " + SceneManager.GetActiveScene().name.ToString());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenu()
    {
        if(isPaused)
        {
            Time.timeScale = 1.0f;
            isPaused = false;
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void SetPause(bool val)
    {
        if(val)
        {
            Time.timeScale = 0.0f;
            UI_PauseScreen.SetActive(true);
            UI_Restart.SetActive(true);
            UI_Quit.SetActive(true);
        }
        else
        {
            UI_PauseScreen.SetActive(false);
            UI_Restart.SetActive(false);
            UI_Quit.SetActive(false);
            Time.timeScale = 1.0f;
        }
        isPaused = val;
    }

    IEnumerator UpdateStats()
    {
        
        int destroyedWalls = 0;
        GameObject[] wallsRemaining = GameObject.FindGameObjectsWithTag("Rebuild");
        if(wallsRemaining != null)
        {
            destroyedWalls = wallsRemaining.Length;
        }
        yield return null;
        int enemyCount = 0;
        GameObject[] enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemiesRemaining != null)
        {
            enemyCount = enemiesRemaining.Length;
        }
        yield return null;
        Stats managerStats = GetComponent<Stats>();
        if (managerStats != null)
        {
            managerStats.goal.max = innerWallCount + enemyCount;
            managerStats.SetGoal(managerStats.goal.max - destroyedWalls);
            //managerStats.SetResource(enemyCount);
        }
        yield return null;
        CheckWin(managerStats);
        changeHint = false;
        updatingStats = false;
        yield return null;
    }

    void CheckWin(Stats managerStats)
    {
        if(managerStats != null)
        {
            if (managerStats.goal.current == 0)
            {
                
                //DO WIN STUFF
                UI_Win.SetActive(true);
                UI_Restart.SetActive(true);
                UI_Quit.SetActive(true);
            }
        }
    }

    public void GameOver()
    {
        UI_Lose.SetActive(true);
        UI_Restart.SetActive(true);
        UI_Quit.SetActive(true);
    }

    void Awake()
    {
        if (theManager != null)
            GameObject.Destroy(theManager);
        else
            theManager = this;
        Cursor.lockState = CursorLockMode.Confined;
        //DontDestroyOnLoad(this);
    }

    public float GetCurrentChainMultiplier()
    {
        return currentChainMultiplier;
    }

    public void IncrementChainMultiplier()
    {
        currentChainMultiplier += chainIncrement;
        elapsedTime = 0.0f;
        inMultiplierChain = true;
    }

    public void ResetChainMultiplier()
    {
        currentChainMultiplier = defaultChainMultiplier;
    }
}
