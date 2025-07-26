using UnityEngine;
using UnityEngine.UI;

public class StylingPassthrough : MonoBehaviour
{
    [Header("Passthrough Configuration")]
    public OVRPassthroughLayer passthroughLayer;

    [Header("Slider References")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider contrastSlider;
    [SerializeField] private Slider saturationSlider;

    [Header("Slider Ranges")]
    [SerializeField] private float minValue = -1f;
    [SerializeField] private float maxValue = 1f;

    private static float currentBrightness = 0.0f;
    private static float currentContrast = 0.0f;
    private static float currentSaturation = 0.0f;

    private void Awake()
    {
        if (brightnessSlider == null || contrastSlider == null || saturationSlider == null)
        {
            FindSlidersInChildren();
        }

        ApplyCurrentSettings();
    }

    private void FindSlidersInChildren()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>();
        if (sliders.Length >= 3)
        {
            brightnessSlider = sliders[0];
            contrastSlider = sliders[1];
            saturationSlider = sliders[2];
        }
    }

    private void ApplyCurrentSettings()
    {
        if (passthroughLayer == null)
        {
            return;
        }

        passthroughLayer.colorMapEditorBrightness = currentBrightness;
        passthroughLayer.colorMapEditorContrast = currentContrast;
        passthroughLayer.colorMapEditorSaturation = currentSaturation;

        UpdateSliderValues();
    }

    public void OnBrightnessChanged(float value)
    {
        currentBrightness = Mathf.Clamp(value, minValue, maxValue);
        UpdatePassthroughSettings();
    }

    public void OnContrastChanged(float value)
    {
        currentContrast = Mathf.Clamp(value, minValue, maxValue);
        UpdatePassthroughSettings();
    }

    public void OnSaturationChanged(float value)
    {
        currentSaturation = Mathf.Clamp(value, minValue, maxValue);
        UpdatePassthroughSettings();
    }

    public void ResetSettings()
    {
        currentBrightness = 0.0f;
        currentContrast = 0.0f;
        currentSaturation = 0.0f;

        UpdatePassthroughSettings();
        UpdateSliderValues();
    }

    private void UpdatePassthroughSettings()
    {
        if (passthroughLayer == null) return;

        passthroughLayer.colorMapEditorBrightness = currentBrightness;
        passthroughLayer.colorMapEditorContrast = currentContrast;
        passthroughLayer.colorMapEditorSaturation = currentSaturation;
    }

    private void UpdateSliderValues()
    {
        if (brightnessSlider != null) brightnessSlider.value = currentBrightness;
        if (contrastSlider != null) contrastSlider.value = currentContrast;
        if (saturationSlider != null) saturationSlider.value = currentSaturation;
    }
}