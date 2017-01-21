using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private enum TurnPhaseType
    {
        PlayerMove,
        BulletMove,
        EndOfTurn
    }

    private List<bool> Players = new List<bool>();
    private static GameManager instance = null;

    public float TurnEndDelay = 2.0f;
    public GameObject ShipPrefab;
    public float WindForce { private set; get; }
    public float MinWind;
    public float MaxWind;

    private TurnPhaseType turnPhase = TurnPhaseType.PlayerMove;
    private int currentPlayer;
    private int turnCounter;
    private int startPlayer = -1;
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
        if (turnPhase == TurnPhaseType.EndOfTurn)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                NextPhase();
            }
        }
    }

    public void NextPhase()
    {
        switch (turnPhase)
        {
            case TurnPhaseType.PlayerMove:
                turnPhase = TurnPhaseType.BulletMove;
                break;
            case TurnPhaseType.BulletMove:
                turnPhase = TurnPhaseType.EndOfTurn;
                timer = TurnEndDelay;
                break;
            case TurnPhaseType.EndOfTurn:
                if (Players.FindAll(player => player).Count == 1)
                {
                    //end of turn
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
                }
                turnPhase = TurnPhaseType.BulletMove;
                break;
        }
    }
    
    private void InitRound()
    {
        RandomizeWind();
        // randomize wave
        // generate players;
    }

    private void NextRound()
    {
        Players.Find(player => player) {
            // add win
        }
        InitRound();
    }

    private void NextTurn()
    {
        turnCounter++;
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
}
