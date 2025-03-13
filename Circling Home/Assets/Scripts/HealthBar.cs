using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
    public PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController != null && foregroundImage != null)
        {
            foregroundImage.fillAmount = playerController.currentHealth / playerController.maxHealth;
        }
    }
}
