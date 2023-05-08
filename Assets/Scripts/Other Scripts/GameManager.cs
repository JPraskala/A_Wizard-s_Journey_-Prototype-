using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;
    public enum myScenes 
    {
        TITLE,
        PAUSE,
        LOSE,
        WIN,
        CREDITS,
        MAIN,
        CAMP
    }

    [System.NonSerialized] public myScenes playerScene;

    bool checkScenes;

    void Awake() 
    {
        if (gameManagerInstance == null) 
        {
            gameManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }

        checkScenes = sceneExists((int) myScenes.TITLE) && sceneExists((int) myScenes.PAUSE) && sceneExists((int) myScenes.LOSE) && sceneExists((int) myScenes.WIN) && sceneExists((int) myScenes.CREDITS) && sceneExists((int) myScenes.MAIN) && sceneExists((int) myScenes.CAMP);
    }

    void Start() 
    {
        if (checkScenes) 
        {
            sceneManager(myScenes.TITLE);
        }
        else 
        {
            throw new UnityException("All scenes do not exist.");
        }
    }

    public void sceneManager(myScenes? scene = null) 
    {
        if (scene.HasValue && ((scene < myScenes.TITLE) ^ (scene > myScenes.CAMP))) 
        {
            Debug.LogError("Scene index is out of bounds.");
            return;
        }

        if (scene.HasValue) 
        {
            Scene currentScene = SceneManager.GetActiveScene();
            switch (scene) 
            {
                case myScenes.TITLE:
                    SceneManager.LoadScene((int) myScenes.TITLE);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case myScenes.PAUSE:
                    if ((currentScene.buildIndex == (int) myScenes.CAMP) ^ (currentScene.buildIndex == (int) myScenes.MAIN)) 
                    {
                        SceneManager.LoadScene((int) myScenes.PAUSE);
                    }
                    else 
                    {
                        return;
                    }
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case myScenes.LOSE:
                    SceneManager.LoadScene((int) myScenes.LOSE);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case myScenes.WIN:
                    SceneManager.LoadScene((int) myScenes.WIN);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case myScenes.CREDITS:
                    SceneManager.LoadScene((int) myScenes.CREDITS);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case myScenes.MAIN:
                    SceneManager.LoadScene((int) myScenes.MAIN);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case myScenes.CAMP:
                    SceneManager.LoadScene((int) myScenes.CAMP);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.None;
                    break; 
            }
        }
        else 
        {
            throw new System.Exception("Scene does not have a value.");
        }


        // if (scene.HasValue) 
        // {
        //     SceneManager.LoadScene((int) scene);
        // }
        // else if (previousSceneIndex >= 0 && previousSceneIndex < SceneManager.sceneCountInBuildSettings) 
        // {
        //     SceneManager.LoadScene(previousSceneIndex);
        // }
        // else 
        // {
        //     throw new UnityException("Cannot load previous scene");
        // }        
    }
    
    bool sceneExists(int sceneIndex) 
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
        return !string.IsNullOrEmpty(scenePath);
    }

    public void restartGame() 
    {
        SceneManager.LoadScene((int) myScenes.TITLE);
    }
}
