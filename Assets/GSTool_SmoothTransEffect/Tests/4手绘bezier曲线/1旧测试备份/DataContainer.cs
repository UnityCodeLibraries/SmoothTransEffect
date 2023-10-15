using UnityEngine;

[System.Serializable]
public class DataContainer : MonoBehaviour
{
    public string myData;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 controlPoint1;
    public Vector3 controlPoint2;

    public float journeyDuration = 3f;
    public float journeyTime;
    [Range(0, 1)]
    public float journeyNormalizedTime;
    [Range(-1, 1)]
    public float x;
}
