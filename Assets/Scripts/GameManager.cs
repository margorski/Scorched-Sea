using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public enum TurnPhaseType
    {
        PlayerMove,
        BulletMove,
        EndOfTurn,
        EndOfRound
    }

    public List<Ship> Players = new List<Ship>();
    private static GameManager instance = null;

    public float TurnEndDelay = 0.5f;
    public float RoundEndDelay = 3.0f;
    public GameObject ShipPrefab;
    public float WindForce;
    public float MinWind;
    public float MaxWind;
    public int TurnCounter { private set; get; }
    public TurnPhaseType TurnPhase = TurnPhaseType.PlayerMove;
    public int SpawnMinX = 2;
    public int SpawnMaxX = 7;
    public SoundPlayer SoundPlayer;

    public int currentPlayer = - 1;
    private int startPlayer = -1;
    public int winPlayer = -1;
    private float timer;
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

    public struct Stats {
        public int kills;
        public int deaths;
        public string name;
    }
    public Stats[] playerStats = {new Stats(), new Stats()};

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
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
        playerStats[0].name = "Player 1";
        playerStats[1].name = "Player 2";

        Players.Add(Instantiate(ShipPrefab.GetComponent<Ship>()));
        Players.Add(Instantiate(ShipPrefab.GetComponent<Ship>()));

        backgroundNoise = gameObject.GetComponent<AudioSource>();
        SoundPlayer = gameObject.GetComponentInChildren<SoundPlayer>();

        foreach (Ship player in Players)
            player.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        InitRound();
	}

    private Ship _defensePlayer;
    public int DefenseScore;

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CamMode = CamMode == CameraMode.Normal ? CameraMode.Stabilized : CameraMode.Normal;
            if (CamMode == CameraMode.Normal)
            {
                Camera.main.transform.eulerAngles = Vector3.zero;
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            BoatBehavior = BoatBehavior == BoatPosition.Static ? BoatPosition.Drifting : BoatPosition.Static;
            Players.ForEach(x => x.BoatSwims = (BoatBehavior == BoatPosition.Drifting));
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PMode = PMode == PlayMode.Versus ? PlayMode.WaveDefense : PlayMode.Versus;
            if (PMode == PlayMode.WaveDefense)
            {
                EndVersusMode();
                StartWaveDefense();
            }
            if (PMode == PlayMode.Versus)
            {
                EndWaveDefense();
                InitPlayers();
                InitRound();
                Hud.Instance.SetPlayerTextEnabled(0, true);
                Hud.Instance.SetPlayerTextEnabled(1, true);
            }
        }
    }

    private Spawner DroneSpawnerInstance = null;

    public void EndVersusMode()
    {
        Players.ForEach(x => x.Die());
        Players.Clear();
    }

    public void StartWaveDefense()
    {
        if (DroneSpawnerInstance == null)
        {
            DroneSpawnerInstance = Instantiate(DroneSpawner).GetComponent<Spawner>();
        }
        DroneSpawnerInstance.StartSpawning();
        var xStart = -Random.Range(SpawnMinX, SpawnMaxX);
        var newShip = Instantiate(ShipPrefab);
        newShip.transform.position = new Vector3(xStart, Waver.Instance.GetY(xStart), 0f);
        _defensePlayer = newShip.GetComponent<Ship>();
        _defensePlayer.FocusCamera = true;
        DefenseScore = 0;
        Hud.Instance.SelectPlayer(0);
        Hud.Instance.SetPlayerTextEnabled(1,false);
    }

    public void EndWaveDefense()
    {
        DefenseScore = 0;
        Destroy(_defensePlayer.gameObject);
        DroneSpawnerInstance.DestroySpawns();
        Destroy(DroneSpawnerInstance);
    }

    private void FixedUpdate()
    {
        SetSound();
        ChangeLevel();

        if (PMode == PlayMode.WaveDefense && _defensePlayer == null)
        {
            StartWaveDefense();
            return;
        }
        if (TurnPhase == TurnPhaseType.EndOfTurn || TurnPhase == TurnPhaseType.EndOfRound)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0.0f)
            {
                NextPhase();
            }
        }
    }

    public void NextPhase()
    {
        if (PMode == PlayMode.WaveDefense) return;
        switch (TurnPhase)
        {
            case TurnPhaseType.PlayerMove:
                TurnPhase = TurnPhaseType.BulletMove;
                break;
            case TurnPhaseType.BulletMove:
                TurnPhase = TurnPhaseType.EndOfTurn;
                timer = TurnEndDelay;
                break;
            case TurnPhaseType.EndOfTurn:
                if (Players.FindAll(player => player.gameObject.GetComponent<Ship>()._isDead).Count == 1)
                {
                    //end of turn
                    winPlayer = Players.FindIndex(player => !player.gameObject.GetComponent<Ship>()._isDead);
                    var losePlayer = Players.FindIndex(player => player.gameObject.GetComponent<Ship>()._isDead);
                    playerStats[winPlayer].kills++;
                    playerStats[losePlayer].deaths++;

                    TurnPhase = TurnPhaseType.EndOfRound;
                    timer = RoundEndDelay;
                }
                else
                {
                    NextPlayer();
                    // all players played, next round
                    if (currentPlayer == startPlayer)
                    {
                        NextTurn();
                    }
                    // change round

                    TurnPhase = TurnPhaseType.PlayerMove;
                }
                break;
            case TurnPhaseType.EndOfRound:
                InitRound();
                break;
        }
    }
    
    private void InitRound()
    {
        Waver.Instance.Init(Random.Range(1, Waver.Instance.Levels.Count+1));
        RandomizeWind();
        // randomize wave
        InitPlayers();
        Hud.Instance.SelectPlayer(currentPlayer);
        foreach (var player in Players) player.FocusCamera = false;
        Players[currentPlayer].FocusCamera = true;
        TurnPhase = TurnPhaseType.PlayerMove;
    }

    private void InitPlayers()
    {
        if (Players.Count < 2)
        {
            Players.Add(null);
            Players.Add(null);
        }
        for(int i = 0; i < 2; i++)
        {
            if (Players[i] == null)
            {
                Players[i] = Instantiate(ShipPrefab.GetComponent<Ship>());
                Hud.Instance.SelectWeapon(i, Players[i].Weapon);
                //  Instantiate(ShipPrefab);
            }
        }
        var x1 = -Random.Range(SpawnMinX, SpawnMaxX);
        Players[0].gameObject.transform.position = new Vector3(x1, Waver.Instance.GetY(x1), 1.0f);
        var x2 = Random.Range(SpawnMinX, SpawnMaxX);
        Players[1].gameObject.transform.position = new Vector3(x2, Waver.Instance.GetY(x2), 1.0f);

        foreach (Ship player in Players)
        {
            player.gameObject.SetActive(true);
        }

        startPlayer = currentPlayer = Random.Range(0, Players.Count);
        UpdateCurrentDrawing();
    }


    private void UpdateCurrentDrawing()
    {
        for (var i = 0; i < Players.Count; i++)
        {
            if (Players[i] != null && !Players[i].Equals(null))
            {
                Players[i].SetCurrent(i == currentPlayer);
            }
        }
    }
    
    private void NextTurn()
    {
        RandomizeWind();
        TurnCounter++;
    }

    private void NextPlayer()
    {
        Players[currentPlayer].FocusCamera = false;
        currentPlayer++;
        if (currentPlayer >= Players.Count)
        {
            currentPlayer = 0;
        }
        UpdateCurrentDrawing();
        Hud.Instance.SelectPlayer(currentPlayer);
        Players[currentPlayer].FocusCamera = true;
    }

    public Ship GetCurrentPlayer()
    {
        switch (PMode)
        {
            case PlayMode.Versus:
                if (currentPlayer == -1)
                    return null;
                return Players[currentPlayer];
            case PlayMode.WaveDefense:
                return _defensePlayer;
        }
        return null;
    }

    private void RandomizeWind()
    {
        WindForce = Random.Range(MinWind, MaxWind);
    }

    public Ship GetWinPlayer()
    {
        if (winPlayer == -1)
            return null;
        return Players[winPlayer];
    }

    public void ChangeLevel()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Waver.Instance.Init(1);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Waver.Instance.Init(2);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            Waver.Instance.Init(3);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            Waver.Instance.Init(4);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            Waver.Instance.Init(5);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            Waver.Instance.Init(6);
        }
    }

    private void SetSound()
    {
        var currentPlayer = GameManager.Instance.GetCurrentPlayer();
        if (currentPlayer == null)
        {
            backgroundNoise.pitch = 1.0f + Waver.Instance.GetY(0.0f) / 100.0f;
        }
        else
        {
            backgroundNoise.pitch = 1.0f + Waver.Instance.GetY(currentPlayer.gameObject.transform.position.x) / 100.0f;
        }
    }
}
