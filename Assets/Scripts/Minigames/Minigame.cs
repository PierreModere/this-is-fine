
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Minigame", order = 1)]
public class Minigame: ScriptableObject
{
    public string title;
    public string instruction;
    public string id;
    public Sprite preview;
}