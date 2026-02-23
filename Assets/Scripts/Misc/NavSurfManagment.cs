using UnityEngine;
using NavMeshPlus.Components;
[RequireComponent(typeof(NavMeshSurface))]
public class NavSurfManagment : MonoBehaviour
{
    public static NavSurfManagment Instance { get; private set; }

    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        Instance = this;
        navMeshSurface = GetComponent<NavMeshSurface>();

        navMeshSurface.hideEditorLogs = true;
    }

    public void RebakeNavMashSurface()
    {
        navMeshSurface.BuildNavMesh();
    }

}
