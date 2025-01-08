using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LRS
{
    public class TileTrigger : MonoBehaviour
    {
        public void SetWalkable(bool walkable)
        {
            isWalkable = walkable;
        }

        [SerializeField] private bool isWalkable = true; // Set this in the Inspector for each tile

        private void OnCollisionEnter(Collision other)
        {
            // Check if the object that entered the trigger is the player
            if (other.gameObject.CompareTag("Player"))
            {
                // If the tile is not walkable, push the player back
                if (!isWalkable)
                {
                    PushPlayerBack(other);
                }
            }
        }

        private void PushPlayerBack(Collision player)
        {
            // Get the player's Rigidbody
            Rigidbody playerRigidbody = player.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Use Transform.left to push the player to the left
                Vector3 pushBackDirection = transform.TransformDirection(Vector3.left);
                playerRigidbody.AddForce(pushBackDirection * 5000f); // Adjust the force as necessary
            }
        }
    }
}
