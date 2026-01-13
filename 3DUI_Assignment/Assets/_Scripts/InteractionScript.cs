using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class InteractionScript : MonoBehaviour
{

    [SerializeField] Transform _controller;
    [SerializeField] GameObject _lighterFlame;

    [Header("Current Object")]
    [SerializeField] GameObject _currentObject;
    [SerializeField] bool _selected = false;

    [Header("Lamp Dimmer")]
    [SerializeField] Light _pointLight;
    [SerializeField] float _minIntensity = 0f;
    [SerializeField] float _maxIntensity = 5f;
    [SerializeField] Transform _minPos;
    [SerializeField] Transform _maxPos;


    private void Update()
    {
        /* ToDos:
         *  - Grasping: While left mouse button is pressed, pick up and move around the lighter and all the candles with the controller
         *  - When the controller collides with the lighter GameObject, press the "T" key to turn on the lighter flame 
         *  - Move the slider ("DimmerSwitch" GameObject) vertically/up&down with the controller to change the light intensity of the lamp. Map the
         *    position of the slider to an intensity value between 0 - 5.
         *  Use the already assigned tags to differentiate between the interactable objects. Think about how you can constrain the movement of
         *  the DimmerSwitch to the y-axis and keep it within the minPos and maxPos Anchors of the light switch. 
         */
        if (_currentObject != null)
        {
            // Grasping
            if (Input.GetMouseButtonDown(0) && !_selected)
            {
                if (_currentObject.CompareTag("Lighter") || _currentObject.CompareTag("Candle"))
                {
                    _currentObject.transform.SetParent(_controller);
                    _selected = true;
                }
            }
            if (Input.GetMouseButtonUp(0) && _selected)
            {
                if (_currentObject.CompareTag("Lighter") || _currentObject.CompareTag("Candle"))
                {
                    _currentObject.transform.SetParent(null);
                    _selected = false;
                }
            }

            // Lighter Flame Toggle
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (_currentObject.CompareTag("Lighter"))
                {
                    _lighterFlame.SetActive(!_lighterFlame.activeSelf);
                }
            }

            // Dimmer Switch
            if (_currentObject.CompareTag("Dimmer") && Input.GetMouseButton(0))
            {
                Vector3 controllerPos = _controller.position;
                Vector3 dimmerPos = _currentObject.transform.position;
                dimmerPos.y = Mathf.Clamp(controllerPos.y, _minPos.position.y, _maxPos.position.y);
                _currentObject.transform.position = dimmerPos;

                float t = (dimmerPos.y - _minPos.position.y) / (_maxPos.position.y - _minPos.position.y);
                _pointLight.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, t);
            }
        }
    }



    /* ToDos:
     *  - Assign/remove the currentObject that the controller collides with
     *  - Implement a "Hover" Interaction State for the interactable objects (lighter, candles, slider/dimmer), activated when the controller collides with them
     *    -> Hint: Use the material's emission keyword. You just need to enable/disable the emission on the objects' default/first material
     */
    private void OnTriggerEnter(Collider other)
    {  
        if (!_selected)
        {
            if (other.gameObject.CompareTag("Lighter") || other.gameObject.CompareTag("Candle") || other.gameObject.CompareTag("Dimmer"))
            {
                _currentObject = other.gameObject;
                // Enable Emission
                Renderer[] objRenderers = _currentObject.GetComponentsInChildren<Renderer>();
                if (objRenderers != null)
                {
                    Material objMaterial = objRenderers[0].materials[0];
                    objMaterial.EnableKeyword("_EMISSION");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _currentObject)
        {
            // Disable Emission
            Renderer[] objRenderers = _currentObject.GetComponentsInChildren<Renderer>();
            if (objRenderers != null)
            {
                Material objMaterial = objRenderers[0].materials[0];
                objMaterial.DisableKeyword("_EMISSION");
            }
            _currentObject = null;
        }
    }

}
