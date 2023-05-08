using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] Canvas gameoverScreen;
    [SerializeField] Image panel;
    [SerializeField] Button restartGame;

    void Start() 
    {
        if (gameoverScreen != null && panel != null && restartGame != null) 
        {
            restartGame.onClick.AddListener(restart);
        }
        else 
        {
            throw new MissingComponentException("GameOver screen not setup.");
        }
    }


    void restart() 
    {
        GameManager.gameManagerInstance.restartGame();
    }
}
