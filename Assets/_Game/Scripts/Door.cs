using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Key requiredKey;
    [SerializeField] private Animator doorAnimator;

    private bool isUnlocked = false;

    public void TryUnlock(Key playerKey)
    {
        if (playerKey == requiredKey)
        {
            isUnlocked = true;
            Debug.Log("Door unlocked!");
            OpenDoor();
        }
        else
        {
            Debug.Log("Incorrect key!");
        }
    }

    private void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }
        else
        {
            Debug.LogWarning("No animator assigned to door.");
        }
    }
}
