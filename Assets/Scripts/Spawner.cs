using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour {

    public float MinSpawnTime;
    public float MaxSpawnTime;
    public float WorldXSpawnPosition;
    public GameObject SpawnPrefab;
    public Transform PlayerLocation;

    private readonly List<GameObject> Spawns = new List<GameObject>();
    private bool _spawning = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (_spawning && _newSpawnTime < Time.time)
        {
            Spawn();
        }
	}

    private float _newSpawnTime;

    public void StartSpawning()
    {
        if (_spawning)
        {
            DestroySpawns();
        }
        _spawning = true;
        Spawn();
    }

    private void Spawn()
    {
        _newSpawnTime = Time.time + Random.Range(MinSpawnTime, MaxSpawnTime);
        var spawn = Instantiate(SpawnPrefab, new Vector3(WorldXSpawnPosition, Waver.Instance.GetY(WorldXSpawnPosition)), Quaternion.identity);
        spawn.GetComponent<DroneShip>().PlayerLocation = PlayerLocation;
        Spawns.Add(spawn);
    }

    public void DestroySpawns()
    {
        var allHitable = Spawns.Where(x => x != null && x.GetComponent<IHitable>() != null).ToList();
        Spawns.RemoveAll(x => allHitable.Contains(x));
        allHitable.ForEach(x => x.GetComponent<IHitable>().Die());
        Spawns.Clear();
    }

    public void StopSpawning()
    {
        _spawning = false;
    }
}
