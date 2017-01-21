using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {
    public Vector3 pointZero = Vector3.zero;
    public float MaxWindLength = 1.5f;

    private LineRenderer windForceRenderer;
    private LineRenderer windGrotURenderer;
    private LineRenderer windGrotDRenderer;
    private Text player1Name;
    private Text player1Stats;
    private Text player1Weapon1;
    private Text player1Weapon2;
    private Text player2Name;
    private Text player2Stats;
    private Text player2Weapon1;
    private Text player2Weapon2;
    private Text WinMessage;
    // Use this for initialization
    void Start()
    {
        windForceRenderer = transform.FindChild("Wind/Force").gameObject.GetComponent<LineRenderer>();
        windGrotURenderer = transform.FindChild("Wind/GrotD").gameObject.GetComponent<LineRenderer>();
        windGrotDRenderer = transform.FindChild("Wind/GrotU").gameObject.GetComponent<LineRenderer>();
        player1Name = transform.FindChild("Player1Info/Name").gameObject.GetComponent<Text>();
        player1Stats = transform.FindChild("Player1Info/Statistics").gameObject.GetComponent<Text>();
        player1Weapon1 = transform.FindChild("Player1Info/Weapons/Weapon1Name").gameObject.GetComponent<Text>();
        player1Weapon2 = transform.FindChild("Player1Info/Weapons/Weapon2Name").gameObject.GetComponent<Text>();
        player2Name = transform.FindChild("Player2Info/Name").gameObject.GetComponent<Text>();
        player2Stats = transform.FindChild("Player2Info/Statistics").gameObject.GetComponent<Text>();
        player2Weapon1 = transform.FindChild("Player2Info/Weapons/Weapon1Name").gameObject.GetComponent<Text>();
        player2Weapon2 = transform.FindChild("Player2Info/Weapons/Weapon2Name").gameObject.GetComponent<Text>();
        WinMessage = transform.FindChild("WinMsg").gameObject.GetComponent<Text>(); ;
        WinMessage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void FixedUpdate()
    {
        DrawWindArrow();
        //player1Name.text = GameManager.Instance.Players[0].name;
        //player1Stats = "W: " + GameManager.Instance.Players[0].wins + " L: " + GameManager.Instance.Players[0].loss;
        //weapon
        //player2Name.text = GameManager.Instance.Players[1].name;
        //player2Stats = "W: " + GameManager.Instance.Players[1].wins + " L: " + GameManager.Instance.Players[1].loss;
        //weapon
        WinMessage.enabled = (GameManager.Instance.TurnPhase == GameManager.TurnPhaseType.EndOfRound);
        if (WinMessage.enabled)
        {
            // WinMessage.text = GameManager.Instance.GetWinPlayer().name;

        }
    }

    void DrawWindArrow()
    {
        var windForce = Mathf.Abs(GameManager.Instance.WindForce) / GameManager.Instance.MaxWind;
        var windDirection = Mathf.Sign(GameManager.Instance.WindForce);
        var grotLength = 0.2f;

        if (windForce == 0.0f)
        {
            windForceRenderer.SetPosition(0, pointZero);
            windForceRenderer.SetPosition(1, pointZero);
            windGrotURenderer.SetPosition(0, pointZero);
            windGrotURenderer.SetPosition(1, pointZero);
            windGrotDRenderer.SetPosition(0, pointZero);
            windGrotDRenderer.SetPosition(1, pointZero);
        }
        else
        {
            var windLength = MaxWindLength * windForce * 0.5f;
            windForceRenderer.SetPosition(0, pointZero - new Vector3(windLength, 0.0f, 0.0f));
            windForceRenderer.SetPosition(1, pointZero + new Vector3(windLength, 0.0f, 0.0f));
            if (windDirection > 0.0f)
            {
                var arrowTopPoint = windForceRenderer.GetPosition(1);
                windGrotURenderer.SetPosition(0, arrowTopPoint);
                windGrotURenderer.SetPosition(1, arrowTopPoint + new Vector3(-grotLength, -grotLength, 0.0f));
                windGrotDRenderer.SetPosition(0, arrowTopPoint);
                windGrotDRenderer.SetPosition(1, arrowTopPoint + new Vector3(-grotLength, grotLength, 0.0f));

            }
            else
            {
                var arrowTopPoint = windForceRenderer.GetPosition(0);
                windGrotURenderer.SetPosition(0, arrowTopPoint);
                windGrotURenderer.SetPosition(1, arrowTopPoint + new Vector3(grotLength, -grotLength, 0.0f));
                windGrotDRenderer.SetPosition(0, arrowTopPoint);
                windGrotDRenderer.SetPosition(1, arrowTopPoint + new Vector3(grotLength, grotLength, 0.0f));
            }
        }
    }


}