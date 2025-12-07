using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private Vector3 lastPosition;
    public GameObject cameraRig;
    public Vector3 centerPosition;
    [SerializeField] private float movementScale = 1.0f;
    [SerializeField] private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        centerPosition = cameraRig.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (webcamTexture.didUpdateThisFrame)
        {
            // Capture the webcam texture
            Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
            texture.SetPixels(webcamTexture.GetPixels());
            texture.Apply();

            // Perform image processing on the captured frame
            DetectGlasses(texture);
        }
    }

    void DetectGlasses(Texture2D frame)
    {
        UnityEngine.Color[] pixels = frame.GetPixels();
        int width = frame.width;
        int height = frame.height;

        int glassesPixelCount = 0;
        int glassesPixelXSum = 0;
        int glassesPixelYSum = 0;
        int centerX = 0;
        int centerY = 0;
        float newCenterX = 0;
        float newCenterY = 0;

        //TODO: Implement tracking of a red circle
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                UnityEngine.Color pixelColor = pixels[y * width + x];
                if (IsTargetHue(pixelColor))
                {
                    glassesPixelCount++;
                    glassesPixelXSum += x;
                    glassesPixelYSum += y;
                }
            }
        }

        if (glassesPixelCount > 20)
        {
            centerX = glassesPixelXSum / glassesPixelCount;
            centerY = glassesPixelYSum / glassesPixelCount;
            // TODO: Move the tracking object (e.g., sphere) to the detected circle's center
            newCenterX = (float)centerX/(float)width - 0.5f;
            newCenterY = (float)centerY/(float)height - 0.5f;
            Debug.Log("Glasses detected at: " + newCenterX + ", " + newCenterY);
            if (player == null)
            {
                cameraRig.transform.position = new Vector3(centerPosition.x - newCenterX * movementScale, centerPosition.y + newCenterY * movementScale, centerPosition.z);
                Debug.Log("No player assigned");
            }
            else 
            {
                // if a player object is assigned, move depending on player's orientation up down left right
                Vector3 right = player.transform.right;
                Vector3 up = player.transform.up;
                cameraRig.transform.position = centerPosition + (right * -newCenterX + up * newCenterY) * movementScale;
            }
            lastPosition = cameraRig.transform.position;
        }
        else
        {
            cameraRig.transform.position = lastPosition;
        }
    }

    bool IsTargetHue(UnityEngine.Color color)
    {
        // Convert RGB to HSV
        float h, s, v;
        UnityEngine.Color.RGBToHSV(color, out h, out s, out v);

        bool isRedHue = (h < 0.083f || h > 0.916f);
        bool isBlueHue = (h > 0.57f && h < 0.77f);

        bool isSaturated = s > 0.3f;

        return (isRedHue || isBlueHue) && isSaturated;
    }
}
