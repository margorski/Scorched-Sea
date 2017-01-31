using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]

public class WaveLineRenderer : MonoBehaviour, IRecordable {

    public float WorldXStart;
    public float WorldXEnd;
    public int NumberOfSegments;
    public bool IsCoreWave = false;
    public bool _isRecording = false;
    public GameObject ChildPrefab;
    internal float Scale = 1f;
    internal float YOffset = 0f;
    internal float XOffset = 0f;

    private int _noOfPoints { get { return NumberOfSegments + 1; } }
    private float _pointInterval;

    public LineRenderer LineToRenderTo;

    public List<Vector3> _singleFrameWavePosition;
    public List<List<Vector3>> _allWavePositions;
    // Use this for initialization
    void Start () {
        _allWavePositions = new List<List<Vector3>>();
        if (IsCoreWave && ChildPrefab != null)
        {
            var color = LineToRenderTo.startColor;
            var colorAlpha = 1f;
            var yOffset = YOffset;
            var xOffset = XOffset;
            for (int i = 0; i < 6; i++)
            {
                var child = Instantiate(ChildPrefab, transform).GetComponent<WaveLineRenderer>();

                child.WorldXStart = WorldXStart;
                child.WorldXEnd = WorldXEnd;
                child.NumberOfSegments = NumberOfSegments;
                child.Scale = Scale * Mathf.Pow(0.85f, i);
                yOffset -= 1f * child.Scale;
                child.YOffset = yOffset;
                xOffset -= 0.2f * child.Scale;
                child.XOffset = xOffset;
                colorAlpha *= 0.6f;
                child.LineToRenderTo.startColor = new Color(color.r, color.g, color.b, colorAlpha);
                child.LineToRenderTo.endColor = new Color(color.r, color.g, color.b, colorAlpha);
            }

            colorAlpha = 1f;
            yOffset = YOffset;
            xOffset = XOffset;
            for (int i = 0; i < 3; i++)
            {
                var child = Instantiate(ChildPrefab, transform).GetComponent<WaveLineRenderer>();

                child.WorldXStart = WorldXStart;
                child.WorldXEnd = WorldXEnd;
                child.NumberOfSegments = NumberOfSegments;
                child.Scale = Scale * Mathf.Pow(0.7f, i);
                yOffset += 1f * child.Scale;
                child.YOffset = yOffset;
                xOffset += 0.15f * child.Scale;
                child.XOffset = xOffset;
                colorAlpha *= 0.5f;
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
      //  if(!Replays.playback)
        CreatePoints();
    }

    public void CreatePoints()
    {
        _singleFrameWavePosition = new List<Vector3>();
        float x;
        float y;
        float z = 0f;
        if (_noOfPoints != LineToRenderTo.numPositions) return;
        for (int index = 0; index < _noOfPoints; index++)
        {
            x = WorldXStart + _pointInterval * index;
            y = Waver.Instance.GetY(x) * Scale + YOffset;
            x += XOffset;

            LineToRenderTo.SetPosition(index, new Vector3(x, y, z));
            _singleFrameWavePosition.Add(new Vector3(x, y, z));
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

    public IEnumerator Playback()
    {

        foreach (List<Vector3> outerList in _allWavePositions.ToList())
        {
            for (int i = 0; i < outerList.Count - 1; i++)
            {
                LineToRenderTo.SetPosition(i, outerList[i]);
            }
            yield return null;
        }

        // Would be nice to release this humongous amount of datas
        _allWavePositions.Clear();
        _singleFrameWavePosition.Clear();
        Replays.playback = false;
    }

    public void Record()
    {
        _allWavePositions.Add(_singleFrameWavePosition);
    }
    
}
