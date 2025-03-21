using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public string worldMapSceneName = "WorldMap"; // Name of your world map scene
    public int nextLevelNodeIndex; // Index of the next level node to unlock

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Assuming your player has the tag "Player"
        {
            // Unlock the next level node
            GameObject worldMapPlayer = GameObject.Find("World Map Player");
            if (worldMapPlayer != null)
            {
                WorldMapPlayerController worldMapPlayerController = worldMapPlayer.GetComponent<WorldMapPlayerController>();
                if (worldMapPlayerController != null)
                {
                    /*if (nextLevelNodeIndex < worldMapPlayerController.nodes.Count)
                    {
                        worldMapPlayerController.nodes[nextLevelNodeIndex].GetComponent<LevelNode>().isUnlocked = true;
                    }*/
                    worldMapPlayer.transform.position = worldMapPlayerController.GetPlayerTransform().position;
                }
            }

            // Load the world map scene
            SceneManager.LoadScene(worldMapSceneName);
        }
    }
}
