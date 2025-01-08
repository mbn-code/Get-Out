using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LRS
{
    public class TileTrigger : MonoBehaviour
    {
        private static readonly List<Vector2Int> allowedCoordinates = new List<Vector2Int>
        {
            // Hardcoded path along the top row, then down the far right column

            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(3, 1),
            new Vector2Int(3, 2),
            new Vector2Int(4, 2),
            new Vector2Int(5, 2),
            new Vector2Int(5, 1),
            new Vector2Int(6, 1),
            new Vector2Int(7, 1),
            new Vector2Int(7, 2),
            new Vector2Int(8, 2),
            new Vector2Int(9, 2),
            new Vector2Int(9, 3),
            new Vector2Int(10, 3),
            new Vector2Int(11, 3),
            new Vector2Int(12, 3),
            new Vector2Int(13, 3),
            new Vector2Int(14, 3),
            new Vector2Int(15, 3),
            new Vector2Int(16, 3),
            new Vector2Int(16, 4)
        };

        private void Awake()
        {
            Vector2Int tilePos = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.z)
            );
            isWalkable = allowedCoordinates.Contains(tilePos);
        }

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
