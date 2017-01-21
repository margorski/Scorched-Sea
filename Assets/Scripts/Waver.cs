using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Waver : MonoBehaviour {

    public float MainAmplitude = 3f;
    public float Pindol200 = 45f;    //for 1 -> 1 sec is 1 degree
    public float DegreesPhase = 0f;
    public float Frequency = 10f; //1 in position is 10 in angle
    public float StandingWaveCoeff = 10f;

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
        _mainWave = new Wave(MainAmplitude, Pindol200, DegreesPhase, Frequency, StandingWaveCoeff);
        // to play with stuff
        //_addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 20f, 1f, -1, 0.5f, 6.2f));
        //_addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, 1f, 5, 0.5f, 6.1f));
        //_addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, 1f, 5, 0.5f, -3f));
    }

    /// <summary>
    /// Turns == -1 -> add wave that will last forever
    /// </summary>
    public void AddFunc(float worldXEpicenter, int turns, float damp = 0.5f)
    {
        _addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, StandingWaveCoeff, turns, damp, worldXEpicenter));
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
        if (_mainWave == null) return 0f;
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
    private float StandingWaveCoeff = 10f;
    private bool IsMain = false;

    // Additional waves
    private float DampingFactor = 1f;
    private int TurnsActive = 0;        //-1 special value - forever
    private float WorldXEpicenter = 0f;
    private int CurrentTurnActive = 1;

    public bool IsActive { get { return TurnsActive - CurrentTurnActive >= 0; } }

    public Wave(float amp, float pindol, float phase, float freq, float stdWaveCoeff, int turnsActive = -1)
    {
        MainAmplitude = amp;
        Pindol200     = pindol;
        DegreesPhase  = phase;
        Frequency     = freq;
        TurnsActive   = turnsActive;
        StandingWaveCoeff = stdWaveCoeff;
        IsMain = true;
    }

    public Wave(float amp, float pindol, float phase, float freq, float stdWaveCoeff, int turnsActive, float damp, float epicenter)
    {
        MainAmplitude = amp;
        Pindol200 = pindol;
        DegreesPhase = phase;
        Frequency = freq;
        TurnsActive = turnsActive;
        StandingWaveCoeff = stdWaveCoeff;
        DampingFactor = damp;
        WorldXEpicenter = epicenter;
        IsMain = false;
    }

    public float GetY(float x, float currentTime)
    {
        var omegaT = Mathf.Deg2Rad * (currentTime * Pindol200);
        var kaIks = Mathf.Deg2Rad * ((x + DegreesPhase) * Frequency);
        if (IsMain)
        {
            var standing = StandingWaveCoeff < Mathf.Epsilon ? 1f : Mathf.Cos(kaIks / StandingWaveCoeff + omegaT);
            return Mathf.Sin(kaIks + omegaT) * standing * MainAmplitude;
        }

        float howFar = Mathf.Abs(WorldXEpicenter - x);
        var y = Mathf.Pow(2.71828f, -howFar * DampingFactor) * Mathf.Cos(kaIks + omegaT) * MainAmplitude;
        if (TurnsActive != -1 && IsActive)
        {
            y *= (TurnsActive - CurrentTurnActive + 1) / (float)TurnsActive;
        }
        return y;
    }

    public float GetY(float x, float currentTime, ref float angle)
    {
        return 0f;
    }

    public void onNewTurn()
    {
        CurrentTurnActive++;
    }
}
