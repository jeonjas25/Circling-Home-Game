using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    private bool isVisible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(float currentHealth)
    {
        slider.value = currentHealth;
        ShowHealthBar();
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void ShowHealthBar()
    {
        if (!isVisible)
        {
            gameObject.SetActive(true);
            isVisible = true;
        }
    }

    public void HideHealthBar()
    {
        gameObject.SetActive(false);
        isVisible = false;
    }
}
