using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameManager parentManager;


    public void QuitGame()
    {
        var mu = GameObject.FindGameObjectWithTag("Music");
        if (mu != null)
            mu.GetComponent<MusicPlayer>().StopMusic();
        this.gameObject.SetActive(false);
        parentManager.Loader.transition.SetTrigger("Loading");
        parentManager.SaveCurrentPlayerData();
        parentManager.TransitionToLevel(-10);
    }

    public void GoToHub()
    {
        var mu = GameObject.FindGameObjectWithTag("Music");
        if (mu != null)
            mu.GetComponent<MusicPlayer>().StopMusic();
        this.gameObject.SetActive(false);
        parentManager.SaveCurrentPlayerData();
        parentManager.TransitionToLevel(-1);
    }
    
    
}
