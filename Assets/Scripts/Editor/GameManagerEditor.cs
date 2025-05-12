using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
    //https://stackoverflow.com/questions/63928964/how-to-align-guilayout-elements
    private bool CentreButton(string label, int width, int height = 25) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool returnVal = GUILayout.Button(label, GUILayout.Height(height), GUILayout.Width(width));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        return returnVal;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GameManager script = (GameManager)target;
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Debug Functions (Use in-game only)", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);

        if (CentreButton("Restart Game", 200)) {
            script.RestartGame();
        }
        if (CentreButton("Refresh all unit actions", 200)) {
            foreach (var unit in script.players[PlayerTeam.HUMAN].units) {
                unit.ResetUnit();
            }
            foreach (var unit in script.players[PlayerTeam.ALIEN].units) {
                unit.ResetUnit();
            }
        }
        if (CentreButton("Make all tiles walkable", 200)) {
            foreach (Tile tile in script.tiles) {
                tile.terrainType.walkable = true;
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Hurt all units", GUILayout.Height(25))) {
            foreach (var unit in script.players[PlayerTeam.HUMAN].units) {
                unit.TakeDamage(1);
            }
            foreach (var unit in script.players[PlayerTeam.ALIEN].units) {
                unit.TakeDamage(1);
            }
        }
        if (GUILayout.Button("Heal all units", GUILayout.Height(25))) {
            foreach (var unit in script.players[PlayerTeam.HUMAN].units) {
                unit.Heal(100);
            }
            foreach (var unit in script.players[PlayerTeam.ALIEN].units) {
                unit.Heal(100);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Hurt all buildings", GUILayout.Height(25))) {
            foreach (var building in script.players[PlayerTeam.HUMAN].buildings) {
                building.TakeDamage(1);
            }
            foreach (var building in script.players[PlayerTeam.ALIEN].buildings) {
                building.TakeDamage(1);
            }
        }
        if (GUILayout.Button("Heal all buildings", GUILayout.Height(25))) {
            foreach (var building in script.players[PlayerTeam.HUMAN].buildings) {
                building.RepairDamage(100);
            }
            foreach (var building in script.players[PlayerTeam.ALIEN].buildings) {
                building.RepairDamage(100);
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reduce human material", GUILayout.Height(25))) {
            script.UseMaterial(PlayerTeam.HUMAN, 5);
        }
        if (GUILayout.Button("Reduce human tokens", GUILayout.Height(25)))
        {
            script.UseTokens(PlayerTeam.HUMAN, 1);
        }
    }
}
