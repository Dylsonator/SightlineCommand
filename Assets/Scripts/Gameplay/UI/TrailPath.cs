using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailPath : MonoBehaviour {
    [SerializeField]
    private TrailPathPoint prefabPoint;

    private LineRenderer _lr;

    private List<TrailPathPoint> indicators = new List<TrailPathPoint>();

    private void Awake() {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 0;
        for (int i = 0; i < 5; i++) {
            AddPoint();
        }
    }

    private void AddPoint() {
        TrailPathPoint newPoint = Instantiate(prefabPoint.gameObject).GetComponent<TrailPathPoint>();
        indicators.Add(newPoint);
        newPoint.gameObject.SetActive(false);
    }

    public void SetPath(List<Tile> tiles, float scale = 0) {
        HidePath();

        while (tiles.Count > indicators.Count) {
            AddPoint();
        }

        List<Vector3> points = tiles.Select(x => x.transform.position + new Vector3(0, scale * 0.5f, 0)).ToList();

        int totalCost = 0 - tiles[0].terrainType.travelSpeed;

        for (int i = 0; i < points.Count; i++) {
            indicators[i].gameObject.SetActive(true);
            
            totalCost += tiles[i].terrainType.travelSpeed;
            Debug.Log($"{i}: {tiles[i].terrainType.travelSpeed} = {totalCost}");
            indicators[i].ShowNumber(totalCost);
            indicators[i].transform.position = points[i];
        }
        Debug.Log("Final: " + totalCost);

        _lr.positionCount = points.Count;
        _lr.SetPositions(points.ToArray());
    }

    public void HidePath() {
        _lr.positionCount = 0;
        foreach (var indicator in indicators) {
            indicator.gameObject.SetActive(false);
        }
    }
}
