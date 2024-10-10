using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;

public class RosImageSubscriber : MonoBehaviour
{
    public string rosImageTopic = "/image"; // Set to your ROS image topic
    public RawImage imageDisplay;           // Assign this to the Raw Image component on the Canvas
    public int targetFrameRate = 30;        // Target frame rate for updating the image

    private ROSConnection ros;
    private Texture2D texture;
    private byte[] correctedData;
    private bool dataReady = false;

    void Start()
    {
        // Initialize the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<ImageMsg>(rosImageTopic, ReceiveImage);

        // Initialize a coroutine for updating the texture periodically
        StartCoroutine(UpdateTextureCoroutine());
    }

    void ReceiveImage(ImageMsg imageMsg)
    {
        // Ensure the texture matches the image size, and reinitialize only if necessary
        if (texture == null || texture.width != imageMsg.width || texture.height != imageMsg.height)
        {
            texture = new Texture2D((int)imageMsg.width, (int)imageMsg.height, TextureFormat.RGB24, false);
            correctedData = new byte[imageMsg.data.Length];
            imageDisplay.texture = texture;
        }

        // Perform the BGR to RGB conversion and flip vertically
        int width = (int)imageMsg.width;
        int height = (int)imageMsg.height;
        int rowLength = width * 3; // Assuming RGB24 format (3 bytes per pixel)

        for (int y = 0; y < height; y++)
        {
            int srcRow = y * rowLength;
            int dstRow = (height - 1 - y) * rowLength;
            for (int x = 0; x < rowLength; x += 3)
            {
                correctedData[dstRow + x] = imageMsg.data[srcRow + x + 2];      // Red
                correctedData[dstRow + x + 1] = imageMsg.data[srcRow + x + 1];  // Green
                correctedData[dstRow + x + 2] = imageMsg.data[srcRow + x];      // Blue
            }
        }

        // Mark data as ready to be processed
        dataReady = true;
    }

    private IEnumerator UpdateTextureCoroutine()
    {
        while (true)
        {
            if (dataReady)
            {
                texture.LoadRawTextureData(correctedData);

                // Asynchronous GPU Readback for optimized performance
                AsyncGPUReadback.Request(texture, 0, (request) =>
                {
                    if (request.hasError)
                    {
                        Debug.LogWarning("GPU readback error detected.");
                    }
                    else
                    {
                        texture.Apply();
                        imageDisplay.texture = texture;
                    }
                });

                dataReady = false;
            }

            // Control the update frequency
            yield return new WaitForSeconds(1f / targetFrameRate);
        }
    }
}
