using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("Breathing Effect")]
    public bool enableBreath = true;
    public float breathScaleAmount = 0.01f;
    public float breathSpeed = 1f;

    [Header("Sway Effect")]
    public bool enableSway = true;
    public float swayAmount = 5f; // in pixels
    public float swaySpeed = 1f;

    private RectTransform rectTransform;
    private Vector3 originalLocalPos;
    private Vector3 originalScale;
    private float breathTime;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalLocalPos = rectTransform.localPosition;
        originalScale = rectTransform.localScale;
    }

    void Update()
    {
        if (enableBreath)
        {
            breathTime += Time.deltaTime * breathSpeed;
            float scale = 1f + Mathf.Sin(breathTime) * breathScaleAmount;
            rectTransform.localScale = new Vector3(scale, scale, 1f);
        }

        if (enableSway)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount * 0.5f;
            rectTransform.localPosition = originalLocalPos + new Vector3(swayX, swayY, 0f);
        }
    }

}