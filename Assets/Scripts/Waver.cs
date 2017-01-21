using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Waver : MonoBehaviour {

    public float MainAmplitude = 3f;
    public float Pindol200 = 45f;    //for 1 -> 1 sec is 1 degree
    public float DegreesPhase = 0f;
    public float Frequency = 10f; //1 in position is 10 in angle

    private Wave _mainWave;
    private readonly List<Wave> _addWaves = new List<Wave>();
    private float _currentTime = 0f;
    private int _currentTurn = -1;

    public static Waver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Waver();
            }
            return _instance;
        }
    }

    private static Waver _instance;
    private Waver() { }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start() {
        Init();
    }

    public void Init()
    {
        _currentTurn = GameManager.Instance.TurnCounter;
        _mainWave = new Wave(MainAmplitude, Pindol200, DegreesPhase, Frequency);
        _addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 20f, -1, 0.5f, 6.2f));
        _addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, 5, 0.5f, 6.1f));
        _addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, 5, 0.5f, -3f));
    }

    /// <summary>
    /// Turns == -1 -> add wave that will last forever
    /// </summary>
    public void AddFunc(float worldXEpicenter, int turns, float damp = 0.5f)
    {
        _addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, turns, damp, worldXEpicenter));
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentTurn != GameManager.Instance.TurnCounter)
        {
            _currentTurn = GameManager.Instance.TurnCounter;
            onNewTurn();
        }
    }

    private void onNewTurn()
    {
        _addWaves.ForEach(x => x.onNewTurn());
        _addWaves.RemoveAll(x => x.IsActive == false);
    }

    void FixedUpdate()
    {
        _currentTime = Time.fixedTime;
    }

    public float GetY(float x)
    {
        return 
            _mainWave.GetY(x, _currentTime) +
            _addWaves.Sum(wave => wave.GetY(x, _currentTime));
    }
}

public class Wave
{
    // Main wave
    private float MainAmplitude = 3f;
    private float Pindol200 = 45f;    //for 1 -> 1 sec is 1 degree
    private float DegreesPhase = 0f;
    private float Frequency = 10f; //1 in position is 10 in angle
    private bool IsMain = false;

    // Additional waves
    private float DampingFactor = 1f;
    private int TurnsActive = 0;        //-1 special value - forever
    private float WorldXEpicenter = 0f;
    private int CurrentTurnActive = 1;

    public bool IsActive { get { return TurnsActive - CurrentTurnActive >= 0; } }

    public Wave(float amp, float pindol, float phase, float freq, int turnsActive = -1)
    {
        MainAmplitude = amp;
        Pindol200     = pindol;
        DegreesPhase  = phase;
        Frequency     = freq;
        TurnsActive   = turnsActive;
        IsMain = true;
    }

    public Wave(float amp, float pindol, float phase, float freq, int turnsActive, float damp, float epicenter)
    {
        MainAmplitude = amp;
        Pindol200 = pindol;
        DegreesPhase = phase;
        Frequency = freq;
        TurnsActive = turnsActive;
        DampingFactor = damp;
        WorldXEpicenter = epicenter;
        IsMain = false;
    }

    public float GetY(float x, float currentTime)
    {
        if (IsMain)
        {
            return Mathf.Sin(Mathf.Deg2Rad * ((x + DegreesPhase) * Frequency) + Mathf.Deg2Rad * (currentTime * Pindol200)) * MainAmplitude;
        }

        float howFar = Mathf.Abs(WorldXEpicenter - x);
        var y = Mathf.Pow(2.71828f, -howFar * DampingFactor) * Mathf.Cos(Mathf.Deg2Rad * ((x + DegreesPhase) * Frequency) + Mathf.Deg2Rad * (currentTime * Pindol200)) * MainAmplitude;
        if (TurnsActive != -1 && IsActive)
        {
            y *= (TurnsActive - CurrentTurnActive + 1) / (float)TurnsActive;
        }
        return y;
    }

    public void onNewTurn()
    {
        CurrentTurnActive++;
    }
}
