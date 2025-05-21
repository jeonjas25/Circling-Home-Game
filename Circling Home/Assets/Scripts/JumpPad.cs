using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpMultiplier = 2.0f; // Adjust this to control how much higher the player jumps

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is colliding with the jump pad
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // Apply the jump boost
            //playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f); // Important: Reset vertical velocity first.  No bounce.
            //playerRb.AddForce(Vector2.up * playerController.initialJumpForce * jumpMultiplier, ForceMode2D.Impulse);
            Debug.Log("Player jumped on Jump Pad");
            playerController.SetOnJumpPad(true); // Notify the player controller
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.SetOnJumpPad(false);
        }
    }
}
