using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]

public class WaveLineRenderer : MonoBehaviour {

    public float WorldXStart;
    public float WorldXEnd;
    public int NumberOfSegments;
    public bool IsCoreWave = false;
    public GameObject ChildPrefab;
    internal float Scale = 1f;
    internal float YOffset = 0f;

    private int _noOfPoints { get { return NumberOfSegments + 1; } }
    private float _pointInterval;

    public LineRenderer LineToRenderTo;

    // Use this for initialization
    void Start () {
        if (IsCoreWave && ChildPrefab != null)
        {
            var color = LineToRenderTo.startColor;
            var colorAlpha = 1f;
            var yOffset = YOffset;
            for (int i = 0; i < 8; i++)
            {
                var child = Instantiate(ChildPrefab, transform).GetComponent<WaveLineRenderer>();

                child.WorldXStart = WorldXStart;
                child.WorldXEnd = WorldXEnd;
                child.NumberOfSegments = NumberOfSegments;
                child.Scale = Scale * Mathf.Pow(0.85f, i);
                yOffset -= 1f * child.Scale;
                child.YOffset = yOffset;
                colorAlpha *= 0.6f;
                child.LineToRenderTo.startColor = new Color(color.r, color.g, color.b, colorAlpha);
                child.LineToRenderTo.endColor = new Color(color.r, color.g, color.b, colorAlpha);
            }
        }

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
        if (_noOfPoints != LineToRenderTo.numPositions) return;
        for (int index = 0; index < _noOfPoints; index++)
        {
            x = WorldXStart + _pointInterval * index;
            y = Waver.Instance.GetY(x) * Scale + YOffset;

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
