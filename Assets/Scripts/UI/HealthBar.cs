using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBar : MonoBehaviour {
    //https://www.youtube.com/watch?v=0tDPxNB2JNs
    [SerializeField] 
    private Image healthFill;

    [SerializeField]
    private Image healthLoss;

    [SerializeField]
    private TextMeshProUGUI textDisplay;

    [SerializeField, Tooltip("What colours the health bar should be.\n0: Human Unit\n1: Alien Unit\n2: Human Building\n3: Alien Building")]
    private Color[] healthColors = new Color[4];

    private float maxHealth;
    private float currentHealth;
    public bool active;

    public void DisplaySpecified(float max, float current, PlayerTeam team, bool building = false) { 
        maxHealth = max;
        currentHealth = current;
        Color fillColor = Color.white;
        if (team == PlayerTeam.ALIEN) {
            fillColor = building ? healthColors[3] : healthColors[1];
        } else {
            fillColor = building ? healthColors[2] : healthColors[0];
        }
        healthFill.color = fillColor;
        healthLoss.color = fillColor;
        UpdateDisplay();
    }

    public void IndicateDamage(float dmg) {
        healthFill.fillAmount = (currentHealth - dmg) / maxHealth;
        healthLoss.fillAmount = dmg / maxHealth;
        healthLoss.transform.localPosition = new Vector2(healthFill.fillAmount * 180, 0);
    }

    public void StopIndicating() {
        healthLoss.fillAmount = 0;
        healthLoss.transform.localPosition = Vector3.zero;
        UpdateDisplay();
    }

    public void Damage(float dmg) {
        StopAllCoroutines();
        StartCoroutine(AnimateChange(currentHealth, currentHealth - dmg));
        currentHealth -= dmg;
    }

    public void Heal(float hp) {
        if (hp == 0) {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(AnimateChange(currentHealth, currentHealth + hp));
        currentHealth += hp;
    }

    private IEnumerator AnimateChange(float start, float end) {
        active = true;
        float time = 0;
        start = start / maxHealth;
        end = end / maxHealth;
        while (time < 1) {
            healthFill.fillAmount = Mathf.Lerp(start, end, time);
            time += Time.deltaTime * 4;
            yield return null;
        }
        UpdateDisplay();
        yield return new WaitForSeconds(0.75f);
        active = false;
    }

    private void UpdateDisplay() {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthFill.fillAmount = currentHealth / maxHealth;
        textDisplay.text = $"{currentHealth}/{maxHealth}";
    }
}

