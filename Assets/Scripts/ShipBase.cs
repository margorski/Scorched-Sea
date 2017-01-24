using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    void Die();
}

public class ShipBase : MonoBehaviour, IHitable
{
    delegate void Destroy();
    Destroy destroyAll;
    public bool _isDead = false;
    protected Transform gun;
    List<Transform> allElements;
    Transform _deck;
    Transform _leftSide;
    Transform _rightSide;
    Transform _Bottom;
    public LineRenderer ofDeck;
    protected Vector3 rendererPosition;
    protected Vector3 startPowerBarPosition;

    void Awake()
    {
        allElements = new List<Transform>();
        gun = transform.FindChild("Dzialo");
        allElements.Add(gun);
        _deck = transform.FindChild("Poklad");
        allElements.Add(_deck);
        _leftSide = transform.Find("LewaBurta");
        allElements.Add(_leftSide);
        _rightSide = transform.Find("PrawaBurta");
        allElements.Add(_rightSide);
        _Bottom = transform.Find("Dno");
        allElements.Add(_Bottom);
        _isDead = false;

        rendererPosition = _deck.GetComponent<LineRenderer>().GetPosition(0);

        float deltaX;
        if (Mathf.Sign(transform.position.x) > 0.0f)
            deltaX = 0.9f;
        else
            deltaX = -0.3f;

        rendererPosition = new Vector3(rendererPosition.x + deltaX, rendererPosition.y, rendererPosition.z);

        for (int i = 0; i < 2; i++)
        {
            ofDeck.SetPosition(i, rendererPosition);
        }
        startPowerBarPosition = new Vector3(rendererPosition.x, rendererPosition.y, rendererPosition.z);

        destroyAll = DestroyElementsDown;
        destroyAll += DestroyElementsLeft;
        destroyAll += DestroyElementsRight;
        destroyAll += DestroyElementsUp;
    }


    protected virtual void OnDie() { }
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        GameManager.Instance.SoundPlayer.PlayExplosion();
        OnDie();
    }

    protected virtual float OnFixedUpdateCalcX(float boatAngle) { return transform.position.x; }
    protected virtual void OnFixedUpdate() { }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isDead)
        {
            destroyAll();
            Invoke("DeadAll", 1.5f);
            return;
        }

        float boatAngle;
        var boatY = Waver.Instance.GetY(transform.position.x, out boatAngle);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, boatAngle);
        var newX = OnFixedUpdateCalcX(boatAngle);
        transform.position = new Vector3(newX, boatY, transform.position.z);

        OnFixedUpdate();
    }

    protected bool _isInControl = false;
    public void SetCurrent(bool current)
    {
        _isInControl = current;
        var width = 0.02f;
        if (current)
            width += 0.05f;
        var colorGreen = Color.green;
        var colorYellow = Color.yellow;
        var colorRed = Color.red;
        Color currentColor;
        //  if (GameManager.Instance.Players.IndexOf(this) == 0)
        //      currentColor = colorYellow;
        //  else
        //     currentColor = colorRed;
        currentColor = Color.white;
        if (current)
        {
            foreach (var element in allElements)
            {
                var temp = element.GetComponent<LineRenderer>();
                temp.endColor = temp.startColor = currentColor;
                temp.endWidth = temp.startWidth = width;
            }
        }
        else
        {
            foreach (var element in allElements)
            {
                var temp = element.GetComponent<LineRenderer>();
                temp.endColor = temp.startColor = currentColor;
                temp.endWidth = temp.startWidth = width;
            }
        }
    }

    void DestroyElementsRight()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(-0.005f, 0.01f);
        float speed = Random.Range(2, 4.5f);
        _rightSide.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _rightSide.position = new Vector3(_rightSide.position.x + randX, _rightSide.position.y + randY, _rightSide.position.z);
    }

    void DestroyElementsLeft()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(-0.005f, 0.005f);
        float speed = Random.Range(2, 4.5f);
        _leftSide.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _leftSide.position = new Vector3(_leftSide.position.x - randX, _leftSide.position.y - randY, _leftSide.position.z);
    }

    void DestroyElementsDown()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(0.001f, 0.005f);
        float speed = Random.Range(2, 4.5f);
        _Bottom.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _Bottom.position = new Vector3(_Bottom.position.x - randX, _Bottom.position.y - randY, _Bottom.position.z);
    }

    void DestroyElementsUp()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(0.001f, 0.005f);
        float speed = Random.Range(2, 4.5f);
        _deck.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));

        _deck.position = new Vector3(_deck.position.x - randX, _deck.position.y + randY, _deck.position.z);
    }

    void DestroyElementsGun()
    {
        float randX = Random.Range(0.005f, 0.01f);
        float randY = Random.Range(0.005f, 0.009f);
        float speed = Random.Range(2, 4.5f);
        gun.Rotate(new Vector3((int)Random.Range(-1, 1) * speed, 0, (int)Random.Range(-1, 1) * speed));
        gun.position = new Vector3(gun.position.x - randX, gun.position.y + randY, gun.position.z);

    }
    void DeadAll()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Ship"))
        {
            collider.gameObject.GetComponent<IHitable>().Die();
            Die();
        }
    }
}
