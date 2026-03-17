using UnityEngine;

public class TileChunk : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;

    private SpawnPoint[] spawnPoints;

    void Awake()
    {
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public SpawnPoint[] GetSpawnPoints()
    {
        return spawnPoints;
    }

    public float GetStartX()
    {
        return StartPoint.position.x;
    }

    public float GetEndX()
    {
        return EndPoint.position.x;
    }

    public void ClearGeneratedObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (child.CompareTag("Generated"))
                Destroy(child.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (StartPoint == null || EndPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(StartPoint.position, EndPoint.position);
    }
}