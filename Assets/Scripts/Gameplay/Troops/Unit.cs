using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //done by both Dylan and Nate
    [Tooltip("The name this unit displays in UI.")]
    public string displayName;

    [Tooltip("The colour adjacent tiles are set to when the unit is moving.")]
    public Color[] moveableCol;
    public Color CurrentMoveableCol;

    public bool canBuild = true;
    [Tooltip("Every building this unit can create (if applicable)")]
    public List<Building> createableBuildings;

    public Tile currentTile;

    protected float scale;

    [Header("Troop Settings")]
    public PlayerTeam team;
    public int MaxMovement;
    public int MaxHealth;
    public int Damage;
    public int AttackRange;
    public int MaxAttack;
    public bool AOEAttack;
    public bool isFlying; //flying added by dylan
    public int tokenCost;
    public int turnsToCreate;

    [SerializeField]
    private AnimatorEventTrigger animateTrigger;

    [SerializeField]
    private ParticleSystem lowHealthParticles;

    private float rotationOffset;

    [HideInInspector] public int Health;
    [HideInInspector] public int CurrentMove;
    [HideInInspector] public int CurrentAttacks;
    [HideInInspector] public Vector3 unitScale;
    [HideInInspector] public HealthBar healthBar;
    [HideInInspector] public TextMeshPro valuesText;
    [HideInInspector] public List<Unit> enemiesInSight = new List<Unit>();
    [HideInInspector] public List<Building> buildingsInSight = new List<Building>();
    [HideInInspector] public bool inAction;

    private List<Tile> tilesTargetted = new List<Tile>();
    private List<PathTile> tilesPath = new List<PathTile>();

    private Tile tileHighlighted;
    private Unit enemyUnit;
    private Building enemyBuilding;

    private Animator animator;

    private Transform model;

    public void Start()
    {
        CurrentAttacks = MaxAttack;
        CurrentMoveableCol = moveableCol[0]; //sets up the moveable material
        CurrentMove = MaxMovement;
        Health = MaxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.DisplaySpecified(MaxHealth, MaxHealth, team);
        //healthBar.gameObject.SetActive(false);
        valuesText = GetComponentInChildren<TextMeshPro>();
        model = transform.Find("Model");
        rotationOffset = model.rotation.eulerAngles.y;
        //positionOffset = model.position;
        animator = model.GetComponent<Animator>();
    }
    //Movement///////////////////////////////////////////// Base Movement done by Nate, Limiting Movement Distance and changing movement material Done By Dylan
    public void UnitSpawn(Tile tile)
    {
        tile.unitHere = this;

        if (GameManager.Instance != null && !GameManager.Instance.editorStart) {
            GameManager.Instance.players[team].units.Add(this);
        }

        scale = tile.transform.localScale.x * 0.5f;
        unitScale = new Vector3(scale, scale, scale);
        transform.localScale = unitScale;

        MoveToTile(tile);
    }
    public void MoveToTile(Tile tile, bool animate = false)
    {
        currentTile = tile;
        tile.UnitMovedHere?.Invoke(this);
        if (animate) {
            StopAllCoroutines();
            PathTile pathToGo = null;
            foreach (PathTile path in tilesPath) { 
                if (path.tile == currentTile) {
                    pathToGo = path;
                }
            }
            StartCoroutine(AnimateToTile(pathToGo));
        } else {
            Vector3 position = currentTile.transform.position;
            position.y += scale * 0.5f;
            transform.position = position;
        }
        if (CurrentMove == 0)
        {
            CurrentMoveableCol = moveableCol[1]; //changes material to the NotMoveable
        }
    }

    private IEnumerator RotateToTarget(Vector3 targetPos, bool attack = false) {
        float angle = model.eulerAngles.y % 360;
        model.LookAt(targetPos);
        float endRot = (model.eulerAngles.y + rotationOffset) % 360;
        float currentVelocity = 0;
        Debug.Log(attack);
        float difference = Mathf.Abs(angle - endRot);
        while (difference > 1.5f && difference - 360 < -1.5f) {
            angle = Mathf.SmoothDampAngle(angle, endRot, ref currentVelocity, 0.25f);
            model.rotation = Quaternion.Euler(0, angle, 0);
            yield return null;
            difference = Mathf.Abs(angle - endRot);
            //Debug.Log($"{angle} - {endRot} = {Mathf.Abs(angle - endRot)}");
        }
        Debug.Log(attack);
        if (attack) {
            
            animator.SetTrigger("Attacking");
        }
    }

    private IEnumerator AnimateToTile(PathTile tilePath) {
        animator.SetBool("Walking", true);
        inAction = true;
        foreach (Tile tileToGo in tilePath.path){
            Vector3 startPosition = transform.position;
            Vector3 targetPos = tileToGo.transform.position;
            targetPos.y += scale * 0.5f;
            StartCoroutine(RotateToTarget(targetPos));
            float time = 0;
            while (time < 1) {
                Vector3 pos = Vector3.Lerp(startPosition, targetPos, time);
                transform.position = pos;
                time += Time.deltaTime * 0.5f;
                yield return null;
            }
        }
        
        inAction = false;
        animator.SetBool("Walking", false);
    }

    public void BeginMove()
    {
        EndTargeting();
        tilesPath = currentTile.GetWalkableGroup(CurrentMove, isFlying);
        tilesTargetted = tilesPath.Select(path => path.tile).ToList();
        foreach (Tile adjacentTile in tilesTargetted)
        {

            if (adjacentTile.terrainType.walkable == true || isFlying)
            {
                adjacentTile.DisplayColour(CurrentMoveableCol);
            }
        }
        
    }
    public void EndMove(Tile targetTile)
    {
        

        if (CurrentMove >= targetTile.terrainType.travelSpeed && tilesTargetted.Contains(targetTile))
        {
            bool moved = false;
            if (!targetTile.unitHere) {
                targetTile.unitHere = this;
                moved = true;
            }

            if (targetTile.buildingHere) {
                if (targetTile.buildingHere.team == team) {
                    targetTile.buildingHere.OnEnterBehaviour(this);
                    moved = true;
                }
                else {
                    moved = false;
                }
            }

            if (moved) {
                CurrentMove = 0;
                currentTile.unitHere = null;              
                //Debug.Log(CurrentMove);
                MoveToTile(targetTile, true);
                EndTargeting();
            }
        }
    }

    public void ResetMove()
    {
        CurrentMove = MaxMovement;
    }
    //End Of Movement////////////////////////////////////////

    //Health///////////////////////////////////////////////// Done By Dylan

    public void TakeDamage(int damageDealt)
    {
        Health -= damageDealt;
        healthBar.Damage(damageDealt);
        if (Health <= 1 && lowHealthParticles != null && !lowHealthParticles.isPlaying) {
            lowHealthParticles.Play();
        }
        if (Health <= 0)
        {
            animator.SetTrigger("Defeated");
            healthBar.gameObject.SetActive(false);
            if (animateTrigger != null) {
                animateTrigger.AnimEvent += DestroySelf;
            } else {
                DestroySelf();
            }
        } else {
            animator.SetTrigger("Damaged");
        }

    }
    //End of Health//////////////////////////////////////////

    private void DestroySelf() {
        if (animateTrigger != null) {
            animateTrigger.AnimEvent -= DestroySelf;
        }
        Destroy(this.gameObject);
    }

    public void Heal(int healingDealt) {
        int trueHeal = Mathf.Min(healingDealt, MaxHealth - Health);
        Health += trueHeal;
        healthBar.Heal(trueHeal);
    }

    //Damage and Targeting/////////////////////////////////// Done By Dylan & Nate

    public void MarkAdjacentTiles(Tile tileToCheck, int maxLoops, bool dmgIndicate = false)
    {
        EndTargeting(); //Hopefully doesn't cause issues, but if multiple things are targetted at once it will (Shouldn't happen but might).
        tilesTargetted = tileToCheck.GetAdjacentGroup(maxLoops);
        foreach (Tile tile in tilesTargetted) { 
            tile.DisplayColour(CurrentMoveableCol);
            if (dmgIndicate) {
                if (tile.unitHere && tile.unitHere.team != team) {
                    tile.unitHere.healthBar.IndicateDamage(Damage);
                    enemiesInSight.Add(tile.unitHere);
                }
                if (tile.buildingHere && tile.buildingHere.team != team) {
                    tile.buildingHere.IndicateHealth(Damage);
                    buildingsInSight.Add(tile.buildingHere);
                }
            }
        }
    }

    public void EndTargeting() {
        
        foreach (Tile tile in tilesTargetted) {
            tile.ResetMaterial();
        }
        tilesTargetted.Clear();
        foreach (Unit unit in enemiesInSight) {
            unit.healthBar.StopIndicating();
        }
        enemiesInSight.Clear();
        foreach (Building building in buildingsInSight) {
            building.StopIndicateHealth();
        }
        buildingsInSight.Clear();
    }

    // Currently unused code for highlighting the currently hovered tile, with AOE attacks

    //public void HighlightTile(Tile tile) {
    //    if (tile == tileHighlighted) {
    //        return;
    //    }
    //    if (tileHighlighted != null) {
    //        UnHighlightTiles();
    //    }
    //    if (!tilesTargetted.Contains(tile)) {
    //        return;
    //    }
    //    tileHighlighted = tile;
    //    foreach (Tile tileToHighlight in tile.GetAdjacentGroup(AOEAttack ? 2 : 1)) {
    //        tileToHighlight.DisplayColour(moveableCol[1]);
    //    }
    //}

    //public void UnHighlightTiles() {
    //    float startTime = 0;
    //    foreach (Tile markedTile in tilesTargetted) {
    //        if (!markedTile.highlighted) {
    //            startTime = markedTile.lerpTime;
    //            break;
    //        }
    //    }
    //    foreach (Tile oldTile in tileHighlighted.GetAdjacentGroup(AOEAttack ? 2 : 1)) {
    //        oldTile.lerpTime = startTime;
    //        oldTile.DisplayColour(CurrentMoveableCol);
    //    }
    //    tileHighlighted = null;
    //}

    //End Of Damage and Targeting///////////////////////////// Done By Dylan

    public void Attack(Vector3 attackPos) {
        EndTargeting();
        attackPos.y += scale * 0.5f;
        StopCoroutine("RotateToTarget");
        StartCoroutine(RotateToTarget(attackPos, true));

        inAction = true;
        CurrentAttacks--;
    }

    public void AttackUnit(Tile enemyTile){
        enemyUnit = enemyTile.unitHere; //if the tile clicked has an enemy unit on it do animation and damage
        Attack(enemyTile.transform.position);
        if (animateTrigger != null) {
            animateTrigger.AnimEvent += DamageEnemy;
            Debug.Log("a");
        } else {
            DamageEnemy();
        }
    }

    public void AttackBuilding(Tile enemyTile) {
        enemyBuilding = enemyTile.buildingHere;
        Attack(enemyTile.transform.position);
        if (animateTrigger != null) {
            animateTrigger.AnimEvent += DamageBuilding;
        } else {
            DamageBuilding();
        }
    }

    public void DamageEnemy() {
        Debug.Log("b");
        if (animateTrigger != null) {
            animateTrigger.AnimEvent -= DamageEnemy;
        }
        enemyUnit.TakeDamage(Damage);
        inAction = false;
    }

    public void DamageBuilding() {
        if (animateTrigger != null) {
            animateTrigger.AnimEvent -= DamageBuilding;
        }
        enemyBuilding.TakeDamage(Damage);
        inAction = false;
    }

    //Creating buildings////////////////////////////////////// Done by Nate

    public void ShowBuildMenu() {
        if (canBuild && currentTile.buildingHere == null) {
            GameManager.Instance.gameUI.ShowBuildingBuyMenu(this);  //COMMENTED OUT
        }
    }

    public bool CreateBuilding(Building building) {
        int buildingCost = building.price.costs[Mathf.Min(building.price.numberActive, building.price.costs.Length - 1)];
        if (GameManager.Instance.UseMaterial(team, buildingCost)) {
            currentTile.CreateBuilding(building);
            currentTile.buildingHere.OnEnterBehaviour(this);
            return true;
        }
        return false;
    }

    //End of creating buildings//////////////////////////////
    public void ResetUnit()
    {
        CurrentMove = MaxMovement;
        CurrentAttacks = MaxAttack;
        CurrentMoveableCol = moveableCol[0];
    }

    private void OnDestroy() {
        if (GameManager.Instance != null && !GameManager.Instance.editorStart) {
            GameManager.Instance.AddTokens(team, tokenCost);
            GameManager.Instance.players[team].units.Remove(this);
        }
        
    }
}
