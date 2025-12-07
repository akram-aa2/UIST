using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // The speed at which the player moves.
    public float moveSpeed = 5f; 
    public float rotationAngle = 0.5f;
    [SerializeField] private HeadMovement headMovement;

    void Update()
    {
        // Get the horizontal and vertical input values.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on the input values.
        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput);
        movementDirection = transform.rotation * movementDirection;

        // Normalize the movement direction to ensure consistent movement speed.
        movementDirection.Normalize();

        // Move the player in the calculated direction.
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
        headMovement.centerPosition += movementDirection * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0f, -rotationAngle, 0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0f, rotationAngle, 0f);
        }
    }

}