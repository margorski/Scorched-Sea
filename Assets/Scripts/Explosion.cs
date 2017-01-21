using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public GameObject ExplosionRayPrefab;
    public int HowManyRays = 4;
    public float MaxRange = 0.2f;
    public float HowLong = 0.2f;

    private readonly List<GameObject> ExplosionRays = new List<GameObject>();
    private readonly List<LineRenderer> ExplosionRayLines = new List<LineRenderer>();
    private readonly List<BoxCollider2D> ExplosionRayCollider = new List<BoxCollider2D>();
    private struct RayData
    {
        public float Length;
        public float Angle;
    }
    private readonly List<RayData> ExplosionRayDatas = new List<RayData>();
    private float startTime;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
        for (int i = 0; i < HowManyRays; i++)
        {
            ExplosionRays.Add(Instantiate(ExplosionRayPrefab, transform, false));
            ExplosionRayLines.Add(ExplosionRays[i].GetComponent<LineRenderer>());
            ExplosionRayLines[i].numPositions = 2;
            ExplosionRayCollider.Add(ExplosionRays[i].GetComponent<BoxCollider2D>());
            var length = Random.Range(0.01f, MaxRange);
            var angle = Random.Range(0f, 359.9f);
            ExplosionRayDatas.Add(new RayData() { Length = length, Angle = angle });
            ExplosionRayLines[i].transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.position = new Vector2(transform.position.x, Waver.Instance.GetY(transform.position.x));
        var timeElapsed = Time.time - startTime;
        bool destroy = false;
        if (timeElapsed > HowLong)
        {
            destroy = true;
        }
        for (int i = 0; i < HowManyRays; i++)
        {
            if (destroy)
            {
                Destroy(ExplosionRays[i]);
                continue;
            }
            var lengthScale = 1f - ((HowLong - timeElapsed) / HowLong);
           // ExplosionRays[i].GetComponent<
            var Rad = Mathf.Deg2Rad * ExplosionRayDatas[i].Angle;
            var rayLength = lengthScale * ExplosionRayDatas[i].Length;
            var endPoint = rayLength * new Vector2(Mathf.Sign(Rad), Mathf.Cos(Rad));
            ExplosionRayLines[i].SetPosition(1, new Vector2(rayLength, 0f));
            ExplosionRayCollider[i].size = new Vector2(rayLength, ExplosionRayLines[i].startWidth);
            ExplosionRayCollider[i].offset = new Vector2(rayLength / 2, 0f);
        }
        if(destroy) Destroy(gameObject);
	}
}
