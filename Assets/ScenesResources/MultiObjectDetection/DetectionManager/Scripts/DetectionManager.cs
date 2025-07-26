using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Events;

namespace PassthroughCameraSamples.MultiObjectDetection
{
    public class DetectionManager : MonoBehaviour
    {
        [SerializeField] private WebCamTextureManager m_webCamTextureManager;

        [Header("Ui references")]
        [SerializeField] private DetectionUiMenuManager m_uiMenuManager;

        [Header("Placement configuration")]
        [SerializeField] private GameObject m_spwanMarker;
        [SerializeField] private EnvironmentRayCastSampleManager m_environmentRaycast;
        [SerializeField] private float m_spawnDistance = 0.25f;

        [Header("Sentis inference ref")]
        [SerializeField] private SentisInferenceRunManager m_runInference;
        [SerializeField] private SentisInferenceUiManager m_uiInference;

        public UnityEvent<int> OnObjectsIdentified;

        private bool m_isPaused = false; 
        private List<GameObject> m_spawnedEntities = new();
        private bool m_isSentisReady = false;
        private bool m_hasSpawnedObjects = false;

        private void Awake()
        {
            OVRManager.display.RecenteredPose += CleanMarkersCallBack;
            m_uiMenuManager.OnPause.AddListener(OnPause);
        }

        private IEnumerator Start()
        {
            var sentisInference = FindObjectOfType<SentisInferenceRunManager>();
            while (!sentisInference.IsModelLoaded)
            {
                yield return null;
            }
            m_isSentisReady = true;
        }

        private void Update()
        {
            var hasWebCamTextureData = m_webCamTextureManager.WebCamTexture != null;

            if (m_isPaused || !hasWebCamTextureData || !m_isSentisReady)
                return;

            if (!m_runInference.IsRunning())
            {
                m_runInference.RunInference(m_webCamTextureManager.WebCamTexture);
            }

            if (!m_hasSpawnedObjects && m_uiInference.BoxDrawn.Count > 0)
            {
                SpawnCurrentDetectedObjects();
                m_hasSpawnedObjects = true;
            }
        }

        private void CleanMarkersCallBack()
        {
            foreach (var e in m_spawnedEntities)
            {
                Destroy(e, 0.1f);
            }
            m_spawnedEntities.Clear();
            OnObjectsIdentified?.Invoke(-1);
            m_hasSpawnedObjects = false; 
        }

        private void SpawnCurrentDetectedObjects()
        {
            var count = 0;
            foreach (var box in m_uiInference.BoxDrawn)
            {
                if (PlaceMarkerUsingEnvironmentRaycast(box.WorldPos, box.ClassName))
                {
                    count++;
                }
            }
            if (count > 0)
            {
                OnObjectsIdentified?.Invoke(count);
            }
        }

        private bool PlaceMarkerUsingEnvironmentRaycast(Vector3 boxWorldPos, string className)
        {
            var markerTransform = m_environmentRaycast.PlaceGameObject(boxWorldPos);
            if (!markerTransform)
                return false;

            foreach (var e in m_spawnedEntities)
            {
                var markerClass = e.GetComponent<DetectionSpawnMarkerAnim>();
                if (markerClass)
                {
                    var dist = Vector3.Distance(e.transform.position, markerTransform.position);
                    if (dist < m_spawnDistance && markerClass.GetYoloClassName() == className)
                    {
                        return false;
                    }
                }
            }

            var newMarker = Instantiate(m_spwanMarker);
            m_spawnedEntities.Add(newMarker);
            newMarker.transform.SetPositionAndRotation(markerTransform.position, markerTransform.rotation);
            newMarker.GetComponent<DetectionSpawnMarkerAnim>().SetYoloClassName(className);

            return true;
        }

        public void OnPause(bool pause)
        {
            m_isPaused = pause;
        }
    }
}
