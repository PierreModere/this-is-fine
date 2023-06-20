
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Minigame", order = 1)]
public class Minigame : ScriptableObject
{
    public string title;
    public string instruction;
    public string id;
    public Sprite preview;
    public VideoClip videoPreview;
    public int gameTime;
    public int cutsceneTime;
    public bool hasTimer;
    public string[] goodFeedbacks;
    public string[] badFeedbacks;
 }