using UnityEngine;
using Unity.XR.Oculus;

public class SceneChanger : MonoBehaviour
{
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            ChangeScene("Menu");
        }
    }

    void ChangeScene(string sceneName)
    {
        ClearCurrentScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    void ClearCurrentScene()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj != gameObject && obj.tag != "MainCamera")
            {
                Destroy(obj);
            }
        }

        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}
