using UnityEngine;

public class Door : MonoBehaviour
{
    public bool DoorOpened;
    public bool DoorEnabled;

    public Travelpoint DoorPoint;

    [SerializeField]
    private SpriteRenderer DoorFrame;
    [SerializeField]
    private GameObject DoorOpen;
	[SerializeField]
	private GameObject DoorClosed;

    [SerializeField]
    private Sprite WallSprite;

	// Start is called before the first frame update
	void Start()
    {
        if (!DoorEnabled)
        {
            DoorFrame.sprite = WallSprite;
            DoorClosed.SetActive(true);
            DoorClosed.GetComponent<SpriteRenderer>().enabled = false;
            DoorOpen.SetActive(false);
        }
        else
        {
			DoorOpen.SetActive(DoorOpened);
			DoorClosed.SetActive(!DoorOpened);
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (DoorEnabled)
        {
			DoorOpen.SetActive(DoorOpened);
			DoorClosed.SetActive(!DoorOpened);
		}
		
	}
}
