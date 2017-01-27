using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Temporary class, thats gonna be merged into the Game Manager, I suppose.
/// </summary>

public interface IRecordable
{
    void Record();
    IEnumerator Playback();
}

public class Replays : MonoBehaviour {

    public static bool playback = false;
    bool _isRecording = false;
    public  Object[] allGameObjects;
    public List<IRecordable> objectsToRecordStates;
    // Use this for initialization
    void Start () {

        Invoke("GetObjects", .01f);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.F10))
            _isRecording = true;
        if (Input.GetKeyDown(KeyCode.F11) && _isRecording)
        {
            _isRecording = false;
            playback = true;
        }
        if (_isRecording)
        {
            // If an object implements the IRecordable interface - start recording. It will be run just after Player/AI shots, and stop when bullet(s) are destroyed
            objectsToRecordStates.ForEach(x => x.Record());
        }
        else if(playback && !_isRecording)
        {
            foreach(var temp in objectsToRecordStates)
            {
                // Only after enemy got killed
                StartCoroutine(temp.Playback());
            }
        }
    }

    void GetObjects()
    {
        allGameObjects = GameObject.FindObjectsOfType(typeof(MonoBehaviour));
        objectsToRecordStates = (from a in allGameObjects where a.GetType().GetInterfaces().Any(i => i == typeof(IRecordable)) select (IRecordable)a).ToList();
        allGameObjects = null;
    }
}
