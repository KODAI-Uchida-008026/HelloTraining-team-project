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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var kagi = collision.GetComponent<Antigravity>();
        if (kagi != null && kagi.HasKagi())
        {
            OpenDoor();
        }
    }
}