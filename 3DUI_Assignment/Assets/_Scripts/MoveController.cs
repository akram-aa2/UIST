using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEditor.PlayerSettings;

public class MoveController : MonoBehaviour
{
    [SerializeField] float _scrollSensitivity = 0.1f;
    [SerializeField] float _minDistance = 0.5f;
    [SerializeField] float _maxDistance = 2f;
    float _posZ = 0.5f;

    void Update()
    {
        /* ToDos:
         *  - move the Interaction with the 3D Controller along the worldspace z-axis with mouse scroll wheel delta value 
         *  - integrate the scrollSensitivity to control the amount/speed of movement for scolling
         *  - clamp z-axis movement to minDistance and maxDistance 
         *  Think about how to convert screen coordinates to world coordinates and make sure that the controller always points forward from the camera's perspective.
         */

        Vector3 mousePos = Input.mousePosition;
        _posZ += Input.mouseScrollDelta.y * _scrollSensitivity;
        _posZ = Mathf.Clamp(_posZ, _minDistance, _maxDistance);
        mousePos.z = _posZ;

        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
}