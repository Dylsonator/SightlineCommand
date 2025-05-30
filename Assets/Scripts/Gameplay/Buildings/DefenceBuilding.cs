using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Nate
/// Behaviour of a defence building, which attacks enemies that enter its range.
/// Child class of building, with custom parameters and adds listeners.
/// </summary>
public class DefenceBuilding : Building {
    [SerializeField]
    private int range = 1, damage = 1;

    [SerializeField]
    private Animator defenseAnimator;

    private List<Tile> tilesInRange = new List<Tile>();

    /// <summary>
    /// When spawned, add a listener to all tiles in range that damages units.
    /// Also increments price as normal.
    /// </summary>
    public override void SpawnBehaviour() {
        base.SpawnBehaviour();
        tilesInRange = tile.GetAdjacentGroup(range);
        foreach (var rangeTile in tilesInRange) {
            rangeTile.UnitMovedHere += UnitEnterRange;
        }
    }

    /// <summary>
    /// Removes listeners from all tiles on death, before destroying as normal.
    /// </summary>
    protected override void DeathBehaviour() {
        foreach (var rangeTile in tilesInRange) {
            rangeTile.UnitMovedHere -= UnitEnterRange;
        }
        base.DeathBehaviour();
    }

    /// <summary>
    /// Damages any unit that enters this building's range.
    /// Called by Action whenever a Unit enters a tile being watched.
    /// </summary>
    /// <param name="unit">The unit being damaged.</param>
    public void UnitEnterRange(Unit unit) {
        if (unit.team != team) {
            unit.TakeDamage(damage);
            defenseAnimator.SetTrigger("Attack");
        }
    }
}
