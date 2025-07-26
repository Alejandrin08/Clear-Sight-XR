using System.Collections;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PassthroughCameraSamples.MultiObjectDetection
{
    public class DetectionUiMenuManager : MonoBehaviour
    {
        [Header("Ui elements ref.")]
        [SerializeField] private GameObject m_noPermissionPanel;
        [SerializeField] private Text m_labelInfromation;

        public UnityEvent<bool> OnPause;

        private int m_objectsDetected = 0;
        private int m_objectsIdentified = 0;

        public bool IsPaused { get; private set; } = true;

        private IEnumerator Start()
        {
            m_noPermissionPanel.SetActive(false);

            var sentisInference = FindObjectOfType<SentisInferenceRunManager>();
            while (!sentisInference.IsModelLoaded)
            {
                yield return null;
            }

            while (!PassthroughCameraPermissions.HasCameraPermission.HasValue)
            {
                yield return null;
            }

            if (PassthroughCameraPermissions.HasCameraPermission == false)
            {
                m_noPermissionPanel.SetActive(true);
                IsPaused = true;
                yield break;
            }

            IsPaused = false;
            OnPause?.Invoke(false);
        }

        private void UpdateLabelInformation()
        {
            m_labelInfromation.text = $"Detectando objetos: {m_objectsDetected}\nObjectos identificados: {m_objectsIdentified}";
        }

        public void OnObjectsDetected(int objects)
        {
            m_objectsDetected = objects;
            UpdateLabelInformation();
        }

        public void OnObjectsIndentified(int objects)
        {
            if (objects < 0)
            {
                m_objectsIdentified = 0;
            }
            else
            {
                m_objectsIdentified += objects;
            }
            UpdateLabelInformation();
        }
    }
}
