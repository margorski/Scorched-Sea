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
        //_addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 20f, 0f, -1, 0.5f, 4.2f));
        //_addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 10f, 1f, 5, 0.5f, 4.1f));
        //_addWaves.Add(new Wave(MainAmplitude * 0.2f, Pindol200, DegreesPhase, Frequency * 20f, 1000000f, -1, 0.5f, -4.2f));
    }

    public void Init(int level)
    {
        LoadLevel(level - 1);
    }

    private List<List<Wave>> Levels = new List<List<Wave>>()
    {
        new List<Wave>(){new Wave(1, 45, 0, 20, 0) },
        new List<Wave>(){new Wave(1, 180, 0, 25, 5000f) },
        new List<Wave>(){new Wave(2, 45, 0, 5, 50f), new Wave(1.2f, 80f, 0f, 10f, 0f, -1, 2f, 3f), new Wave(1.2f, 80f, 0f, 10f, 0f, -1, 2f, -3f) },
        new List<Wave>(){new Wave(2, 45, 0, 5, 0f), new Wave(1.8f, 120f, 0f, 10f, 0f, -1, 0.1f, 3f), new Wave(0.4f, 20f, 0f, 100f, 0f, -1, 0.3f, -0.8f), new Wave(0.5f, 40f, 20f, 120f, 8f, -1, 0.3f, -4.0f)},
    };

    private void LoadLevel(int levelIndex)
    {
        _addWaves.Clear();
        _mainWave = Levels[levelIndex][0];
        foreach (var wave in Levels[levelIndex].Skip(1))
        {
            _addWaves.Add(wave);
        }
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

    public float GetY(float x, out float angle)
    {
        angle = 0f;
        if (_mainWave == null) return 0f;
        var delta = 0.25f;
        var h = GetY(x + delta) - GetY(x - delta);
        angle = Mathf.Atan(h / (2f * delta)) * Mathf.Rad2Deg;
        return GetY(x);
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
            var standingMain = StandingWaveCoeff < Mathf.Epsilon ? 1f : Mathf.Cos(kaIks / StandingWaveCoeff + omegaT);
            return Mathf.Sin(kaIks + omegaT) * standingMain * MainAmplitude;
        }

        var standing = StandingWaveCoeff < Mathf.Epsilon ? 1f : -Mathf.Sin(kaIks / StandingWaveCoeff + omegaT);
        float howFar = Mathf.Abs(WorldXEpicenter - x);
        var y = Mathf.Pow(2.71828f, -howFar * DampingFactor) * Mathf.Cos(kaIks + omegaT) * standing * MainAmplitude;
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
