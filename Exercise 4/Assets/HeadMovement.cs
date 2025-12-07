using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private Vector3 lastPosition;
    public GameObject cameraRig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        webcamTexture = new WebCamTexture();
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

        //TODO: Implement tracking of a red circle
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                UnityEngine.Color pixelColor = pixels[y * width + x];
                if ((pixelColor.r > pixelColor.g*2 && pixelColor.r > pixelColor.b*2 && pixelColor.r > 0.4f) || (pixelColor.b > pixelColor.g*2 && pixelColor.b > pixelColor.r*2 && pixelColor.b > 0.4f))
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
            int newX = (centerX * Screen.width) / width;
            int newY = (centerY * Screen.height) / height;
            Vector3 screenPosition = new Vector3(newX, newY, Camera.main.nearClipPlane + 1f);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            cameraRig.transform.position = worldPosition;
            lastPosition = worldPosition;
        }
        else
        {
            cameraRig.transform.position = lastPosition;
        }
    }
}
