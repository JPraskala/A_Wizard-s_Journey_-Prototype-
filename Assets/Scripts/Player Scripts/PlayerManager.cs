using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static GameObject playerInstance;
    [SerializeField] GameObject player;
    public static Vector3 playerPosition;

    void Awake() 
    {
        if (playerInstance == null) 
        {
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (tagExists("Player")) 
            {
                if (existingPlayer != null) 
                {
                    playerInstance = existingPlayer;
                }
                else if (player != null) 
                {
                    playerInstance = Instantiate(player);
                }
                else 
                {
                    throw new MissingReferenceException("Both existingPlayer and player are null in PlayerManager.");
                }

                //GameManager.gameManagerInstance.playerScene = (GameManager.myScenes)SceneManager.GetActiveScene().buildIndex;
            }
            else 
            {
                throw new UnityException("Tag Player does not exist.");
            }
        }
    }

    void Start() 
    {
        SceneManager.sceneUnloaded += onSceneUnloaded;
        player.transform.position = playerInstance.transform.position;
        DontDestroyOnLoad(playerInstance);
    }

    void onDestroy() 
    {
        SceneManager.sceneUnloaded -= onSceneUnloaded;
    }

    void onSceneUnloaded(Scene scene) 
    {
        if (playerInstance != null) 
        {
            Destroy(playerInstance);
        }

        if (scene.buildIndex != (int) GameManager.myScenes.MAIN && scene.buildIndex != (int) GameManager.myScenes.CAMP) 
        {
            PauseMenu.previousSceneIndex = scene.buildIndex;
        }
    }

    bool playerAllowedInScene() 
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.buildIndex == (int) GameManager.myScenes.CAMP ^ currentScene.buildIndex == (int) GameManager.myScenes.MAIN;
    }

    void Update() 
    {
        if (!playerAllowedInScene()) 
        {
            GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.MAIN);
        }

        
    }

    static bool tagExists(string tag) 
    {
        try 
        {
            GameObject.FindGameObjectsWithTag(tag);
            return true;
        }
        catch 
        {
            return false;
        }
    }
}
