using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Cinema : MonoBehaviour
{
    public PlayableDirector timeline;
    public int SceneToTravelTo;
    public LevelLoad loader;

    private bool doneTransitioning = false;
    private bool playing = false;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        //timeline.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (loader.GetAnimationFinished())
        {
            doneTransitioning = true;
        }
        if (doneTransitioning)
        {
            doneTransitioning = false;
            timeline.Play();
            playing = true;
        }
        if (playing)
        {
            timer += Time.deltaTime;
            if (timeline.state != PlayState.Playing)
            {
                if (timer >= timeline.duration)
                {
                    SceneManager.LoadScene(SceneToTravelTo);
                }
            }
        }
    }
}
