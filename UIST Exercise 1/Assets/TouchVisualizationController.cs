using System.Collections.Generic;
using UnityEngine;

// Attach this script to a GameObject
// Assign a Sprite asset to the 'touchSprite' field in the Inspector
public class TouchVisualizationController : MonoBehaviour
{
    public Sprite touchSprite; // Assign a Sprite with a circle or similar

    private Dictionary<int, GameObject> activeTouchIndicators = new Dictionary<int, GameObject>();

    private Camera mainCamera;
    private string onGuiText = "No touch contacts";
    private GUIStyle guiStyle;

    void Start()
    {
        mainCamera = Camera.main;

        if (touchSprite == null)
        {
            Debug.LogError("Touch Sprite not assigned. Please assign a Sprite asset to 'touchSprite'.");
            enabled = false;
            return;
        }

        // Initialize GUI Style for green text
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 12;
        guiStyle.normal.textColor = Color.green;
    }

    void Update()
    {
        List<int> currentFingerIds = new List<int>();
        string currentDisplayText = "";

        int touchCount = Input.touchCount;

        // ***** Touch support (use real touches on device) *****
        if (touchCount > 0)
        {
            for (int i = 0; i < touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                int fingerId = touch.fingerId;
                currentFingerIds.Add(fingerId);

                // GUI Text
                currentDisplayText += $"Touch {i} (ID: {fingerId}):\n" +
                                      $"  Screen Pos: {touch.position}\n" +
                                      $"  Phase: {touch.phase}\n" +
                                      $"  Delta: {touch.deltaPosition}\n\n";

                // --- Convert touch to world position ---
                float zDepth = Mathf.Abs(mainCamera.transform.position.z); // Usually 10 if camera is at (0,0,-10)
                Vector3 screenPoint = new Vector3(touch.position.x, touch.position.y, zDepth);
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPoint);

                if (touch.phase == TouchPhase.Began)
                {
                    CreateIndicator(fingerId, worldPos);
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    UpdateIndicator(fingerId, worldPos);
                }
            }
        }
        else
        {
            currentDisplayText = "No touch contacts";
        }

        // ***** Mouse support for Editor testing (simulate finger 42) *****
        #if UNITY_EDITOR
        if (touchCount == 0)
        {
            int mouseFingerId = 42;
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Input.mousePosition;
                currentFingerIds.Add(mouseFingerId);

                currentDisplayText += $"MOUSE (ID: {mouseFingerId}):\n" +
                                      $"  Screen Pos: {mousePos}\n";

                float zDepth = Mathf.Abs(mainCamera.transform.position.z);
                Vector3 screenPoint = new Vector3(mousePos.x, mousePos.y, zDepth);
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPoint);

                if (Input.GetMouseButtonDown(0))
                    CreateIndicator(mouseFingerId, worldPos);
                else
                    UpdateIndicator(mouseFingerId, worldPos);
            }
        }
        #endif

        onGuiText = currentDisplayText;

        // Clean up indicators for fingers that have lifted/canceled
        List<int> fingerIdsToRemove = new List<int>();
        foreach (var entry in activeTouchIndicators)
        {
            if (!currentFingerIds.Contains(entry.Key))
            {
                fingerIdsToRemove.Add(entry.Key);
            }
        }
        foreach (int fingerId in fingerIdsToRemove)
        {
            if (activeTouchIndicators.TryGetValue(fingerId, out GameObject indicatorToDestroy))
            {
                Destroy(indicatorToDestroy);
                activeTouchIndicators.Remove(fingerId);
            }
        }
    }

    private void CreateIndicator(int fingerId, Vector3 worldPos)
    {
        if (activeTouchIndicators.ContainsKey(fingerId))
            return;

        GameObject indicator = new GameObject($"TouchIndicator_{fingerId}");
        SpriteRenderer sr = indicator.AddComponent<SpriteRenderer>();
        sr.sprite = touchSprite;
        sr.sortingOrder = 100; // Draw on top
        indicator.transform.position = worldPos;
        indicator.transform.localScale = Vector3.one * 1.0f; // You can adjust this

        activeTouchIndicators.Add(fingerId, indicator);
    }

    private void UpdateIndicator(int fingerId, Vector3 worldPos)
    {
        if (activeTouchIndicators.TryGetValue(fingerId, out GameObject indicator))
        {
            indicator.transform.position = worldPos;
        }
    }

    // Draws the green text overlay
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 700, 500), onGuiText, guiStyle);
    }

    void OnDisable()
    {
        ClearAllIndicators();
    }

    void OnDestroy()
    {
        ClearAllIndicators();
    }

    private void ClearAllIndicators()
    {
        foreach (var entry in activeTouchIndicators)
        {
            if (entry.Value != null)
            {
                Destroy(entry.Value);
            }
        }
        activeTouchIndicators.Clear();
    }
}
