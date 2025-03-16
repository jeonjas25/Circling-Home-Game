using UnityEngine;
using UnityEngine.UI;

public class AttackModeUI : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public Image meleeIcon;
    public Sprite meleeIconHighlighted;
    public Sprite meleeIconUnhighlighted;
    public Image rangedIcon;
    public Sprite rangedIconHighlighted;
    public Sprite rangedIconUnhighlighted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCombat.isMeleeMode)
        {
            meleeIcon.sprite = meleeIconHighlighted;
            rangedIcon.sprite = rangedIconUnhighlighted;
        }
        else 
        {
            meleeIcon.sprite = meleeIconUnhighlighted;
            rangedIcon.sprite = rangedIconHighlighted;
        }
    }
}
