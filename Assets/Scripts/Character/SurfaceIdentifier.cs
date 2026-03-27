using UnityEngine;

public enum SurfaceType
{
    Default,
    Rock
}

public class SurfaceIdentifier : MonoBehaviour
{
    [SerializeField] private SurfaceType surfaceType;

    public SurfaceType GetSurfaceType()
    {
        return surfaceType;
    }
}
