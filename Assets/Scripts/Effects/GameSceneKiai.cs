using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
# if UNITY_EDITOR
using UnityEditor;
# endif

public class GameSceneKiai : MonoBehaviour
{
    Camera mainCamera;
    VolumeProfile globalVolumeProfile;
    float normalCameraFov;
    [SerializeField] float kiaiCameraFov;
    float normalCameraZPos;
    [SerializeField] float kiaiCameraZPos;

    ChromaticAberration globalChromaticAberration;
    float normalChromaticAberration;
    [SerializeField] float kiaiChromaticAberration;
    ColorAdjustments globalColorAdjustments;
    float normalPostExposure;
    [SerializeField] float kiaiPostExposure;

    void Start()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif    
        
        Maestro.OnKiaiStart.AddListener(SetKiai);
        Maestro.OnKiaiEnd.AddListener(SetNormal);

        GetRefs();

        SetNormalInstant();
    }

    void GetRefs()
    {
        mainCamera = Camera.main;
        globalVolumeProfile = FindFirstObjectByType<Volume>()?.profile;
        normalCameraFov = mainCamera.fieldOfView;
        normalCameraZPos = mainCamera.transform.localPosition.z;
        normalChromaticAberration = 0;
        normalPostExposure = 0;
        globalVolumeProfile.TryGet(out globalChromaticAberration);
        globalVolumeProfile.TryGet(out globalColorAdjustments);
        normalChromaticAberration = globalChromaticAberration.intensity.value;
        normalPostExposure = globalColorAdjustments.postExposure.value;
    }

    void SetKiai()
    {
        StartCoroutine(SetKiaiCoroutine());
    }

    void SetNormal()
    {
        StartCoroutine(SetNormalCoroutine());
    }

    void SetNormalInstant()
    {
        if (mainCamera == null || globalVolumeProfile == null)
            Start();

        mainCamera.fieldOfView = normalCameraFov;
        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, normalCameraZPos);
        globalChromaticAberration.intensity.value = normalChromaticAberration;
        globalChromaticAberration.active = false;
        globalColorAdjustments.postExposure.value = normalPostExposure;
        globalColorAdjustments.active = false;
    }

    IEnumerator SetKiaiCoroutine()
    {
        globalChromaticAberration.active = true;
        globalColorAdjustments.active = true;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * .5f;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, kiaiCameraFov, t);
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, Mathf.Lerp(mainCamera.transform.localPosition.z, kiaiCameraZPos, t));
            globalChromaticAberration.intensity.value = Mathf.Lerp(globalChromaticAberration.intensity.value, kiaiChromaticAberration, t);
            globalColorAdjustments.postExposure.value = Mathf.Lerp(globalColorAdjustments.postExposure.value, kiaiPostExposure, t);
            yield return null;
        }
    }

    IEnumerator SetNormalCoroutine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * .5f;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalCameraFov, t);
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, Mathf.Lerp(mainCamera.transform.localPosition.z, normalCameraZPos, t));
            globalChromaticAberration.intensity.value = Mathf.Lerp(globalChromaticAberration.intensity.value, normalChromaticAberration, t);
            globalColorAdjustments.postExposure.value = Mathf.Lerp(globalColorAdjustments.postExposure.value, normalPostExposure, t);
            yield return null;
        }

        globalChromaticAberration.active = false;
        globalColorAdjustments.active = false;
    }

    void OnDestroy()
    {
        Maestro.OnKiaiStart.RemoveListener(SetKiai);
        Maestro.OnKiaiEnd.RemoveListener(SetNormal);
    }

#if UNITY_EDITOR
    void OnExitPlayMode(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode) return;
        SetNormalInstant();
    }
#endif
}
