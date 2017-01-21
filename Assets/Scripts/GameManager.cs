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

    public float TurnEndDelay = 2.0f;
    public GameObject ShipPrefab;
    public float WindForce;
    public float MinWind;
    public float MaxWind;
    public int TurnCounter { private set; get; }
    public TurnPhaseType TurnPhase = TurnPhaseType.PlayerMove;
    public int SpawnMinX = 2;
    public int SpawnMaxX = 7;

    private int currentPlayer = - 1;
    private int startPlayer = -1;
    private int winPlayer = -1;
    private float timer;
    
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

        Players = new List<Ship>();
        Players.Add(Instantiate(ShipPrefab.GetComponent<Ship>()));
        Players.Add(Instantiate(ShipPrefab.GetComponent<Ship>()));
        Players[0].playerName = "Player 1";
        Players[1].playerName = "Player 2";

        foreach (Ship player in Players)
            player.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        InitRound();
	}
	


	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (TurnPhase == TurnPhaseType.EndOfTurn)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0.0f)
            {
                NextPhase();
            }
        }
        else if (TurnPhase == TurnPhaseType.EndOfRound)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                NextPhase();
            }
        }
    }

    public void NextPhase()
    {
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
                if (Players.FindAll(player => player.gameObject.activeSelf).Count == 1)
                {
                    //end of turn
                    winPlayer = Players.FindIndex(player => player.gameObject.activeSelf);
                    TurnPhase = TurnPhaseType.EndOfRound;
                    Debug.Log("GameManager: End Of Turn");
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
        RandomizeWind();
        // randomize wave
        InitPlayers();
        TurnPhase = TurnPhaseType.PlayerMove;
    }

    private void InitPlayers()
    {
        var x1 = -Random.Range(-SpawnMinX, -SpawnMaxX);
        Players[0].gameObject.transform.position = new Vector3(x1, Waver.Instance.GetY(x1), 1.0f);
        var x2 = -Random.Range(SpawnMinX, SpawnMaxX);
        Players[1].gameObject.transform.position = new Vector3(x2, Waver.Instance.GetY(x2), 1.0f);
        foreach(Ship player in Players)
        {
            player.gameObject.SetActive(true);
        }
        startPlayer = currentPlayer = Random.Range(0, 1);
    }

    private void NextTurn()
    {
        TurnCounter++;
        RandomizeWind();
    }

    private void NextPlayer()
    {
        currentPlayer++;
        if (currentPlayer >= Players.Count)
        {
            currentPlayer = 0;
        }
    }

    public Ship GetCurrentPlayer()
    {
        return Players[currentPlayer];
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
}
