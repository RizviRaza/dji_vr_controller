using UnityEngine;
using UnityEngine.XR;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class MetaJoystickPublisher : MonoBehaviour
{
    public string gimbalcontrolTopic = "/mavic_1/gimbal_vel_cmd";
    public string gimbalresetTopic = "/mavic_1/gimbal_cmd";
    private ROSConnection ros;
    private XRNode leftHand = XRNode.LeftHand;

    void Start()
    {
        // Initialize the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Vector3Msg>(gimbalcontrolTopic);
        ros.RegisterPublisher<Vector3Msg>(gimbalresetTopic);

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
            ros.Publish(gimbalcontrolTopic, msg);
        }

        // Check for the X button press
        if (InputDevices.GetDeviceAtXRNode(leftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed) && isXPressed)
        {
            // Create a Vector3 message with all zeros for the gimbal command
            Vector3Msg gimbalCmdMsg = new Vector3Msg
            {
                x = 0,
                y = 0,
                z = 0
            };

            // Publish the gimbal command
            ros.Publish(gimbalresetTopic, gimbalCmdMsg);
        }
    }
}
