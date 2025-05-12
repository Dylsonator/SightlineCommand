using UnityEngine;

[CreateAssetMenu(menuName = "SightlineCommand/Building Cost Tree")]
public class BuildingCostTree : ScriptableObject {
    public int[] costs;
    public int numberActive;

    private void OnEnable() {
        numberActive = 0;
    }
}
