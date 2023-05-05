using Unity.Mathematics;
using UnityEngine;

public class HeartBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Heart Heart1;
    public Heart Heart2;
    public Heart Heart3;
    public Heart Heart4;
    public Heart Heart5;

    public float HealthValue;
    public float MaxHealthValue;

    private const int divisor = 25;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MaxHealthValue > 100)
        {
            Heart5.gameObject.SetActive(true);
        }
        else
        {
            Heart5.gameObject.SetActive(false);
        }


        int div = (int)math.trunc(HealthValue / divisor);
        int per = (int)math.trunc(((HealthValue / divisor) - div) * 100);
        int ind = 1;
       
        switch (div)
        {
            case 0: break;
            case 1:
                {
                    Heart1.Value = Heart.HEARTFULL;
                    ind++;
                    break;
                }
            case 2:
                {
                    Heart1.Value = Heart.HEARTFULL;
                    Heart2.Value = Heart.HEARTFULL;
                    ind += 2;
                    break;
                }
            case 3:
                {
                    Heart1.Value = Heart.HEARTFULL;
                    Heart2.Value = Heart.HEARTFULL;
                    Heart3.Value = Heart.HEARTFULL;
                    ind += 3;
                    break;
                }
            case 4:
                {
                    Heart1.Value = Heart.HEARTFULL;
                    Heart2.Value = Heart.HEARTFULL;
                    Heart3.Value = Heart.HEARTFULL;
                    Heart4.Value = Heart.HEARTFULL;
                    ind += 4;
                    break;
                }
            case 5:
                {
                    Heart1.Value = Heart.HEARTFULL;
                    Heart2.Value = Heart.HEARTFULL;
                    Heart3.Value = Heart.HEARTFULL;
                    Heart4.Value = Heart.HEARTFULL;
                    Heart5.Value = Heart.HEARTFULL;
                    ind += 5;
                    break;
                }
        }

        //Tell heart index to apply percentage value
        if (ind < 6)
        {
            int value = Heart.HEART34S;
            if (per <= 50 && per > 25)
                value = Heart.HEARTHALF;
            else if (per <= 25 && per >= 0)
                value = Heart.HEARTQUARTER;
            else
                value = Heart.HEARTEMPTY;
            
            switch (ind)
            {
                case 1:
                    {
                        Heart1.Value = value;
                        Heart2.Value = Heart.HEARTEMPTY;
                        Heart3.Value = Heart.HEARTEMPTY;
                        Heart4.Value = Heart.HEARTEMPTY;
                        Heart5.Value = Heart.HEARTEMPTY;
                        break;
                    }
                case 2:
                    {
                        Heart2.Value = value;
                        Heart3.Value = Heart.HEARTEMPTY;
                        Heart4.Value = Heart.HEARTEMPTY;
                        Heart5.Value = Heart.HEARTEMPTY;
                        break;
                    }
                case 3:
                    {
                        Heart3.Value = value;                        
                        Heart4.Value = Heart.HEARTEMPTY;
                        Heart5.Value = Heart.HEARTEMPTY;
                        break;
                    }
                case 4:
                    {
                        Heart4.Value = value;
                        Heart5.Value = Heart.HEARTEMPTY;
                        break;
                    }
                case 5:
                    {
                        Heart5.Value = value;
                        break;
                    }
                default: break;
            }
        }


        
    }
}
