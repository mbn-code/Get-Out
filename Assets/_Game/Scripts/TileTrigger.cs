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
        private Vector3 lastPlayerPosition; // Store the last valid position of the player

        private void OnTriggerEnter(Collider other)
        {
            // Check if the object that entered the trigger is the player
            if (other.CompareTag("Player"))
            {
                // Store the player's current position when they enter the tile
                lastPlayerPosition = other.transform.position;

                // If the tile is not walkable, push the player back
                if (!isWalkable)
                {
                    PushPlayerBack(other);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // If the player is still on the tile and it's not walkable, keep them from moving
            if (other.CompareTag("Player") && !isWalkable)
            {
                // Smoothly move the player back to the last valid position
                other.transform.position = Vector3.Lerp(other.transform.position, lastPlayerPosition, Time.deltaTime * 5f); // Adjust speed as necessary
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Optional: Handle what happens when the player exits the tile
            if (other.CompareTag("Player"))
            {
                // Reset the last valid position when the player leaves the tile
                lastPlayerPosition = other.transform.position;
            }
        }

        private void PushPlayerBack(Collider player)
        {
            // Get the player's Rigidbody
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Calculate the push-back direction
                Vector3 pushBackDirection = (player.transform.position - transform.position).normalized;
                playerRigidbody.velocity = pushBackDirection * 2f; // Adjust the speed as necessary
            }
        }
    }
}
