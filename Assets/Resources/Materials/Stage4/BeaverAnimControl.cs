using UnityEngine;

public class BeaverAnimControl : MonoBehaviour
{
    public GameObject Beaver;
    public GameObject BeaverInside;
    public GameObject AnimDoor;
    public GameObject BaseDoor;



    public void DoorAnimOn()
    {
        BaseDoor.SetActive(false);
        AnimDoor.SetActive(true);

        Beaver.SetActive(false);
        BeaverInside.SetActive(true);
    }

    public void DoorAnimOff()
    {
        BaseDoor.SetActive(true);
        AnimDoor.SetActive(false);

        Beaver.gameObject.SetActive(true);
        BeaverInside.SetActive(false);
    }
}
