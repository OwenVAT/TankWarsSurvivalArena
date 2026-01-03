using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Transform followTank;
    [SerializeField] private Vector3 offset = new Vector3(0, 4f, 0);

    private Color defaultColor;

    private void Awake()
    {
        if (fillImage != null)
            defaultColor = fillImage.color;
    }

    public void BindHealthBarToTank(Transform tankPostition)
    {
        followTank = tankPostition;
    }

    private void LateUpdate()
    {
        transform.position = followTank.position + offset;
    }

    public void SetHealth_UI(float currentHealth, float maxHealth)
    {
        fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    public void Flash(Color color, float duration = 0.12f)
    {
        if (fillImage == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        fillImage.color = color;
        yield return new WaitForSeconds(duration);
        fillImage.color = defaultColor;
    }
}
