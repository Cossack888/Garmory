using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct CameraPointPair
{
    public CameraPoint point;
    public Transform cameraTransform;
}

public enum CameraPoint
{
    Game,
    Ring,
    Necklace,
    Boots,
    Helmet,
    Overview
}
public class CameraTransition : MonoBehaviour
{
    [SerializeField] private List<CameraPointPair> cameraPoints;
    private Camera cam;
    private Dictionary<CameraPoint, Transform> camPointsDict;
    public void ChangeCamera(CameraPoint cameraPoint)
    {
        if (camPointsDict.TryGetValue(cameraPoint, out Transform targetPoint))
        {
            StopAllCoroutines();
            StartCoroutine(SmoothCameraTransition(targetPoint));
        }
        else
        {

        }
    }
    public Dictionary<CameraPoint, Transform> GetCamPointsDict()
    {
        return camPointsDict;
    }
    private void Start()
    {
        cam = Camera.main;
        InitialiseCameraDictionary();
    }
    private IEnumerator SmoothCameraTransition(Transform targetPoint)
    {
        float duration = 1.5f;
        float elapsedTime = 0f;

        Vector3 startingPosition = cam.transform.position;
        Quaternion startingRotation = cam.transform.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            cam.transform.position = Vector3.Lerp(startingPosition, targetPoint.position, t);
            cam.transform.rotation = Quaternion.Lerp(startingRotation, targetPoint.rotation, t);

            yield return null;
        }


        cam.transform.position = targetPoint.position;
        cam.transform.rotation = targetPoint.rotation;
    }
    private void InitialiseCameraDictionary()
    {
        camPointsDict = new Dictionary<CameraPoint, Transform>();

        foreach (var pair in cameraPoints)
        {
            if (!camPointsDict.ContainsKey(pair.point))
            {
                camPointsDict.Add(pair.point, pair.cameraTransform);
            }
        }
    }

}
