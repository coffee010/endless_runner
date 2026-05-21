using UnityEngine;

public sealed class TrackSegment : MonoBehaviour
{
    [SerializeField] private float length = 30f;
    [SerializeField] private Transform endAnchor;

    public float Length => length;

    public Vector3 EndPosition
    {
        get
        {
            if (endAnchor != null)
            {
                return endAnchor.position;
            }

            return transform.position + Vector3.forward * length;
        }
    }
}
