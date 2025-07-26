using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ToggleFilterControl : MonoBehaviour
{
    public GameObject canvasObject;
    public float distanceFromController = 2.5f;
    public float verticalOffset = -0.4f;

    private InputDevice leftController;
    private bool buttonPressed = false;

    void Start()
    {
        canvasObject.SetActive(false);
    }

    void Update()
    {
        if (!leftController.isValid)
        {
            GetLeftController();
            return;
        }

        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressed))
        {
            if (isPressed && !buttonPressed)
            {
                buttonPressed = true;
                ToggleCanvas();
            }
            else if (!isPressed && buttonPressed)
            {
                buttonPressed = false;
            }
        }
    }

    void GetLeftController()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        if (inputDevices.Count > 0) leftController = inputDevices[0];
    }

    void ToggleCanvas()
    {
        if (canvasObject == null) return;

        bool shouldShow = !canvasObject.activeSelf;
        canvasObject.SetActive(shouldShow);

        if (shouldShow)
        {
            if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 controllerPosition) &&
                leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion controllerRotation))
            {
                Vector3 forward = controllerRotation * Vector3.forward;
                Vector3 flatForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
                Vector3 targetPosition = controllerPosition + flatForward * distanceFromController + Vector3.up * verticalOffset;
                canvasObject.transform.position = targetPosition;

                canvasObject.transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
            }
        }
    }
}
