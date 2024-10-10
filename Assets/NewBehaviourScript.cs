using UnityEngine;
using UnityEngine.XR;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class MetaJoystickPublisher : MonoBehaviour
{
    public string gimbalcontrolTopic = "/mavic_1/gimbal_vel_cmd";
    public string gimbalresetTopic = "/mavic_1/gimbal_cmd";
    public string cmdVelTopic = "/mavic_1/cmd_vel";

    private ROSConnection ros;
    private XRNode leftHand = XRNode.LeftHand;
    private XRNode rightHand = XRNode.RightHand;

    private float deadzone = 0.2f;       // Deadzone to allow more freedom around the center for both gimbal and movement
    private float speedMultiplier = 0.3f; // Multiplier to keep speed within range

    void Start()
    {
        // Initialize the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Vector3Msg>(gimbalcontrolTopic);
        ros.RegisterPublisher<Vector3Msg>(gimbalresetTopic);
        ros.RegisterPublisher<TwistMsg>(cmdVelTopic);
    }

    void Update()
    {
        // Fetch joystick input from the left controller for gimbal control
        if (InputDevices.GetDeviceAtXRNode(leftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickPosition))
        {
            float gimbalY = 0;
            float gimbalZ = 0;

            // Apply deadzone filtering for forward/backward gimbal movement
            if (Mathf.Abs(leftJoystickPosition.y) > deadzone && Mathf.Abs(leftJoystickPosition.x) <= deadzone)
            {
                gimbalY = leftJoystickPosition.y * speedMultiplier;
            }
            // Apply deadzone filtering for right/left gimbal movement
            else if (Mathf.Abs(leftJoystickPosition.x) > deadzone && Mathf.Abs(leftJoystickPosition.y) <= deadzone)
            {
                gimbalZ = leftJoystickPosition.x* speedMultiplier;
            }

            // Create a Vector3 message for filtered gimbal control
            Vector3Msg gimbalControlMsg = new Vector3Msg
            {
                x = 0.0f,  // No up/down control in this setup
                y = -gimbalY,
                z = -gimbalZ
            };

            // Publish the gimbal control message
            ros.Publish(gimbalcontrolTopic, gimbalControlMsg);
        }

        // Check for the X button press on the left controller for gimbal reset
        if (InputDevices.GetDeviceAtXRNode(leftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed) && isXPressed)
        {
            // Create a Vector3 message with all zeros for the gimbal reset command
            Vector3Msg gimbalCmdMsg = new Vector3Msg
            {
                x = 0,
                y = 0,
                z = 0
            };

            // Publish the gimbal reset command
            ros.Publish(gimbalresetTopic, gimbalCmdMsg);
        }

        // Fetch joystick input from the right controller for the cmd_vel topic
        if (InputDevices.GetDeviceAtXRNode(rightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickPosition))
        {
            float linearX = 0;
            float linearY = 0;
            float linearZ = 0;

            // Check if joystick is pushed forward/backward significantly
            if (Mathf.Abs(rightJoystickPosition.y) > deadzone && Mathf.Abs(rightJoystickPosition.x) <= deadzone)
            {
                linearX = rightJoystickPosition.y * speedMultiplier; // Forward/backward
            }
            // Check if joystick is pushed right/left significantly
            else if (Mathf.Abs(rightJoystickPosition.x) > deadzone && Mathf.Abs(rightJoystickPosition.y) <= deadzone)
            {
                linearY = rightJoystickPosition.x * speedMultiplier; // Right/left
            }

            // Check for B button press on right controller
            if (InputDevices.GetDeviceAtXRNode(rightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed) && isBPressed)
            {
                linearZ = 1.0f * speedMultiplier; // Positive z-direction
            }
            // Check for A button press on right controller
            if (InputDevices.GetDeviceAtXRNode(rightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed) && isAPressed)
            {
                linearZ = -1.0f * speedMultiplier; // Negative z-direction
            }

            // Angular Z based on trigger and grip press
            float angularZ = 0;

            // Check for right trigger press
            if (InputDevices.GetDeviceAtXRNode(rightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed) && isTriggerPressed)
            {
                angularZ = 1.0f * speedMultiplier; // Positive angular z
            }
            // Check for right grip press
            if (InputDevices.GetDeviceAtXRNode(rightHand).TryGetFeatureValue(CommonUsages.gripButton, out bool isGripPressed) && isGripPressed)
            {
                angularZ = -1.0f * speedMultiplier; // Negative angular z
            }

            // Create a Twist message for movement based on filtered joystick input and button presses
            TwistMsg cmdVelMsg = new TwistMsg
            {
                linear = new Vector3Msg
                {
                    x = linearX,
                    y = linearY,
                    z = linearZ
                },
                angular = new Vector3Msg { x = 0, y = 0, z = angularZ }
            };

            // Publish the cmd_vel message
            ros.Publish(cmdVelTopic, cmdVelMsg);
        }
    }
}
