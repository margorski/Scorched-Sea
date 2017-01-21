using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class BoatRenderer : MonoBehaviour
{

    private int NumberOfSegments;
    public List<Vector2> Points;

    private int _noOfPoints { get { return NumberOfSegments + 1; } }
    public LineRenderer LineToRenderTo;

    // Use this for initialization
    void Start()
    {
        LineToRenderTo = gameObject.GetComponent<LineRenderer>();

        LineToRenderTo.numPositions = Points.Count + 1;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        CreatePoints();
    }

    public void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        int index = 0;
        foreach (var item in Points)
        {
            x = item.x;
            y = item.y + Waver.Instance.GetY(x);

            LineToRenderTo.SetPosition(index++, new Vector3(x, y, z));
        }
        LineToRenderTo.SetPosition(index, new Vector3(Points[0].x, Points[0].y + Waver.Instance.GetY(Points[0].x), z));
    }

    public void Show()
    {
        LineToRenderTo.enabled = true;
    }

    public void Hide()
    {
        LineToRenderTo.enabled = false;
    }
}
