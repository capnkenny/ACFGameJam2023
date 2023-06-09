using UnityEngine;

public class Door : MonoBehaviour
{
    public bool DoorOpened;
    public bool DoorEnabled;
    public bool EndRoomDoor;

    public Travelpoint DoorPoint;
    public GameObject SnapPoint;

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
            if (!EndRoomDoor)
            {
                DoorFrame.sprite = WallSprite;
                DoorClosed.SetActive(true);
                DoorClosed.GetComponent<SpriteRenderer>().enabled = false;
                DoorOpen.SetActive(false);
            }
            else
            {
                DoorEnabled = true;
				DoorOpen.SetActive(true);
				DoorClosed.SetActive(false);
			}
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
        else
        {
            DoorOpen.SetActive(DoorOpened);
            DoorClosed.SetActive(!DoorOpened);
        }
		
	}
}
