using UnityEngine;

[CreateAssetMenu(fileName = "Ending_New", menuName = "Game Over/Ending Data")]
public class EndingData : ScriptableObject
{
    public string endingName;
    public DialogueLine[] lines;
}