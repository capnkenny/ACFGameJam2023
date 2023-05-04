using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource source;

    public bool IsPlaying { get { return source.isPlaying; } }
	public void PlayMusic()
    {
        Debug.LogWarning("Playing requested source");
        source.Play();
    }

    public void StopMusic()
    {
        source.Stop();
    }



    
}
