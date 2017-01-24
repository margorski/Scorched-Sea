using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Waver : MonoBehaviour
{

    public float MainAmplitude = 3f;
    public float Pindol200 = 45f;    //for 1 -> 1 sec is 1 degree
    public float DegreesPhase = 0f;
    public float Frequency = 10f; //1 in position is 10 in angle
    public float StandingWaveCoeff = 10f;
    public float YOffset;
    public float TransitionTime = 0.5f;

    private Wave _mainWave;
    private readonly List<Wave> _addWaves = new List<Wave>();
    private float _currentTime = 0f;

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
    void Start()
    {
    }

    public int CurrentLevel = 1;
    public void Init(int level)
    {
        if (level == -1)
        {
            LoadLevel(-1);
            return;
        }
        if (level > Levels.Count) level = 1;
        if (level < 1) level = Levels.Count;
        CurrentLevel = level;
        LoadLevel(level - 1);
    }

    private Wave Level0 = new Wave(0, 0, 0, 0, 0);

    public List<List<Wave>> Levels = new List<List<Wave>>()
    {
        new List<Wave>(){new Wave(1, 45, 0, 20, 0) },
        new List<Wave>(){new Wave(1.2f, 60f, 0, 25, 5000f) },
        new List<Wave>(){new Wave(2, 45, 0, 5, 50f), new Wave(1.2f, 80f, 0f, 10f, 0f, -1, 2f, 3f), new Wave(1.2f, 80f, 0f, 10f, 0f, -1, 2f, -3f) },
        new List<Wave>(){new Wave(2, 45, 0, 5, 0f), new Wave(1.8f, 120f, 0f, 10f, 0f, -1, 0.1f, 3f), new Wave(0.4f, 20f, 0f, 100f, 0f, -1, 0.3f, -0.8f), new Wave(0.5f, 40f, 20f, 120f, 8f, -1, 0.3f, -4.0f)},
        new List<Wave>()
        {   new Wave(1f, 45f, 0f, 20f, 0f), //main
            new Wave(0.8f, 120f, 5f , 10f, 0f, 10, 0.1f, -5f),
            new Wave(0.8f, 120f, 10f, 10f, 0f, 10, 0.1f, -4f),
            new Wave(0.8f, 120f, 15f, 10f, 0f, 10, 0.1f, -3f),
            new Wave(0.8f, 120f, 20f, 10f, 0f, 10, 0.1f, -2f),
            new Wave(0.8f, 120f, 25f, 10f, 0f, 10, 0.1f, -1f),
            new Wave(0.8f, 120f, 30f, 10f, 0f, 10, 0.1f, 0f),
            new Wave(0.8f, 120f, 35f, 10f, 0f, 10, 0.1f, 1f),
            new Wave(0.8f, 120f, 40f, 10f, 0f, 10, 0.1f, 2f),
            new Wave(0.8f, 120f, 45f, 10f, 0f, 10, 0.1f, 3f),
            new Wave(0.8f, 120f, 50f, 10f, 0f, 10, 0.1f, 4f),
            new Wave(0.8f, 120f, 55f, 10f, 0f, 10, 0.1f, 5f),
        },
        new List<Wave>()
        {   new Wave(0.5f, 45f, 0f, 10f, 0f), //main
            new Wave(1.8f, 60f * 4f, -60f, 15f, 0f, -1, 1f, -1f),
            new Wave(2.0f, 50f * 4f, 0f, 35f, 0f, -1, 1f, -0.5f),
            new Wave(2.8f, 40f * 4f, 0f, 45f, 0f, -1, 1f, 0f),
            new Wave(2.0f, 50f * 4f, 0f, 35f, 0f, -1, 1f, 0.5f),
            new Wave(1.8f, 60f * 4f, 60f, 15f, 0f, -1, 1f, 1f),
        },
    };

    private void LoadLevel(int levelIndex)
    {
        _addWaves.Clear();
        if (levelIndex == -1)
        {
            _mainWave = Level0;
            return;
        }
        if (Levels.Count <= levelIndex) return;
        _mainWave = Levels[levelIndex][0];
        foreach (var wave in Levels[levelIndex].Skip(1))
        {
            AddWave(wave);
        }
    }

    private void AddWave(Wave wave)
    {
        _addWaves.Add(wave);
        wave.TransitionTime = TransitionTime;
    }

    /// <summary>
    /// Turns == -1 -> add wave that will last forever
    /// </summary>
    public void AddFunc(float worldXEpicenter, int turns, float damp = 5f)
    {
        AddWave(new Wave(MainAmplitude * 3.5f, Pindol200 * 1.5f, DegreesPhase + 30f, Frequency * 8f, 0.2f, turns, damp, worldXEpicenter));
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnNewTurn()
    {
        _addWaves.ForEach(x => x.onNewTurn());
        _addWaves.Where(x => x.IsActive == false).ToList().ForEach(x => x.DieOut());
    }

    void FixedUpdate()
    {
        _currentTime = Time.fixedTime;
        _addWaves.RemoveAll(x => x.State == Wave.WaveState.Dead);
        if (Mathf.Abs(_currentTime - LastTimeTimestamp) > TimeResolution)
        {
            Recalc(_currentTime);
        }
    }

    public float GetY(float x)
    {
        if (_mainWave == null) return 0f;
        var index = Mathf.Clamp(Mathf.RoundToInt((x - (float)left) * (float)resolutionFactor), 0, resolution - 1);
        return LastTimeUpdate[index];
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


    private readonly float[] LastTimeUpdate = new float[resolution];
    private float LastTimeTimestamp = 0f;
    private const int resolution = (right - left) * resolutionFactor + 1;
    private const int resolutionFactor = 15;
    private const int left = -10;
    private const int right = 10;
    private const float TimeResolution = 0.03f;

    private void Recalc(float currentTime)
    {
        for (int index = 0; index < resolution; index++)
        {
            float x = (float)index / (float)resolutionFactor + (float)left;
            LastTimeTimestamp = currentTime;
            LastTimeUpdate[index] =
                _mainWave.GetY(x, _currentTime) +
                _addWaves.Sum(wave => wave.GetY(x, _currentTime)) +
                YOffset;
        }
    }
}

public class Wave
{
    public float TransitionTime;
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
    private float _transitionTimestamp;

    public enum WaveState
    {
        Awake,
        Damping,
        Dying,
        Dead
    };

    public WaveState State = WaveState.Awake;

    public bool IsActive { get { return TurnsActive == -1 || (TurnsActive - CurrentTurnActive >= 0); } }

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
        if (State == WaveState.Dead) return 0f;
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
        var turnDelta = 0f;
        if (State == WaveState.Dying || State == WaveState.Damping)
        {
            var timeDelta = Time.time - _transitionTimestamp;
            var timeDeltaScale = 1f - (timeDelta) / TransitionTime;
            if (State == WaveState.Dying && timeDelta > TransitionTime)
            {
                State = WaveState.Dead;
                return 0f;
            }
            if (State == WaveState.Dying)
            {
                y *= timeDeltaScale / (float)TurnsActive;
            }
            if (State == WaveState.Damping && timeDelta > TransitionTime)
            {
                State = Wave.WaveState.Awake;
            }
            else
            {
                turnDelta = timeDeltaScale / TurnsActive;
            }
        }
        if (TurnsActive != -1 && IsActive)
        {
            y *= (TurnsActive - CurrentTurnActive + 1) / (float)TurnsActive + turnDelta;
        }
        return y;
    }

    public void DieOut()
    {
        State = WaveState.Dying;
        StartTransition();
    }

    public void onNewTurn()
    {
        State = WaveState.Damping;
        StartTransition();
        CurrentTurnActive++;
    }

    private void StartTransition()
    {
        _transitionTimestamp = Time.time;
    }
}
