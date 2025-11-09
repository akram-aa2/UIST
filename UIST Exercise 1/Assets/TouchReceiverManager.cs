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
    
    // Finds the topmost touched object with a TouchReceiver component, by using raycasting
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

        // Get the hit with the highest render queue, which is visually on top
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

        // If the top hit object has a TouchReceiver component, set it as the touched object
        // so that it can handle the touch input + put it on top visually
        if (topHitObject != null && topHitObject.GetComponent<TouchReceiver>() != null)
        {
            touchedObject = topHitObject;
            isTouching = true;

            var receiver = touchedObject.GetComponent<TouchReceiver>();
            receiver.IsTouching = true;

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