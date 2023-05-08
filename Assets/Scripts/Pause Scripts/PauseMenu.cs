using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Canvas titleScreen;
    [SerializeField] Image panel;
    [SerializeField] Button game;
    [SerializeField] Button menu;
    bool checkComponents;
    public static int previousSceneIndex = -1;
    [SerializeField] public GameObject playerPrefab;

    void Awake() 
    {
        checkComponents = titleScreen != null && panel != null && game != null && menu != null;
    }

    void Start() 
    {
        if (checkComponents) 
        {
            previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (titleScreen.enabled) 
            {
                game.onClick.AddListener(resume);
                menu.onClick.AddListener(returnMenu);
            }
        }
        else 
        {
            throw new MissingComponentException("Missing components for pause canvas.");
        } 
    }

    public static void pauseGame(Vector3 currentPosition, Quaternion currentRotation, int currentHealth, int currentMana, float currentStamina) 
    {
        Time.timeScale = 0f;

        if (currentHealth > 0 && currentMana >= 0 && currentStamina >= 0) 
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            PlayerData data = new PlayerData();
            data.playerPosition = currentPosition;
            data.playerRotation = currentRotation;
            data.health = currentHealth;
            data.mana = currentMana;
            data.stamina = currentStamina;
            // data.playerTime = playerTime;
            PlayerPrefs.SetInt("PreviousSceneIndex", currentSceneIndex);
            PlayerPrefs.SetString("PlayerData", JsonUtility.ToJson(data));
        }
        else 
        {
            Debug.LogWarning("Information was not saved.");
        }

        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.PAUSE);
    }

    void resume() 
    {
        // if (PlayerPrefs.HasKey("PreviousSceneIndex") && PlayerPrefs.HasKey("PlayerData")) 
        // {
        //     previousSceneIndex = PlayerPrefs.GetInt("PreviousSceneIndex");
        //     SceneManager.LoadScene(previousSceneIndex);

        //     string playerDataJSON = PlayerPrefs.GetString("PlayerData");
        //     PlayerData playerData = JsonUtility.FromJson<PlayerData>(playerDataJSON);

        //     if (GameObject.FindGameObjectWithTag("Player") == null) 
        //     {
        //         Instantiate(playerPrefab, playerData.playerPosition, playerData.playerRotation);
        //     }

        //     GameObject player = GameObject.FindGameObjectWithTag("Player");

        //     player.transform.position = playerData.playerPosition;
        //     player.transform.rotation = playerData.playerRotation;

        //     playerData.health = HealthManaStaminaManager.playerStats.getCurrentHealth();
        //     playerData.mana = HealthManaStaminaManager.playerStats.getCurrentMana();
        //     playerData.stamina = HealthManaStaminaManager.playerStats.getCurrentStamina();
        //     // DayNightCycle.instance.currentTime = playerData.playerTime;

        //     Time.timeScale = 1f;
        // }
        // else 
        // {
        //     GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.Main);
        // }

        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.MAIN);
    }   

    void returnMenu() 
    {
        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.TITLE);
    }
}
