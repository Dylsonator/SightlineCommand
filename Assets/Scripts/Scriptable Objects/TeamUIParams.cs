using UnityEngine;

[CreateAssetMenu(menuName = "SightlineCommand/TeamUIParameters")]
public class TeamUIParams : ScriptableObject {
    public string teamName;
    public Sprite background;
    public Color textColor;
    public Color buttonColor;
}
