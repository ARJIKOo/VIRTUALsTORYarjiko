using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DrawingCanvas : MonoBehaviour
{
    [Header("Drawing")]
    public RawImage drawArea;
    public Color brushColor = Color.black;
    public int brushSize = 5;

    [Header("Brush Size Settings")]
    public int minBrushSize = 1;
    public int maxBrushSize = 100;
    public int brushStep   = 3;   // რამდენით გაიზარდოს/დაპატარავდეს ერთ დაწკაპებაზე

    private Texture2D texture;
    private RectTransform rectTransform;

    private const int textureWidth  = 512;
    private const int textureHeight = 512;

    private Vector2? lastDrawPos = null;
    public GameObject targetObject; // ეს დააკონექტე Unity-ში იმ ობიექტზე, რომელზეც გინდა რომ ჩანდეს ნახატი

    private Color previousColor;   // შევინახოთ უკანა ფუნჯი
    private bool  isEraser = false;
    
    Camera renderCamera;


    /* ───────────────────────── INIT ───────────────────────── */
    void Awake()
    {
        rectTransform = drawArea.GetComponent<RectTransform>();

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        drawArea.texture = texture;
        ClearCanvas();
        renderCamera = GetComponentInParent<Canvas>()?.worldCamera;
    
        if (renderCamera == null)
            renderCamera = Camera.main; // fallback
    }

    private void Start()
    {
        StartCoroutine(CallSaveDrawingAfterDelay(58f));
    }

    /* ───────────────────────── UPDATE ───────────────────────── */
    void Update()
    {
#if UNITY_EDITOR
    if (Input.GetMouseButton(0))
        Draw(Input.mousePosition);
    else
        lastDrawPos = null;
#else
        // Use both mouse and touch in builds
        if (Input.touchCount > 0)
            Draw(Input.GetTouch(0).position);
        else if (Input.GetMouseButton(0))
            Draw(Input.mousePosition);
        else
            lastDrawPos = null;
#endif
    }

    /* ───────────────────────── DRAW CORE ───────────────────────── */
    void Draw(Vector2 screenPos)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, renderCamera, out var local))
            return;

        float normX = (local.x + rectTransform.rect.width  * 0.5f) / rectTransform.rect.width;
        float normY = (local.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height;

        int x = Mathf.FloorToInt(normX * texture.width);
        int y = Mathf.FloorToInt(normY * texture.height);

        if (lastDrawPos == null)
        {
            DrawCircle(x, y);
            lastDrawPos = new Vector2(x, y);
        }
        else
        {
            DrawLine(lastDrawPos.Value, new Vector2(x, y));
            lastDrawPos = new Vector2(x, y);
        }

        texture.Apply();
    }

    void DrawCircle(int cx, int cy)
    {
        int r = brushSize / 2;
        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
                if (x * x + y * y <= r * r)
                {
                    int px = Mathf.Clamp(cx + x, 0, texture.width  - 1);
                    int py = Mathf.Clamp(cy + y, 0, texture.height - 1);
                    texture.SetPixel(px, py, brushColor);
                }
    }
    
    /* UI‑დან შესაძლებელია ორი ღილაკი ან Toggle */
    public void EnableEraser()
    {
        if (isEraser) return;
        previousColor = brushColor;
        brushColor    = new Color(1, 1, 1, 0); // α = 0 → წაშლის
        isEraser      = true;
    }

    public void DisableEraser()
    {
        if (!isEraser) return;
        brushColor = previousColor;
        isEraser   = false;
    }

/* თუ Toggle ღილაკი გირჩევნია */
    public void ToggleEraser()
    {
        if (isEraser) DisableEraser();
        else          EnableEraser();
    }


    void DrawLine(Vector2 a, Vector2 b)
    {
        int points = (int)Vector2.Distance(a, b);
        for (int i = 0; i <= points; i++)
        {
            Vector2 p = Vector2.Lerp(a, b, i / (float)points);
            DrawCircle((int)p.x, (int)p.y);
        }
    }

    /* ───────────────────────── PUBLIC API ───────────────────────── */

    public void ClearCanvas()
    {
        var clear = new Color(1, 1, 1, 0);
        var arr   = new Color[textureWidth * textureHeight];
        for (int i = 0; i < arr.Length; i++) arr[i] = clear;

        texture.SetPixels(arr);
        texture.Apply();
    }

    public void SetBrushColor(Color c)  => brushColor = c;

    public void SetBrushSize(int size)  => brushSize  = Mathf.Clamp(size, minBrushSize, maxBrushSize);

    /* -----  UI Buttons  ----- */
    public void IncreaseBrushSize() => SetBrushSize(brushSize + brushStep);
    public void DecreaseBrushSize() => SetBrushSize(brushSize - brushStep);
    
    
    private IEnumerator CallSaveDrawingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SaveDrawing();
    }

    public void SaveDrawing()
    {
        // 1. Texture → Sprite
        Sprite drawnSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // 2. Assign sprite to target object
        if (targetObject != null)
        {
            var renderer = targetObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = drawnSprite;
                Debug.Log("✅ Drawing applied to target GameObject.");
            }
            else
            {
                Debug.LogWarning("⚠️ Target GameObject has no SpriteRenderer component!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No target GameObject assigned!");
        }
    }
}
