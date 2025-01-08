using UnityEngine;

public class DoorDestroyer : MonoBehaviour
{
    [SerializeField] private string keyTag = "Key"; // Tag to identify the key

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(keyTag))
        {
            Destroy(gameObject); // Destroy the door
            Debug.Log("Door destroyed!");
        }
    }
}
