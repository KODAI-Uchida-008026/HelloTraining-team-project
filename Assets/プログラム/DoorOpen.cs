using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public GameObject ドア;
    public GameObject 開いたドア;


    // ドアの開閉
    public void OpenDoor()
    {
        ドア.SetActive(false);
        開いたドア.SetActive(true);
    }
}