using System.Collections.Generic;
using UnityEngine;

public sealed class TrackSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private List<TrackSegment> segmentPrefabs = new List<TrackSegment>();
    [SerializeField] private int initialSegments = 5;
    [SerializeField] private int keepSegmentsBehind = 2;
    [SerializeField] private float defaultSegmentLength = 30f;
    [SerializeField] private int redSegmentEvery = 3;
    [SerializeField] private int redSegmentOffset = 2;

    private readonly Queue<TrackSegment> activeSegments = new Queue<TrackSegment>();
    private float nextSpawnZ;
    private int spawnedSegmentCount;

    private void Start()
    {
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnNext();
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        while (player.position.z + defaultSegmentLength * initialSegments > nextSpawnZ)
        {
            SpawnNext();
        }

        RecycleBehindPlayer();
    }

    private void SpawnNext()
    {
        if (segmentPrefabs.Count == 0)
        {
            return;
        }

        TrackSegment prefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Count)];
        TrackSegment segment = Instantiate(prefab, new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity, transform);
        segment.ApplyColorTheme(GetSegmentTheme(spawnedSegmentCount));
        activeSegments.Enqueue(segment);
        spawnedSegmentCount++;
        nextSpawnZ += segment.Length > 0f ? segment.Length : defaultSegmentLength;
    }

    private EnergyMode GetSegmentTheme(int segmentIndex)
    {
        int interval = Mathf.Max(1, redSegmentEvery);
        int offset = Mathf.Clamp(redSegmentOffset, 0, interval - 1);
        return segmentIndex % interval == offset ? EnergyMode.Red : EnergyMode.Blue;
    }

    private void RecycleBehindPlayer()
    {
        while (activeSegments.Count > keepSegmentsBehind + initialSegments)
        {
            TrackSegment oldest = activeSegments.Peek();
            if (player.position.z - oldest.EndPosition.z < defaultSegmentLength)
            {
                break;
            }

            Destroy(activeSegments.Dequeue().gameObject);
        }
    }
}
