using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // The speed at which the player moves.
    public float moveSpeed = 5f; 

    void Update()
    {
        // Get the horizontal and vertical input values.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on the input values.
        Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput);

        // Normalize the movement direction to ensure consistent movement speed.
        movementDirection.Normalize();

        // Move the player in the calculated direction.
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
    }

}