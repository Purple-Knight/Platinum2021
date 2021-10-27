using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelection : MonoBehaviour
{
    private static CharacterSelection _instance = null;
    public static CharacterSelection Instance { get => _instance; }

    private List<Player> players = new List<Player>();
    private List<Player> playersActual = new List<Player>();

    private void Awake()
    {
        _instance = this;
    }

    public void asignPlayers(List<Player> pList)
    {
        players = pList;
        foreach (var item in players)
        {
            Debug.Log(item.name);
        }
    }
}
