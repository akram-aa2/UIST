using UnityEngine;

public class CircleDetection : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    public GameObject redSphere; // Reference to your red sphere object

    private Vector3 lastPosition;
    private int framesWithoutDetection = 0;

    // Start is called before the first frame update
    void Start()
    {
        webcamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();
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
            DetectRedCircle(texture);
        }
    }
    void DetectRedCircle(Texture2D frame)
    {
        UnityEngine.Color[] pixels = frame.GetPixels();
        int width = frame.width;
        int height = frame.height;

        int redPixelCount = 0;
        int redPixelXSum = 0;
        int redPixelYSum = 0;
        int centerX = 0;
        int centerY = 0;

        //TODO: Implement tracking of a red circle
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                UnityEngine.Color pixelColor = pixels[y * width + x];
                if (pixelColor.r > pixelColor.g*2 && pixelColor.r > pixelColor.b*2 && pixelColor.r > 0.4f)
                {
                    redPixelCount++;
                    redPixelXSum += x;
                    redPixelYSum += y;
                }
            }
        }

        if (redPixelCount > 20)
        {
            framesWithoutDetection = 0;
            redSphere.SetActive(true);
            centerX = redPixelXSum / redPixelCount;
            centerY = redPixelYSum / redPixelCount;
            // TODO: Move the tracking object (e.g., sphere) to the detected circle's center
            int newX = (centerX * Screen.width) / width;
            int newY = (centerY * Screen.height) / height;
            Vector3 screenPosition = new Vector3(newX, newY, Camera.main.nearClipPlane + 1f);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            redSphere.transform.position = worldPosition;
            lastPosition = worldPosition;
        }
        else
        {
            redSphere.transform.position = lastPosition;
            framesWithoutDetection++;
            if (framesWithoutDetection > 30)
            {
                redSphere.SetActive(false);
            }
        }
    }
}
