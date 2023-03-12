/*
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KWUtils
{
    public class ObstaclesGrid : MonoBehaviour
    {
        private const int CellSize = 2;


        private void Update()
        {
            if (!Mouse.current.leftButton.wasReleasedThisFrame) return;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            ArrayPool<RaycastHit> arrayPool = ArrayPool<RaycastHit>.Shared;
            RaycastHit[] hits = arrayPool.Rent(1);
            if (Physics.RaycastNonAlloc(ray.origin, ray.direction, hits,math.INFINITY, 1<<8) != 0)
            {
                int currentGridIndex = GridUtils.GetIndexFromPosition(hits[0].point, GridSystem.MapBounds, 2);
                if(Grid.GetValue(currentGridIndex) == true) return;
                Grid.SetValue(currentGridIndex, true);
            }
        }
        
        private void OnDrawGizmos()
        {
            if(Grid == null) return;
            Vector3 cubeBounds = (Vector3.one * CellSize).SetAxis(Axis.Y, 0.5f);
            Gizmos.color = Color.red;
            for (int i = 0; i < Grid.GridArray.Length; i++)
            {
                if (Grid.GetValue(i) == false) continue;
                Gizmos.DrawCube(Grid.GetCellCenter(i), cubeBounds);
            }
        }
    }
}
*/