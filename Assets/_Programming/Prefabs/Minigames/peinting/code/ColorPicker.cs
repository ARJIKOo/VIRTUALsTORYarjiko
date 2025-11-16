using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPicker : MonoBehaviour, IPointerClickHandler
{
    public Color output;

    [Header("Link to Drawing Canvas")]
    public DrawingCanvas drawingCanvas;

    private Camera renderCamera;

    void Awake()
    {
        // მოვძებნოთ შესაბამისი Canvas და მისგან ავიღოთ worldCamera
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            renderCamera = canvas.worldCamera;
        }
        else
        {
            renderCamera = Camera.main;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        output = Pick(eventData.position, GetComponent<Image>());
        drawingCanvas.SetBrushColor(output);
    }

    public Color Pick(Vector2 screenPoint, Image imageToPick)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageToPick.rectTransform,
            screenPoint,
            renderCamera,
            out Vector2 localPoint
        );

        Rect rect = imageToPick.rectTransform.rect;
        Vector2 pivotAdjustedPoint = new Vector2(
            (localPoint.x + rect.width * 0.5f) / rect.width,
            (localPoint.y + rect.height * 0.5f) / rect.height
        );

        Texture2D t = imageToPick.sprite.texture;
        int texX = Mathf.Clamp(Mathf.FloorToInt(pivotAdjustedPoint.x * t.width), 0, t.width - 1);
        int texY = Mathf.Clamp(Mathf.FloorToInt(pivotAdjustedPoint.y * t.height), 0, t.height - 1);

        return t.GetPixel(texX, texY);
    }
}