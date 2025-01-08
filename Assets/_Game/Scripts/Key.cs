using UnityEngine;

public class Key : MonoBehaviour
{
    public bool IsPickedUp { get; private set; } = false;

    public void PickUp()
    {
        IsPickedUp = true;
    }

    public void Drop()
    {
        IsPickedUp = false;
    }
}
