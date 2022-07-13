using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coords
{
    public static Vector2Int MapCoords(GameObject obj) {
        return new Vector2Int(
            (int)Mathf.Floor(obj.transform.position.x),
            (int)Mathf.Round(- (1f / 0.875f) * obj.transform.position.y));
    }

    public static Vector3 WorldCoords(Vector2Int coords) {
        return WorldCoords(coords, 0.0f);
    }

    public static Vector3 WorldCoords(Vector2Int coords, float z) {
        if (coords.y % 2 == 1) {
            return new Vector3(0.5f + coords.x, -0.865f * coords.y, z);
        }
        return new Vector3(coords.x, -0.865f * coords.y, z);
    }
}
