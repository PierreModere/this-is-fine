
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewsCard", order = 3)]
public class NewsCard: ScriptableObject
{
    public float id;
    public string title;
    public string boardCase;
    public Sprite newsTitle;
}