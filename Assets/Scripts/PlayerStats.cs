using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerStats", menuName = "Assets/Scriptables/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public string playerName;
    public int playerScore;
}
