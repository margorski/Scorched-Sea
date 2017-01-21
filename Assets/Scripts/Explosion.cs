using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public GameObject ExplosionRayPrefab;
    public int HowManyRays = 4;
    public float MaxRange = 0.2f;
    public float HowLong = 0.2f;

    private List<GameObject> ExplosionRays;
    private List<LineRenderer> ExplosionRayLines;
    private List<Collider2D> ExplosionRayCollider;
    private struct RayData
    {
        public Vector2 end;
    }
    private List<RayData> ExplosionRayDatas;
    private float startTime;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
        for (int i = 0; i < HowManyRays; i++)
        {
            ExplosionRays.Add(Instantiate(ExplosionRayPrefab, transform));
            ExplosionRayLines.Add(ExplosionRays[i].GetComponent<LineRenderer>());
            ExplosionRayCollider.Add(ExplosionRays[i].GetComponent<Collider2D>());
            ExplosionRayDatas.Add(new RayData() { end = new Vector2(Random.Range(0.01f, 0.5f), Random.Range(0.01f, 0.5f)) });
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
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
           // ExplosionRays[i].GetComponent<
        }
	}
}
