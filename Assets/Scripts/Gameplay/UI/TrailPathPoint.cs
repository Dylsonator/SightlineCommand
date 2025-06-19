using UnityEngine;
using TMPro;

public class TrailPathPoint : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI numberIndicator;
    
    public void ShowNumber(int number) {
        if (numberIndicator) {
            numberIndicator.text = number.ToString();
        }
    }
}
