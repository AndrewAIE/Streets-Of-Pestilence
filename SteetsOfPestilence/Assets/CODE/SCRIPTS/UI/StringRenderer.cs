using UnityEngine;

public class StringRenderer : MonoBehaviour
{
    public Transform[] points; // Array of transforms for the string points
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.positionCount = points.Length; // Match the number of points
    }

    void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
