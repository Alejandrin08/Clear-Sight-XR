// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR;
using Meta.XR.Samples;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace PassthroughCameraSamples.MultiObjectDetection
{
    [MetaCodeSample("PassthroughCameraApiSamples-MultiObjectDetection")]
    public class EnvironmentRayCastSampleManager : MonoBehaviour
    {
        private const string SPATIALPERMISSION = "com.oculus.permission.USE_SCENE";

        public Transform Camera;
        public EnvironmentRaycastManager RaycastManager;

        private void Start()
        {
            if (!EnvironmentRaycastManager.IsSupported)
            {
                Debug.LogError("EnvironmentRaycastManager is not supported: please read the official documentation to get more details. (https://developers.meta.com/horizon/documentation/unity/unity-depthapi-overview/)");
            }
        }

        public bool HasScenePermission()
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(SPATIALPERMISSION);
#else
            return true;
#endif
        }

        public Transform PlaceGameObject(Vector3 cameraPosition)
        {
            if (EnvironmentRaycastManager.IsSupported)
            {
                transform.position = Camera.position;
                transform.LookAt(cameraPosition);

                var ray = new Ray(Camera.position, transform.forward);
                if (RaycastManager.Raycast(ray, out var hitInfo))
                {
                    transform.SetPositionAndRotation(
                        hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal, Vector3.up));
                }
                else
                {
                    Debug.Log("RaycastManager failed");
                }

                return transform;
            }
            else
            {
                Debug.LogError("EnvironmentRaycastManager is not supported");
                return null;
            }
        }
    }
}
