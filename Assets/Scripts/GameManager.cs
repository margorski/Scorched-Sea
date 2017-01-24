using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using SystemRandom = System.Random;
using URandom = UnityEngine.Random;

public enum EnemyType
{
    Drone,
}

public interface IGameMode
{
    void StartMode();
    void EndMode();
    void OnUpdate();
    void OnFixedUpdate();
    void OnAllBulletsDestroyed();
    void OnShipFired();

    void EnemyDestroyed(EnemyType type);
    void PlayerDied(Ship player);

    Ship CurrentlyPlayingShip();
    List<Ship> GetAllPlayerShips();
}

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;

    public enum Spawnables
    {
        PlayerShip,
        DroneShipSpawner,
    }

    [Serializable]
    public struct SpawnableEntry
    {
        public Spawnables type;
        public GameObject prefab;
    }

    public List<SpawnableEntry> SpawnablePrefabs;
    public GameObject ShipPrefab;
    public float WindForce;
    public float MinWind;
    public float MaxWind;
    public SoundPlayer SoundPlayer;

    private AudioSource backgroundNoise;

    public Spawner DroneSpawner;

    public enum CameraMode
    {
         Normal,
         Stabilized
    };
    public CameraMode CamMode = CameraMode.Normal;

    public enum BoatPosition
    {
        Static,
        Drifting
    }
    public BoatPosition BoatBehavior = BoatPosition.Static;

    public enum PlayMode
    {
        Versus,
        WaveDefense
    };
    public PlayMode PMode = GameManager.PlayMode.Versus;

    private IGameMode _currentGameMode;
    private Dictionary<PlayMode, IGameMode> _gameModes = new Dictionary<PlayMode, IGameMode>()
    {
        { PlayMode.Versus, new VersusGameMode() },
        { PlayMode.WaveDefense, new WaveDefenseGameMode() }
    };

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public void DestroyRelay(GameObject toDestroy)
    {
        Destroy(toDestroy);
    }

    public GameObject InstantiateRelay(Spawnables spawnable)
    {
        var prefabEntry = SpawnablePrefabs.Single(x => x.type == spawnable);
        return Instantiate(prefabEntry.prefab);
    }

    public void AllBulletsDestroyed()
    {
        _currentGameMode.OnAllBulletsDestroyed();
    }

    public void ShipFired()
    {
        _currentGameMode.OnShipFired();
    }

    public void DroneEnemyDestroyed(EnemyType type)
    {
        _currentGameMode.EnemyDestroyed(type);
    }

    public void PlayerDied(Ship player)
    {
        _currentGameMode.PlayerDied(player);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        backgroundNoise = gameObject.GetComponent<AudioSource>();
        SoundPlayer = gameObject.GetComponentInChildren<SoundPlayer>();

    }

	// Use this for initialization
	void Start () {
        ShowInstructions();
        _currentGameMode = _gameModes[PlayMode.Versus];
        _currentGameMode.StartMode();
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CamMode = CamMode == CameraMode.Normal ? CameraMode.Stabilized : CameraMode.Normal;
            if (CamMode == CameraMode.Normal)
            {
                Camera.main.transform.eulerAngles = Vector3.zero;
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            BoatBehavior = BoatBehavior == BoatPosition.Static ? BoatPosition.Drifting : BoatPosition.Static;
            _currentGameMode.GetAllPlayerShips().ForEach(x => x.BoatSwims = (BoatBehavior == BoatPosition.Drifting));
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            PMode = PMode == PlayMode.Versus ? PlayMode.WaveDefense : PlayMode.Versus;
            SwitchGameMode(PMode);
        }

        _currentGameMode.OnUpdate();
    }

    public void SwitchGameMode(PlayMode newMode)
    {
        _currentGameMode.EndMode();
        _currentGameMode = _gameModes[newMode];
        Invoke("StartNewGameMode", 2.5f);
    }

    public void StartNewGameMode()
    {
        if (_currentGameMode == null) return;
        _currentGameMode.StartMode();
    }

    public void ShowWinMessage(string name, float time)
    {
        Hud.Instance.ShowWinMessage(name, time);
    }

    private void FixedUpdate()
    {
        SetSound();
        ChangeLevel();

        _currentGameMode.OnFixedUpdate();
    }

    internal void RandomizeWind()
    {
        WindForce = URandom.Range(MinWind, MaxWind);
    }

    public GameObject Instructions;

    public void ChangeLevel()
    {
        if (Input.GetKeyDown(KeyCode.Minus))  Waver.Instance.Init(Waver.Instance.CurrentLevel - 1);
        if (Input.GetKeyDown(KeyCode.Equals))   Waver.Instance.Init(Waver.Instance.CurrentLevel + 1);
        if (Input.GetKeyDown(KeyCode.Alpha1)) Waver.Instance.Init(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Waver.Instance.Init(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Waver.Instance.Init(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Waver.Instance.Init(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Waver.Instance.Init(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) Waver.Instance.Init(6);
        if (Input.GetKeyDown(KeyCode.F1))     ShowInstructions();
    }

    private void ShowInstructions()
    {
        if (Instructions != null)
        {
            Instructions.SetActive(true);
            Invoke("HideInstructions", 3f);
        }
    }

    private void HideInstructions()
    {
        if (Instructions != null) Instructions.SetActive(false);
    }

    private void SetSound()
    {
        var currentPlayer = _currentGameMode == null ? null : _currentGameMode.CurrentlyPlayingShip();
        if (currentPlayer == null)
        {
            backgroundNoise.pitch = 1.0f + Waver.Instance.GetY(0.0f) / 100.0f;
        }
        else
        {
            backgroundNoise.pitch = 1.0f + currentPlayer.gameObject.transform.position.y / 100.0f;
        }
    }
}
