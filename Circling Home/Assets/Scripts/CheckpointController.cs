using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public Sprite lanternOff;
    public Sprite lanternOn;
    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = lanternOff;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        isActivated = true;
        spriteRenderer.sprite = lanternOn;
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (player != null)
        {
            player.respawnPosition = transform.position;
        }
    }
}
