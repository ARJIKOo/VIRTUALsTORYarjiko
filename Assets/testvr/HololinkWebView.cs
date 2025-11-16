using UnityEngine;
using System.Collections;

public class HololinkWebView : MonoBehaviour
{
    public string hololinkUrl = "https://hololink.app/experience/yourID";
    WebViewObject webViewObject;

    // UI Buttons (optional) - hook these in the inspector
    public GameObject openButton;
    public GameObject closeButton;
    public GameObject loadingSpinner; // optional spinner object

    IEnumerator Start()
    {
        // Create WebViewObject at runtime
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();

        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log("[WebView] message from web: " + msg);
                if (msg == "CloseHololink") CloseHololink();
            },
            err: (msg) =>
            {
                Debug.LogError("[WebView] err: " + msg);
            },
            started: (msg) =>
            {
                Debug.Log("[WebView] started: " + msg);
            },
            ld: (msg) =>
            {
                Debug.Log("[WebView] loaded: " + msg);
                // Hide spinner when loaded
                if (loadingSpinner) loadingSpinner.SetActive(false);
            }
        );

        // full screen (0,0,0,0) margins
        webViewObject.SetMargins(0, 0, 0, 0);
        webViewObject.SetVisibility(false);

        // Make sure WebView is behind Unity UI (if using overlay UI)
        // Some versions require SetTransform or z-order handling — test on device.

        // Hide close button initially
        if (closeButton) closeButton.SetActive(false);
        if (loadingSpinner) loadingSpinner.SetActive(false);

        yield break;
    }

    public void OpenHololink()
    {
#if UNITY_ANDROID
        RequestAndroidCameraPermissionIfNeeded();
#endif
        if (loadingSpinner) loadingSpinner.SetActive(true);
        webViewObject.SetVisibility(true);

        // Load URL and show
        webViewObject.LoadURL(hololinkUrl);

        if (openButton) openButton.SetActive(false);
        if (closeButton) closeButton.SetActive(true);
    }

    public void CloseHololink()
    {
        if (webViewObject != null)
        {
            webViewObject.SetVisibility(false);
            webViewObject.LoadURL("about:blank");
        }

        if (openButton) openButton.SetActive(true);
        if (closeButton) closeButton.SetActive(false);
        if (loadingSpinner) loadingSpinner.SetActive(false);
    }

#if UNITY_ANDROID
    void RequestAndroidCameraPermissionIfNeeded()
    {
        const string camPerm = "android.permission.CAMERA";
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(camPerm))
        {
            UnityEngine.Android.Permission.RequestUserPermission(camPerm);
        }
    }
#endif
}
