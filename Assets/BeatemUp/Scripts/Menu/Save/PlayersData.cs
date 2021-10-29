using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayersData
{
    public int numberOfPlayer;
    public List<APlayerData> allPlayerData = new List<APlayerData>();
}

[System.Serializable]
public class APlayerData
{
    public int myCharID;
    public Color myColorID;
    public int playerControllerID;
}
