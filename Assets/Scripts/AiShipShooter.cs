using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShipShooter : ShipShooter {

    public GameObject AimProjection;

    protected override void ChangeWeapon()
    {
       // me not know how
    }

    protected override bool IsShootReleased()
    {
        return _firePressed  == false;
    }
    protected override bool IsShootPressed()
    {
        return _firePressed;
    }


    private GameObject player = null;
    private float aim = 0f;
    private float desiredPower = 0f;
    protected override float AimResult()
    {
        return angle < aim ? 1f : -1f;
    }

    protected override void OnCurrentSetActive(bool isActive)
    {
        player = FindObjectOfType<Ship>().gameObject;

        if (isActive == false)
            return;
        aim = Random.Range(22f, 35f);
        desiredPower = Random.Range(ShipShooter._minPower * 1.3f, ShipShooter._maxPower * 0.7f);

        // calculate stuff
        float start = Time.realtimeSinceStartup;
        float howFar = 100f;
        float forceDelta = 3f;
        float angleDelta = 5f;
        int passes = 0;
        bool adding = true;
        while (passes++ < 50)
        {
            float newHowFar = howFarToHit();
            if (howFar < 0.1f)
            {
                howFar = newHowFar;
                break;
            }

            if (adding && newHowFar > howFar)
            {
                forceDelta *= 0.7f;
                adding = false;
            }
            else if (!adding && newHowFar > howFar)
            {
                forceDelta *= 0.7f;
                adding = true;
            }

            if (adding) desiredPower += forceDelta; else desiredPower -= forceDelta;

            howFar = newHowFar;
        }

        Debug.Log("How far: " + howFar + " |POWER| passes: " + passes + " power: " + desiredPower + " aim: " + aim);
        passes = 0;
        adding = true;
        howFar = 100f;
        while (passes++ < 50)
        {
            float newHowFar = howFarToHit();
            if (howFar < 0.1f)
            {
                howFar = newHowFar;
                break;
            }

            if (adding && newHowFar > howFar)
            {
                angleDelta *= 0.7f;
                adding = false;
            }
            else if (!adding && newHowFar > howFar)
            {
                angleDelta *= 0.7f;
                adding = true;
            }

            if (adding) aim += angleDelta; else aim -= angleDelta;
            aim = Mathf.Clamp(aim, clampMin, clampMax);
            howFar = newHowFar;
        }
        Debug.Log("How far: " + howFar + " |AIM| passes: " + passes + " power: " + desiredPower + " aim: " + aim);

        Debug.Log("calcTime: " + (Time.realtimeSinceStartup - start));

        // draw stuff
        List<Vector3> calculatedPoints = new List<Vector3>();
        float wind = GameManager.Instance.WindForce;
        float celownikAngle = (aim + 90) * Mathf.Deg2Rad;
        for (int points = 0; points < 20; points++)
        {
            float t = points * 0.3f;
            var current = new Vector3(transform.position.x + Celownik.getForward(t, celownikAngle, wind, 10f, desiredPower),
                transform.position.y + 0.3f + Celownik.getDown(t, celownikAngle, 10f, desiredPower));
            calculatedPoints.Add(current - transform.position);
        }
        if (AimProjection != null)
        {
            var line = AimProjection.GetComponent<LineRenderer>();
            line.positionCount = calculatedPoints.Count;
            line.SetPositions(calculatedPoints.ToArray());
        }
        _gunState = GunState.Aiming;
        Invoke("StartShooting", 2f);
    }

    protected override void OnFixedUpdate()
    {
        StopShooting();
        base.OnFixedUpdate();
    }

    private bool _firePressed = false;
    private void StartShooting()
    {
        _firePressed = true;
    }
    private void StopShooting()
    {
        if (_gunState == GunState.AdjustingPower && ShootPower > desiredPower)
        {
            _firePressed = false;
        }
    }

    private float howFarToHit()
    {
        const float epsilon = 0.1f;
        const float intervalEpsilon = 0.005f;
        Vector2 target = player.transform.position;
        float previousHowFar;
        float howFar = Vector2.Distance(transform.position, target);
        float t = 0.5f;
        float interval = 0.1f;
        float wind = GameManager.Instance.WindForce;
        float celownikAngle = (aim + 90) * Mathf.Deg2Rad;
        Vector3 current;
        int passes = 0;
        do
        {
            previousHowFar = howFar;
            current = new Vector3(transform.position.x + Celownik.getForward(t, celownikAngle , wind, 10f, desiredPower),
                transform.position.y + 0.3f + Celownik.getDown(t, celownikAngle , 10f, desiredPower));
            howFar = Vector2.Distance(current, target);
            if(target.y > current.y * Mathf.Sign(interval))
            {
                interval /= -2f;
            }
            t += interval;
        }
        while(Mathf.Abs((current.y - target.y)) > epsilon && Mathf.Abs(interval) > intervalEpsilon && passes++ < 100);
        return howFar;
    }

}
