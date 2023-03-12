using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnnemyManager : MonoBehaviour
{
    [SerializeField] private Transform prefab;
    [SerializeField] private EnnemiGrid grid;
    private List<Transform> ennemies;

    private void Awake()
    {
        ennemies = new List<Transform>(6);
        grid = grid == null ? FindObjectOfType<EnnemiGrid>() : grid;
    }

    // Update is called once per frame
    private void Update()
    {
        SpawnEntities();
        MoveEntities();
    }

    private void SpawnEntities()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        if (prefab == null) return;
        for (int i = 0; i < grid.GetStartPoints.Count; i++)
        {
            ennemies.Add(Instantiate(prefab, grid.GetStartPoints[i], Quaternion.identity));
        }
    }

    private void MoveEntities()
    {
        if (ennemies.Count == 0) return;
        List<Transform> entityRemoved = new List<Transform>(ennemies.Count);

        int2 gridBounds = new int2(grid.gridWidth, grid.gridHeight);
        Bounds endBound = new Bounds(grid.EndTransform.position, grid.EndTransform.localScale);
        for (int i = 0; i < ennemies.Count; i++)
        {
            int indexInGrid = GridUtils.GetIndexFromPositionOffset(ennemies[i].position, gridBounds);
            ennemies[i].position += grid.GetDirectionAt(indexInGrid) * (Time.deltaTime * 5);
            if (endBound.Contains(ennemies[i].position))
            {
                entityRemoved.Add(ennemies[i]);
            }
        }

        if (entityRemoved.Count == 0) return;
        for (int i = 0; i < entityRemoved.Count; i++)
        {
            ennemies.Remove(entityRemoved[i]);
            Destroy(entityRemoved[i].gameObject);
        }
    }
    
    
}
