using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D Full;
    public Texture2D ThreeQuarters;
    public Texture2D Half;
    public Texture2D OneQuarter;
    public Texture2D Empty;
    public Texture2D Shield;

    public const int HEARTEMPTY = 0;
    public const int HEARTQUARTER = 1;
    public const int HEARTHALF = 2;
    public const int HEART34S = 3;
    public const int HEARTFULL = 4;
    public const int HEARTSHIELD = 4;

    public bool Shielded;
    public int Value;

    private new RawImage renderer;

    void Start()
    {
        renderer = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (Value)
        {
            case 0: renderer.texture = Empty; break;
            case 1: renderer.texture = OneQuarter; break;
            case 2: renderer.texture = Half; break;
            case 3: renderer.texture = ThreeQuarters; break;
            case 4: renderer.texture = Full; break;
            case 5: renderer.texture = Shield; break;
        }
        Debug.Log($"{this.gameObject.name} - {Value}");
    }
}


