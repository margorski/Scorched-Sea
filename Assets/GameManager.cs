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

    public List<bool> Players = new List<bool>();
    private static GameManager instance = null;

    public float TurnEndDelay = 2.0f;
    public GameObject ShipPrefab;
    public float WindForce;
    public float MinWind;
    public float MaxWind;
    public int TurnCounter { private set; get; }
    public TurnPhaseType TurnPhase = TurnPhaseType.PlayerMove;

    private int currentPlayer;
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
    }

	// Use this for initialization
	void Start () {
		
	}
	


	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (TurnPhase == TurnPhaseType.EndOfTurn)
        {
            timer -= Time.deltaTime;
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
                if (Players.FindAll(player => player).Count == 1)
                {
                    //end of turn
                    winPlayer = Players.FindIndex(player => player);
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

                    TurnPhase = TurnPhaseType.BulletMove;
                }
                break;
            case TurnPhaseType.EndOfRound:
                InitRound();
                TurnPhase = TurnPhaseType.BulletMove;
                break;
        }
    }
    
    private void InitRound()
    {
        RandomizeWind();
        // randomize wave
        // generate players;
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

    private void RandomizeWind()
    {
        WindForce = Random.Range(MinWind, MaxWind);
    }

    public bool GetWinPlayer()
    {
        if (winPlayer == -1)
            return false;
        return Players[winPlayer];
    }
}
