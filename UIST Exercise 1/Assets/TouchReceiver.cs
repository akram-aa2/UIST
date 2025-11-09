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

    int numberOfFingers = 0;
    public bool IsTouching = false;
    Vector3 previousPosition1 = Vector3.zero;
    Vector3 previousPosition2 = Vector3.zero;
    float previousDistance = 0f;
    float previousRotation = 0f;
    
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
            numberOfFingers = 0;
            IsTouching = false;
            previousPosition1 = Vector3.zero;
            previousPosition2 = Vector3.zero;
            previousDistance = 0f;
            previousRotation = 0f;
            return;
        }
        else if (Touch.activeTouches.Count >= 2)
        {
            var touch1 = Touch.activeTouches[0];
            var touch2 = Touch.activeTouches[1];
            int fingerId1 = touch1.finger.index;
            int fingerId2 = touch2.finger.index;
            Vector3 worldPos1 = Camera.main.ScreenToWorldPoint(new Vector3(touch1.screenPosition.x, touch1.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3 worldPos2 = Camera.main.ScreenToWorldPoint(new Vector3(touch2.screenPosition.x, touch2.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            // Move
            Vector3 midPoint = (worldPos1 + worldPos2) / 2f;
            Vector3 previousMidPoint = (previousPosition1 + previousPosition2) / 2f;
            if (numberOfFingers != 2)
            {
                previousPosition1 = worldPos1;
                previousPosition2 = worldPos2;
            }
            else
            {
                Vector3 diff = midPoint - previousMidPoint;
                transform.position += diff;
            }

            // Scale
            float currentDistance = Vector3.Distance(worldPos1, worldPos2);
            if (numberOfFingers != 2)
            {
                previousDistance = currentDistance;
            }
            else
            {
                float scaleFactor = currentDistance / previousDistance;
                transform.localScale *= scaleFactor;
                previousDistance = currentDistance;
            }

            // Rotate
            float currentRotation = Mathf.Atan2(worldPos2.y - worldPos1.y, worldPos2.x - worldPos1.x) * Mathf.Rad2Deg;
            if (numberOfFingers != 2)
            {
                previousRotation = currentRotation;
            }
            else
            {
                float rotationDiff = currentRotation - previousRotation;
                transform.Rotate(Vector3.forward, rotationDiff);
                previousRotation = currentRotation;
            }

            previousPosition1 = worldPos1;
            previousPosition2 = worldPos2;
            numberOfFingers = 2;
        }
        else if (Touch.activeTouches.Count == 1)
        {
            // Moving object with one finger
            var touch = Touch.activeTouches[0];
            int fingerId = touch.finger.index;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));

            if (numberOfFingers != 1)
            {
                previousPosition1 = worldPos;
                numberOfFingers = 1;
            }
            else
            {
                Vector3 diff = worldPos - previousPosition1;
                transform.position += diff;
                previousPosition1 = worldPos;
            }
            previousPosition2 = Vector3.zero;
            previousDistance = 0f;
            previousRotation = 0f;
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
