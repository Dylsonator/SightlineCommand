/// <summary>
/// Nate
/// Behaviour for the Operating Base, a unique building on each team.
/// Inherits from UnitCamp, giving it unit hiring functionality.
/// When the base is destroyed, the game ends.
/// </summary>
public class OperatingBase : UnitCamp {
    protected override void DeathBehaviour() {
        GameManager.Instance.EndGame(team);
    }
}
