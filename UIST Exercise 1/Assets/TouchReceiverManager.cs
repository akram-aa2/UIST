using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchReceiverManager : MonoBehaviour
{
    GameObject touchedObject = null;
    bool isTouching = false;

    private int currentTopQueue = 3000;

    void Start()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Touch.activeTouches.Count == 0)
        {
            touchedObject = null;
            isTouching = false;
            return;
        }
        else if (isTouching == false)
        {
            FindTouchedObject();
        }
    }

    // void FindTouchedObject()
    // {
    //     var touch = Touch.activeTouches[0];
    //     int fingerId = touch.finger.index;
    //     Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));
    //     Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
    //     RaycastHit hit;
    //     if (Physics.Raycast(ray, out hit))
    //     {
    //         touchedObject = hit.collider.gameObject;
    //         if (touchedObject != null && touchedObject.GetComponent<TouchReceiver>() != null)
    //         {
    //             touchedObject.GetComponent<TouchReceiver>().IsTouching = true;
    //             isTouching = true;
    //         }
    //     }
    //     else
    //     {
    //         touchedObject = null;
    //         isTouching = false;
    //     }
    // }
    
    void FindTouchedObject()
    {
        var touch = Touch.activeTouches[0];
        Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length == 0)
        {
            touchedObject = null;
            isTouching = false;
            return;
        }

        // Get the hit with the highest render queue (i.e. topmost visually)
        GameObject topHitObject = null;
        int highestQueue = int.MinValue;

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                int rq = rend.material.renderQueue;
                if (rq > highestQueue)
                {
                    highestQueue = rq;
                    topHitObject = hit.collider.gameObject;
                }
            }
        }

        if (topHitObject != null && topHitObject.GetComponent<TouchReceiver>() != null)
        {
            touchedObject = topHitObject;
            isTouching = true;

            var receiver = touchedObject.GetComponent<TouchReceiver>();
            receiver.IsTouching = true;

            // Bring visually to front by increasing render queue
            Renderer r = touchedObject.GetComponent<Renderer>();
            if (r != null)
            {
                currentTopQueue++;
                r.material.renderQueue = currentTopQueue;
            }
        }
        else
        {
            touchedObject = null;
            isTouching = false;
        }
    }
}