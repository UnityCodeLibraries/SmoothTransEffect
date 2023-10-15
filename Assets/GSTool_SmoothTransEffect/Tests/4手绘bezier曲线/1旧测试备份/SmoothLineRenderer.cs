using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int numPoints = 10;
    public float curveHeight = 2.0f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numPoints;

        // Calculate and set the positions for the smooth curve
        Vector3[] positions = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            float x = t * 10;  // Adjust this value to control the length of the curve
            float y = Mathf.Sin(x) * curveHeight;
            positions[i] = new Vector3(x, y, 0);
        }

        lineRenderer.SetPositions(positions);
    }
}
