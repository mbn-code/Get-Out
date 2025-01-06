using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private string correctSpriteName; // Name of the correct sprite
    [SerializeField] private GameObject actionObject; // Object to activate or any other action

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has a SpriteRenderer
        SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Check if the sprite name matches the correct sprite name
            if (spriteRenderer.sprite.name == correctSpriteName)
            {
                // Perform the desired action
                PerformAction();
            }
        }
    }

    private void PerformAction()
    {
        // Example action: Activate a GameObject
        if (actionObject != null)
        {
            actionObject.SetActive(true);
        }

        // You can add other actions here, such as playing a sound, changing the scene, etc.
    }
}
