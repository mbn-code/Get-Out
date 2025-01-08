using UnityEngine;
using UnityEngine.SceneManagement;

public class PositionBasedSceneLoader : MonoBehaviour
{
    [System.Serializable]
    public struct SceneLoadTrigger
    {
        public Vector3 targetPosition; // Position to trigger the scene change
        public string sceneName; // Name of the scene to load
    }

    [SerializeField] private SceneLoadTrigger[] sceneLoadTriggers; // Array of scene load triggers
    [SerializeField] private float threshold = 1.0f; // Distance threshold to consider as "reached"

    private void Update()
    {
        foreach (var trigger in sceneLoadTriggers)
        {
            if (Vector3.Distance(transform.position, trigger.targetPosition) <= threshold)
            {
                SceneManager.LoadScene(trigger.sceneName); // Load the new scene
                break; // Exit the loop after loading the scene
            }
        }
    }
}