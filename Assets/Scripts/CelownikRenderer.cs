using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]

public class CelownikRenderer : MonoBehaviour {

    public Ship ShipScript; 
    public int NumberOfSegments;
    public float TimeIntervalPerSegment;
    public float g;

    private int _noOfPoints { get { return NumberOfSegments + 1; } }
    private float _pointInterval;

    public LineRenderer LineToRenderTo;

    public float angle;

    // Use this for initialization
    void Start () {
        LineToRenderTo.positionCount = _noOfPoints;
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
        float z = 0f;
        if (_noOfPoints != LineToRenderTo.positionCount) return;
        angle = (ShipScript.angle + 90) * Mathf.Deg2Rad;
        for (int index = 0; index < _noOfPoints; index++)
        {
            float time = index * TimeIntervalPerSegment;
            LineToRenderTo.SetPosition(
                index, 
                new Vector3(
                    getX(time, angle), 
                    getY(time, angle), 
                    z));
        }
    }

    private float getY(float t, float angle)
    {
        float transformAngle = angle - 90 * Mathf.Deg2Rad;
        return Mathf.Cos(transformAngle) * getDown(t, angle) + Mathf.Sin(-transformAngle) * getForward(t, angle);
    }

    private float getX(float t, float angle)
    {
        float transformAngle = angle - 90 * Mathf.Deg2Rad;
        return Mathf.Sin(transformAngle) * getDown(t, angle) + Mathf.Cos(transformAngle) * getForward(t, angle);
    }

    private float getDown(float t, float angle)
    {
        float v0 = ShipScript.ShootPower;
        return -(g * t * t) / 2 +  Mathf.Sin(angle) * v0 * t;
    }

    private float getForward(float t, float angle)
    {
        float v0 = ShipScript.ShootPower;
        float mass = 10f;
        float wind = GameManager.Instance.WindForce / mass;
        return (wind * t * t) / 2 + Mathf.Cos(angle) * v0 * t;
    }

    public void Show()
    {
        LineToRenderTo.enabled = true;
    }

    public void Hide()
    {
        LineToRenderTo.enabled = false;
    }

    public void Toggle()
    {
        LineToRenderTo.enabled = !LineToRenderTo.enabled;
    }
}
