using UnityEngine;

public class EdgeColorSlider : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    [Header("Configuración del Slider Invisible")]
    public float sensitivity = 0.5f;
    private float t = 1f;

    private bool isTriggerHeld = false;
    private Vector3 lastControllerPos;

    void Start()
    {
        passthroughLayer.edgeColor = new Color(1f, 1f, 1f, 1f);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            isTriggerHeld = true;
            lastControllerPos = OVRInput.GetLocalControllerPosition(controller);
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            isTriggerHeld = false;
        }

        if (isTriggerHeld)
        {
            Vector3 currentPos = OVRInput.GetLocalControllerPosition(controller);
            float deltaX = currentPos.x - lastControllerPos.x;
            lastControllerPos = currentPos;

            t += deltaX * sensitivity * 5f;
            t = Mathf.Clamp01(t);
            Debug.Log("t: " + t);

            Color edgeColor = new Color(t, t, t, 1f);
            passthroughLayer.edgeColor = edgeColor;
        }
    }
}
