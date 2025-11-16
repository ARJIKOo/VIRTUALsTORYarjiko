using UnityEngine;

public class WebViewController : MonoBehaviour
{
    private WebViewObject webViewObject;
    public string MyWebLink;

    void Start()
    {
        webViewObject = gameObject.AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) => {
                Debug.Log($"WebView message: {msg}");
            },
            err: (msg) => {
                Debug.LogError($"WebView error: {msg}");
            },
            httpErr: (msg) => {
                Debug.LogError($"HTTP error: {msg}");
            },
            started: (msg) => {
                Debug.Log($"WebView started: {msg}");
            },
            ld: (msg) => {
                Debug.Log($"WebView loaded: {msg}");
            }
        );

        webViewObject.SetMargins(0, 0, 0, 0); // Fullscreen
        webViewObject.SetVisibility(true);

        // აქ ჩაწერე Hololink URL
        webViewObject.LoadURL(MyWebLink);
    }
}