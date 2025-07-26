using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderSelector : MonoBehaviour
{
    [Header("Configuración Básica")]
    [Range(1, 10)] public int numberOfRadialPart = 4;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenRadialPart = 10f;
    public Transform handTransform;

    [Header("Contenido del Menú")]
    public Material[] materialList;
    public Sprite[] radialIcons;

    [Header("Aplicación del Shader")]
    public Renderer targetRenderer;

    [Header("Ajustes Visuales")]
    public float menuRadius = 1.5f;
    public float iconRadius = 1.2f;
    public Color normalColor = new Color(1, 1, 1, 0.5f);
    public Color selectedColor = new Color(0.2f, 0.4f, 0.8f, 0.8f);

    private List<GameObject> spawnedParts = new List<GameObject>();
    private List<Image> segmentImages = new List<Image>();
    private List<Transform> iconTransforms = new List<Transform>();
    private int currentSelectedRadialPart = -1;
    private bool isMenuVisible = false;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            ToggleMenu();
        }

        if (isMenuVisible)
        {
            UpdateSelection();

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                ConfirmSelection();
            }
        }
    }

    void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        radialPartCanvas.gameObject.SetActive(isMenuVisible);

        if (isMenuVisible)
        {
            SetupMenuPosition();
            Canvas canvas = radialPartCanvas.GetComponent<Canvas>();
            if (canvas != null) canvas.sortingOrder = 100;
            CreateMenuItems();
        }
        else
        {
            ClearMenuItems();
        }
    }

    void SetupMenuPosition()
    {
        Vector3 menuPosition = handTransform.position + handTransform.forward * 0.5f;
        radialPartCanvas.position = menuPosition;
        radialPartCanvas.rotation = Quaternion.LookRotation(radialPartCanvas.position - Camera.main.transform.position);
    }

    void CreateMenuItems()
    {
        ClearMenuItems();

        float angleStep = 360f / numberOfRadialPart;
        float startAngle = -90f;
        float iconVerticalOffset = 0.2f;

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            float radians = currentAngle * Mathf.Deg2Rad;

            GameObject menuItem = Instantiate(radialPartPrefab, radialPartCanvas);
            menuItem.transform.localPosition = Vector3.zero;
            menuItem.transform.localRotation = Quaternion.identity;

            Image segmentImage = menuItem.GetComponent<Image>();
            if (segmentImage != null)
            {
                segmentImage.fillAmount = 1f / numberOfRadialPart - (angleBetweenRadialPart / 360f);
                segmentImage.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
                segmentImage.color = normalColor;
                segmentImages.Add(segmentImage);
            }

            Vector3 radialDirection = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0);

            Transform icon = menuItem.transform.Find("Icon");
            if (icon != null)
            {
                Vector3 iconOffset = new Vector3(-radialDirection.y, radialDirection.x, 0).normalized * iconVerticalOffset;
                Vector3 iconPosition = radialDirection * iconRadius + iconOffset;
                icon.localPosition = iconPosition;
                icon.localRotation = Quaternion.Euler(0, 0, -currentAngle);
                icon.localScale = 1.5f * Vector3.one;

                iconTransforms.Add(icon);

                if (i < radialIcons.Length && radialIcons[i] != null)
                {
                    icon.GetComponent<Image>().sprite = radialIcons[i];
                }
            }

            spawnedParts.Add(menuItem);
        }

        UpdateVisualFeedback();
    }

    void UpdateSelection()
    {
        if (radialPartCanvas == null || spawnedParts.Count == 0) return;

        Plane menuPlane = new Plane(radialPartCanvas.forward, radialPartCanvas.position);
        Ray laserRay = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch),
                            OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);

        float enter;
        if (menuPlane.Raycast(laserRay, out enter))
        {
            Vector3 hitPoint = laserRay.GetPoint(enter);
            Vector3 localHitPoint = radialPartCanvas.InverseTransformPoint(hitPoint);
            localHitPoint.z = 0;

            float angle = Mathf.Atan2(localHitPoint.y, localHitPoint.x) * Mathf.Rad2Deg + 90f;
            if (angle < 0) angle += 360f;

            int newSelection = Mathf.Clamp((int)(angle / (360f / numberOfRadialPart)), 0, numberOfRadialPart - 1);

            if (newSelection != currentSelectedRadialPart)
            {
                currentSelectedRadialPart = newSelection;
                UpdateVisualFeedback();
            }
        }
    }

    void UpdateVisualFeedback()
    {
        for (int i = 0; i < spawnedParts.Count; i++)
        {
            bool isSelected = (i == currentSelectedRadialPart);

            if (i < segmentImages.Count && segmentImages[i] != null)
            {
                segmentImages[i].color = isSelected ? selectedColor : normalColor;
            }

            if (i < iconTransforms.Count && iconTransforms[i] != null)
            {
                Image iconImage = iconTransforms[i].GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.enabled = isSelected;
                    iconTransforms[i].localScale = isSelected ? 1.7f * Vector3.one : 1.5f * Vector3.one;
                }
            }
        }
    }

    void ConfirmSelection()
    {
        if (currentSelectedRadialPart >= 0 &&
            currentSelectedRadialPart < materialList.Length &&
            materialList[currentSelectedRadialPart] != null)
        {
            Material selectedMaterial = new Material(materialList[currentSelectedRadialPart]); 
            Texture mainTex = targetRenderer.material.GetTexture("_MainTex");

            if (mainTex != null)
            {
                selectedMaterial.SetTexture("_MainTex", mainTex);
            }

            targetRenderer.material = selectedMaterial;
        }
    }

    void ClearMenuItems()
    {
        foreach (var item in spawnedParts)
        {
            if (item != null) Destroy(item);
        }
        spawnedParts.Clear();
        segmentImages.Clear();
        iconTransforms.Clear();
        currentSelectedRadialPart = -1;
    }
}
