#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SnapSpawnPointsTool
{
    // This creates a new clickable button at the very top of your Unity Editor
    [MenuItem("Tools/Snap Spawn Points To Ground")]
    public static void SnapThem()
    {
        // Get whatever objects you currently have highlighted in the hierarchy
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Please select a TileChunk in the hierarchy first!");
            return;
        }

        int snappedCount = 0;

        foreach (GameObject obj in selectedObjects)
        {
            // Find all SpawnPoint components inside the selected object (and its children)
            SpawnPoint[] points = obj.GetComponentsInChildren<SpawnPoint>(true);

            foreach (SpawnPoint pt in points)
            {
                // Cast a ray straight down. 20f distance should be plenty!
                RaycastHit2D hit = Physics2D.Raycast(pt.transform.position, Vector2.down, 20f);

                if (hit.collider != null)
                {
                    // Record this action so you can use CTRL+Z (Undo) if you don't like it!
                    Undo.RecordObject(pt.transform, "Snap Spawn Point");

                    pt.transform.position = hit.point;

                    // Tell Unity this prefab was modified so it knows to save it
                    EditorUtility.SetDirty(pt.transform);
                    snappedCount++;
                }
                else
                {
                    Debug.LogWarning($"SpawnPoint '{pt.gameObject.name}' missed the ground. Is it floating too high?");
                }
            }
        }

        Debug.Log($"Done! Successfully snapped {snappedCount} spawn points to the ground.");
    }
}
#endif