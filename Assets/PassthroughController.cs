using UnityEngine;

public class PassthroughController : MonoBehaviour
{
    private void Start()
    {
        // Enable passthrough by default
        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
        }
    }
}
