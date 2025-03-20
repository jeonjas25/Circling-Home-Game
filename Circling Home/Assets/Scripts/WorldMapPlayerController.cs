using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WorldMapPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public List<Transform> nodes;
    private int currentNodeIndex = 0;
    private bool isMoving = false;
    private LevelNode currentNode; //store the current node.

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && currentNodeIndex < nodes.Count - 1)
            {
                if (nodes[currentNodeIndex + 1].GetComponent<LevelNode>().isUnlocked)
                {
                    currentNodeIndex++;
                    isMoving = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentNodeIndex > 0)
            {
                if (nodes[currentNodeIndex - 1].GetComponent<LevelNode>().isUnlocked)
                {
                    currentNodeIndex--;
                    isMoving = true;
                }
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, nodes[currentNodeIndex].position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, nodes[currentNodeIndex].position) < 0.01f)
            {
                isMoving = false;
            }
        }

        // Check for Return key press within Update
        if(currentNode != null && currentNode.isUnlocked && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(currentNode.levelSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        LevelNode levelNode = other.GetComponent<LevelNode>();
        if (levelNode != null && levelNode.isUnlocked)
        {
            currentNode = levelNode;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        LevelNode levelNode = other.GetComponent<LevelNode>();
        if(levelNode == currentNode)
        {
            currentNode = null;
        }
    }
}
