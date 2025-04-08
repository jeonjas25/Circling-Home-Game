using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    public Image foregroundImage;
    public float maxCharge = 100f;
    private float currentCharge = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateChargeBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCharge(float chargeAmount)
    {
        currentCharge += chargeAmount;
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        UpdateChargeBar();
    }

    public void ResetCharge()
    {
        currentCharge = 0f;
        UpdateChargeBar();
    }

    public bool IsChargeFull()
    {
        return currentCharge >= maxCharge;
    }

    private void UpdateChargeBar()
    {
        if (foregroundImage != null)
        {
            foregroundImage.fillAmount = currentCharge / maxCharge;
        }
    }
}
