using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshCreator : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private bool _isInitialized = false;

    public delegate void OnNavmeshStartBake();
    public OnNavmeshStartBake OnNavmeshStartBakeEvent;

    public delegate void OnNavmeshBaked();
    public OnNavmeshBaked OnNavmeshBakedEvent;

    public void Initialize()
    {
        if (_navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface was not set!");
            return;
        }

        _isInitialized = true;
    }

    public void Bake()
    {
        if (!_isInitialized)
            Initialize();

        if (!_isInitialized)
            return;

        OnNavmeshStartBakeEvent?.Invoke();

        _navMeshSurface.BuildNavMesh();

        OnNavmeshBakedEvent?.Invoke();
    }
}