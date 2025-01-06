using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private PlatformTrigger[] platforms; // Array of platform triggers
    [SerializeField] private GameObject[] cubes; // Array of cubes

    private void Start()
    {
        foreach (var platform in platforms)
        {
            platform.OnCorrectObjectPlaced += CheckAllPlatforms;
        }
    }

    private void CheckAllPlatforms()
    {
        foreach (var platform in platforms)
        {
            if (!platform.IsCorrectObjectPlaced)
            {
                return; // If any platform does not have the correct object, exit the method
            }
        }

        // All platforms have the correct objects
        PerformAllCorrectAction();
    }

    private void PerformAllCorrectAction()
    {
        Debug.Log("All cubes are on the correct platforms!");
        // delete a 4th cube from the scene when all cubes are on the correct platforms
        Destroy(cubes[3]);
    }
}
