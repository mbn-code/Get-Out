using UnityEngine;
using System;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private string correctObjectName; // Name of the correct object
    [SerializeField] private GameObject actionObject; // Object to activate or any other action

    public event Action OnCorrectObjectPlaced; // Event to notify the manager
    public bool IsCorrectObjectPlaced { get; private set; } // Property to check if the correct object is placed

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the correct name
        if (other.gameObject.name == correctObjectName)
        {
            // Perform the desired action
            PerformAction();
            IsCorrectObjectPlaced = true;
            OnCorrectObjectPlaced?.Invoke(); // Notify the manager
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the flag if the object leaves the platform
        if (other.gameObject.name == correctObjectName)
        {
            IsCorrectObjectPlaced = false;
        }
    }

    private void PerformAction()
    {
        Debug.Log("Correct object detected!");
        // Example action: Activate a GameObject
        if (actionObject != null)
        {
            actionObject.SetActive(true);
            Debug.Log("Activated " + actionObject.name);
        }

        // You can add other actions here, such as playing a sound, changing the scene, etc.
    }
}
