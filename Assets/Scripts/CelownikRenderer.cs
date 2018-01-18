using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Celownik
{
    public static float getDown(float t, float angle, float g, float v0)
    {
        return -(g * t * t) / 2 +  Mathf.Sin(angle) * v0 * t;
    }

    public static float getForward(float t, float angle, float wind, float mass, float v0)
    {
        return (wind / mass * t * t) / 2 + Mathf.Cos(angle) * v0 * t;
    }
}

[RequireComponent(typeof(LineRenderer))]
public class CelownikRenderer : MonoBehaviour {

    public ShipShooter ShipScript; 
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
        Hide();
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
        float v0 = ShipScript.ShootPower;
        float transformAngle = angle - 90 * Mathf.Deg2Rad;
        return Mathf.Cos(transformAngle) * Celownik.getDown(t, angle, g, v0) +
               Mathf.Sin(-transformAngle) * Celownik.getForward(t, angle, GameManager.Instance.WindForce, 10f, v0);
    }

    private float getX(float t, float angle)
    {
        float v0 = ShipScript.ShootPower;
        float transformAngle = angle - 90 * Mathf.Deg2Rad;
        return Mathf.Sin(transformAngle) * Celownik.getDown(t, angle, g, v0) + 
               Mathf.Cos(transformAngle) * Celownik.getForward(t, angle, GameManager.Instance.WindForce, 10f, v0);
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
