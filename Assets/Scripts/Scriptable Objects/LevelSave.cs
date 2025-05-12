using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSave : ScriptableObject {
    public List<TileData> tiles;
    public int width;
    public int height;
    public float scale;
    public float gapScale;
}
