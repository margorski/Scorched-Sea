using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class WaveLineRenderer : MonoBehaviour {

    public float WorldXStart;
    public float WorldXEnd;
    public int NumberOfSegments;

    private int _noOfPoints { get { return NumberOfSegments + 1; } }
    private float _pointInterval;

    public LineRenderer LineToRenderTo;

    // Use this for initialization
    void Start () {
        LineToRenderTo = gameObject.GetComponent<LineRenderer>();

        LineToRenderTo.numPositions = _noOfPoints;
        _pointInterval = Mathf.Abs(WorldXEnd - WorldXStart) / (float)_noOfPoints;
    }
	
	// Update is called once per frame
	void Update () {
		
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

        float angle = 0f;


        for (int index = 0; index < _noOfPoints; index++)
        {
            x = WorldXStart + _pointInterval * index;
            y = Waver.Instance.GetY(x);

            LineToRenderTo.SetPosition(index, new Vector3(x, y, z));
        }
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
