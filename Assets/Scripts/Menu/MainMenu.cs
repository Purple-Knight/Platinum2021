using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MainMenu : MonoBehaviour
{
    public IList<Joystick> joysticks;
    private List<Player> players = new List<Player>();

    private void Awake()
    {
        joysticks = ReInput.controllers.GetJoysticks(); ///////////////////check connected disconected

        for (int i = 0; i < joysticks.Count; i++)
        {
            players.Add(ReInput.players.GetPlayer(i));
            Debug.Log(players.Count + " " + joysticks[i].name);
        }

    }

    void Update()
    {
        foreach (var item in players)
        {
            if (item.GetButtonDown("Start"))
            {
                Debug.Log("start pressed");
            }
        }
    }
}
