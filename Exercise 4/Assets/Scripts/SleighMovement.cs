using UnityEngine;

public class SleighMovement : MonoBehaviour
{
    [SerializeField] private float zSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private float xSpeed;
    [SerializeField] private float delayTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            return;
        }
        transform.Translate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, zSpeed * Time.deltaTime, Space.Self);
    }
}
