using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Nate
/// Defines building behaviour, which can be created by a Unit (or a level save).
/// This script is inherited by UnitCamp and DefenceBuilding, to expand functionality.
/// </summary>
public class Building : MonoBehaviour {
    //Prefab-assignable values, allowing for variants to be created easily.
    [Header("Values")]
    public BuildingCostTree price;
    public int capacity = 5;
    public int maxHealth = 5;
    public PlayerTeam team;
    public int materialYield;

    [Header("Panel Text")]
    public string buildingName;
    public string toolTip;
    public string command;

    [HideInInspector] public bool canActivate;

    [HideInInspector] public List<Unit> unitsHere;
    [HideInInspector] public Tile tile;

    [HideInInspector] public int health;
    private HealthBar healthBar;
    private SpriteRenderer unitIndicator;
    private TutorialCursor tutorialCursor;

    protected virtual void Awake() {
        tutorialCursor = Camera.main.GetComponent<TutorialCursor>();
        
        health = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.DisplaySpecified(maxHealth, maxHealth, team, true);
        healthBar.gameObject.SetActive(false);
        unitIndicator = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Ensures references to this object do not remain once it is destroyed.
    /// </summary>
    private void OnDestroy() {
        if (GameManager.Instance != null && !GameManager.Instance.editorStart) {
            GameManager.Instance.players[team].buildings.Remove(this);
        }
    }

    /// <summary>
    /// The code that runs when the activate button on the Building Panel is pressed.
    /// Empty by default, designed to be overwritten by child classes.
    /// </summary>
    /// <returns> Whether to clear the main buiding menu after activating. </returns>
    public virtual bool ActivateBehaviour() { return false; }

    /// <summary>
    /// Called whenever a building is spawned.
    /// By default, increments a scriptable object to increase price of spawning this building.
    /// </summary>
    public virtual void SpawnBehaviour() {
        if (price != null) {
            price.numberActive++;
        }
    }

    /// <summary>
    /// Called when health is (or below) 0.
    /// Decrements price, ejects all units within and destroys self.
    /// </summary>
    protected virtual void DeathBehaviour() {
        if (price != null) {
            price.numberActive--;
        }
        List<Unit> unitsToRemove = new List<Unit>(unitsHere); //Made temporary list to avoid errors when removing.
        foreach (var unit in unitsToRemove) {
            if (unit != null) {
                OnExitBehaviour(unit);
            }
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// When a unit enters the tile this building is on, add it to the list, hide it and display a chevron.
    /// </summary>
    /// <param name="unitEntered">The unit entering the building.</param>
    public virtual void OnEnterBehaviour(Unit unitEntered) {
        unitsHere.Add(unitEntered);
        tile.unitHere = null;
        unitEntered.transform.localScale = Vector3.zero;
        string chevronString = "Billboards/Chevron3";
        if (unitsHere.Count < 3) {
            chevronString = $"Billboards/Chevron{unitsHere.Count}";
        }
        Sprite indicatorSprite = Resources.Load<Sprite>(chevronString);
        unitIndicator.sprite = indicatorSprite;
    }

    /// <summary>
    /// Called when a unit is selected from the building panel (or the building is destroyed).
    /// Finds a tile with no units (if possible) and moves the player to that tile.
    /// Removes it from the list and updates the chevron display.
    /// </summary>
    /// <param name="unitLeaving">The unit being removed from the building.</param>
    public virtual void OnExitBehaviour(Unit unitLeaving) {
        Tile exitTile = tile;
        if (tile.unitHere != null) {
            exitTile = tile.FindEmptyTile();
            unitLeaving.MoveToTile(exitTile);
        }
        unitsHere.Remove(unitLeaving);
        exitTile.unitHere = unitLeaving;
        unitLeaving.transform.localScale = unitLeaving.unitScale;
        if (unitsHere.Count == 0) {
            unitIndicator.sprite = null;
        } else {
            string chevronString = "Billboards/Chevron3";
            if (unitsHere.Count < 3) {
                chevronString = $"Billboards/Chevron{unitsHere.Count}";
            }
            Sprite indicatorSprite = Resources.Load<Sprite>(chevronString);
            unitIndicator.sprite = indicatorSprite;
        }
    }

    /// <summary>
    /// Called whenever the turn changes, before the other team's turn begins.
    /// Adds this building's material yield, so they have more material next turn.
    /// (This is done at the end rather than beginning so both teams start with the same material).
    /// </summary>
    public void EndTurn() { 
        GameManager.Instance.players[team].material += materialYield;
    }

    /// <summary>
    /// Called whenever the turn changes, before this team's turn begins.
    /// Overriden by UnitCamp.
    /// </summary>
    public virtual void NewTurn() { }

    /// <summary>
    /// Called when in the range of an enemy's attack.
    /// Shows how much health the unit will have after being attacked.
    /// </summary>
    /// <param name="damage">Amount of health to lose in an attack.</param>
    public void IndicateHealth(int damage) {
        healthBar.gameObject.SetActive(true);
        healthBar.IndicateDamage(damage);
    }

    /// <summary>
    /// Reset health bar once enemy finishes aiming.
    /// </summary>
    public void StopIndicateHealth() {
        healthBar.gameObject.SetActive(false);
        healthBar.StopIndicating();
    }

    /// <summary>
    /// Called when an enemy attacks this building.
    /// Displays the damage, and calls death behaviour if health is <= 0.
    /// </summary>
    /// <param name="damage">Health to lose.</param>
    public void TakeDamage(int damage) {
        health -= damage;
        StartCoroutine(DelayHealthBarDisplay(damage, false));
        if (health <= 0) {
            DeathBehaviour();
        }
    }

    /// <summary>
    /// Currently unused outside of debug functions.
    /// Restores health to the building, and displays accordingly.
    /// </summary>
    /// <param name="heal">Health to gain (gain is capped at max health)</param>
    public void RepairDamage(int heal) {
        int trueHeal = Mathf.Min(heal, maxHealth - health);
        health += trueHeal;
        StartCoroutine(DelayHealthBarDisplay(trueHeal, true));
    }

    /// <summary>
    /// Show damage/healing in health bar, then hide after display is finished.
    /// </summary>
    /// <param name="value">Difference in health to display.</param>
    /// <param name="heal">Heal the difference instead of taking damage.</param>
    private IEnumerator DelayHealthBarDisplay(int value, bool heal) {
        yield return new WaitForEndOfFrame(); //Delays display as it sometimes gets disabled multiple times
        healthBar.gameObject.SetActive(true);
        if (heal) {
            healthBar.Heal(value);
        } else {
            healthBar.Damage(value);
        }
        while (healthBar.active) {
            yield return null;
        }
        healthBar.gameObject.SetActive(false);
    }
}
