using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchReceiverManager : MonoBehaviour
{
    GameObject touchedObject = null;
    bool isTouching = false;

    void Start()
    {
        // This has to be called once in the entire project
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

    void FindTouchedObject()
    {
        var touch = Touch.activeTouches[0];
        int fingerId = touch.finger.index;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            touchedObject = hit.collider.gameObject;
            if (touchedObject != null && touchedObject.GetComponent<TouchReceiver>() != null)
            {
                Debug.Log("Touched Object: " + touchedObject.name);
                touchedObject.GetComponent<TouchReceiver>().IsTouching = true;
                isTouching = true;
            }
        }
        else
        {
            touchedObject = null;
            isTouching = false;
        }
    }
    
}