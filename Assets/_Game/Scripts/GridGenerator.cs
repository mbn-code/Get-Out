// GridGenerator.cs

using System.Collections.Generic;
using UnityEngine;

namespace LRS
{
    public class GridGenerator : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Vector3 gridPosition = Vector3.zero; // Position in world space
        [SerializeField] private Vector2Int gridSize = new Vector2Int(5, 5); // Width and Length of grid
        [SerializeField] private float spacing = 1.1f; // Space between tiles
        [SerializeField] private float height = 0f; // Y position of the grid

        [Header("Walkable Tiles")]
        [SerializeField] private bool[,] walkableTiles; // Set in inspector or through method
        [SerializeField] private bool showGridEditor = true; // Toggle grid editor in inspector

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

        private void OnValidate()
        {
            // Initialize or resize walkableTiles array when grid size changes
            if (walkableTiles == null || walkableTiles.GetLength(0) != gridSize.x || walkableTiles.GetLength(1) != gridSize.y)
            {
                bool[,] newArray = new bool[gridSize.x, gridSize.y];
                
                // Copy existing values if array exists
                if (walkableTiles != null)
                {
                    int minX = Mathf.Min(walkableTiles.GetLength(0), gridSize.x);
                    int minY = Mathf.Min(walkableTiles.GetLength(1), gridSize.y);
                    
                    for (int x = 0; x < minX; x++)
                    {
                        for (int y = 0; y < minY; y++)
                        {
                            newArray[x, y] = walkableTiles[x, y];
                        }
                    }
                }
                
                walkableTiles = newArray;
            }
        }

        void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            // Clear existing tiles
            foreach (Transform child in transform)
            {
                #if UNITY_EDITOR
                    DestroyImmediate(child.gameObject);
                #else
                    Destroy(child.gameObject);
                #endif
            }

            // Generate new grid
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int z = 0; z < gridSize.y; z++)
                {
                    Vector3 position = gridPosition + new Vector3(
                        x * spacing, 
                        height, 
                        z * spacing
                    );

                    CreateTile(position, walkableTiles[x, z], x, z);
                }
            }
        }

        private void CreateTile(Vector3 position, bool isWalkable, int x, int z)
        {
            GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            tile.name = $"Tile_{x}_{z}";

            TileTrigger tileTrigger = tile.GetComponent<TileTrigger>();
            if (tileTrigger != null)
            {
                if(allowedCoordinates.Contains(new Vector2Int(x, z)))
                {
                    tileTrigger.SetWalkable(true);
                }
                else
                {
                    tileTrigger.SetWalkable(isWalkable);
                }
            }
        }

        // Method to set specific tile walkable status
        public void SetTileWalkable(int x, int z, bool walkable)
        {
            if (x >= 0 && x < gridSize.x && z >= 0 && z < gridSize.y)
            {
                walkableTiles[x, z] = walkable;
            }
        }

        // Method to set multiple tiles walkable status
        public void SetTilesWalkable(Vector2Int[] positions, bool walkable)
        {
            foreach (Vector2Int pos in positions)
            {
                SetTileWalkable(pos.x, pos.y, walkable);
            }
        }
    }
}
