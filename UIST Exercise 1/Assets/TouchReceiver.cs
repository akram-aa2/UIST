using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public partial class TouchReceiver : MonoBehaviour
{

    [SerializeField]
    Mesh debugMesh;
    [SerializeField]
    Material startMat;
    [SerializeField]
    Material currentMat;

    float distance = 0f;
    int numberOfFingers = 0;
    public bool IsTouching = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // This has to be called once in the entire project
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        if (IsTouching)
        {
            DrawDebugMeshes();
            HandleTouchInput();
        }
    }

    void HandleTouchInput()
    {
        //TODO
        //Simple example:
        if (Touch.activeTouches.Count == 0)
        {
            distance = 0f;
            numberOfFingers = 0;
            IsTouching = false;
        }
        else if (Touch.activeTouches.Count >= 2)
        {
            numberOfFingers = Touch.activeTouches.Count;
            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];

            Vector3 worldPos0 = Camera.main.ScreenToWorldPoint(new Vector3(touch0.screenPosition.x, touch0.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3 worldPos1 = Camera.main.ScreenToWorldPoint(new Vector3(touch1.screenPosition.x, touch1.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            Vector3 midPoint = (worldPos0 + worldPos1) / 2f;
            transform.position = midPoint;
            float currentDistance = Vector3.Distance(worldPos0, worldPos1);
            if (distance == 0f)
                distance = currentDistance;
            else
            {
                float delta = currentDistance - distance;
                transform.localScale += new Vector3(delta, delta, delta);
                distance = currentDistance;
            }

            var currentRotation = Quaternion.FromToRotation(Vector3.right, (worldPos1 - worldPos0).normalized);
            transform.rotation = currentRotation;
        }
        else if (Touch.activeTouches.Count == 1 && numberOfFingers == 0)
        {
            var touch = Touch.activeTouches[0];
            int fingerId = touch.finger.index;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            transform.position = worldPos;
        }
    }

    void DrawDebugMeshes()
    {
        if(Touch.activeTouches.Count > 0)
        {
            // Draw debug mesh for the every touch point
            for(int i = 0; i < Touch.activeTouches.Count; i++)
            {
                Touch touch = Touch.activeTouches[i];
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));
                DrawDebugMeshFor(worldPos);
            }
        }

    }

    private void DrawDebugMeshFor(Vector3 touchPointVector)
    {
        Graphics.DrawMesh(debugMesh,
                            Matrix4x4.TRS(touchPointVector, Quaternion.identity,
                                new Vector3(0.05f, 0.05f, 0.05f)), currentMat, 0);
    }
}
