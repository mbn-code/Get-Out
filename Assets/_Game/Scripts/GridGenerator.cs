// GridGenerator.cs

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
                tileTrigger.SetWalkable(isWalkable);
                
                // Change material color based on walkable status
                Renderer renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = isWalkable ? Color.green : Color.red;
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
