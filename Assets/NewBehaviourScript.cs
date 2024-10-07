using UnityEngine;
using UnityEngine.XR;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class MetaJoystickPublisher : MonoBehaviour
{
    public string rosTopic = "/mavic_1/gimbal_cmd";
    private ROSConnection ros;
    private XRNode leftHand = XRNode.LeftHand;

    void Start()
    {
        // Initialize the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Vector3Msg>(rosTopic);
    }

    void Update()
    {
        // Fetch joystick input from the left controller
        if (InputDevices.GetDeviceAtXRNode(leftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickPosition))
        {
            // Create a Vector3 message (or Vector2 if that’s sufficient for your application)
            Vector3Msg msg = new Vector3Msg
            {
                x = 0.0,
                y = joystickPosition.y/2.0,
                z = joystickPosition.x/2.0 // set to zero or another value as needed
            };

            // Publish the message to ROS2
            ros.Publish(rosTopic, msg);
        }
    }
}
