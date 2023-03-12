using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using static Unity.Jobs.LowLevel.Unsafe.JobsUtility;

public class EnnemiGrid : MonoBehaviour
{
    public bool GizmosDebug = false;
    public bool SpawnsPointsDebug = false;
    public bool FlowFieldDebug = false;
    public bool CostFieldDebug = false;
    public bool IntegrationFieldDebug = false;
    
    [field:SerializeField] public Transform StartTransform{ get; private set; }
    [field:SerializeField] public Transform EndTransform{ get; private set; }
    private Transform terrainTransform;

    public int gridWidth { get; private set; } = 0;
    public int gridHeight { get; private set; } = 0;
    public int totalCells { get; private set; } = 0;

    private int startIndex = -1;
    private int destinationIndex = -1;

    private Vector3[] grid;
    public int[] debugIntegrationField;
    public byte[] debugCostField;
    private bool[] turretGrid;
    
    //CostField
    //private NativeArray<bool> nativeObstacles;
    //private NativeArray<byte> nativeCostField;
    //IntegrationField
    //private NativeArray<int> nativeBestCostField;
    //FlowField
    //private NativeArray<float3> nativeBestDirection;

    private Vector3[] flowField;

    private bool jobSchedule;
    private JobHandle lastJobScheduled;

    private List<Vector3> spawnPoints;
    public Vector3[] GetGridPositions() => grid;
    public Vector3 GetDirectionAt(int index) => flowField[index];
    public List<Vector3> GetStartPoints => spawnPoints;
    
    private void Awake()
    {
        terrainTransform = transform;
        gridHeight = (int)terrainTransform.localScale.y;
        gridWidth = (int)terrainTransform.localScale.x;
        totalCells = gridWidth * gridHeight;

        flowField = new Vector3[totalCells];
        
        startIndex = GridUtils.GetIndexFromPositionOffset(StartTransform.position, new int2(gridWidth, gridHeight));
        destinationIndex = GridUtils.GetIndexFromPositionOffset(EndTransform.position, new int2(gridWidth, gridHeight));
    }

    private void Start()
    {
        ConstructGrid();
        GetObstacles();
        CalculateFlowField();
        GetStartPositions();
    }

    private void ConstructGrid()
    {
        totalCells = totalCells == 0 ? (int)(terrainTransform.localScale.x * terrainTransform.localScale.y) : totalCells;
        grid = new Vector3[totalCells];
        turretGrid = new bool[totalCells];
        Vector3 offset = new Vector3(gridWidth/2f,0,gridHeight/2f);
        
        for (int i = 0; i < totalCells; i++)
        {
            float2 cellCoord = GridUtils.GetXY2(i, gridWidth) + new float2(0.5f);
            grid[i] = new Vector3(cellCoord.x,0,cellCoord.y) - offset;
        }
    }

    private void CalculateFlowField()
    {
        NativeArray<bool> nativeObstacles = new (totalCells, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        nativeObstacles.CopyFrom(turretGrid);
        
        //Cost Field
        NativeArray<byte> nativeCostField = new (totalCells, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        JCostField jobCost = new JCostField
        {
            Obstacles = nativeObstacles,
            CostField = nativeCostField
        };
        JobHandle jobHandleCost = jobCost.ScheduleParallel(nativeCostField.Length, JobWorkerCount - 1, default);

        //Integration Field
        NativeArray<int> nativeBestCostField = new (totalCells, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        for (int i = 0; i < totalCells; i++) { nativeBestCostField[i] = ushort.MaxValue; }

        JIntegrationField integrationJob = new JIntegrationField
        {
            DestinationCellIndex = destinationIndex,
            NumCellX = gridWidth,
            NumCellY = gridHeight,
            CostField = nativeCostField,
            BestCostField = nativeBestCostField
        };
        JobHandle jHIntegrationField = integrationJob.Schedule(jobHandleCost);
        
        //Direction Field
        NativeArray<float3> nativeBestDirection = new (totalCells, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        JBestDirection directionFieldJob = new JBestDirection
        {
            NumCellX = gridWidth,
            NumCellY = gridHeight,
            BestCostField = nativeBestCostField,
            CellBestDirection = nativeBestDirection
        };
        JobHandle jHDirectionField = directionFieldJob.ScheduleParallel(totalCells, JobWorkerCount - 1, jHIntegrationField);
        jHDirectionField.Complete();
        nativeBestDirection.Reinterpret<Vector3>().CopyTo(flowField);
        
        debugIntegrationField = nativeBestCostField.ToArray();
        debugCostField = nativeCostField.ToArray();
        
        nativeObstacles.Dispose();
        nativeCostField.Dispose();
        nativeBestCostField.Dispose();
        nativeBestDirection.Dispose();
    }

    private void GetObstacles()
    {
        const float radius = 0.5f;
        for (int i = 0; i < totalCells; i++)
        {
            turretGrid[i] = Physics.CheckSphere(grid[i],radius,1 << 6);
        }
    }
    
    private void GetStartPositions()
    {
        spawnPoints = new List<Vector3>(6);
        Bounds bound = new Bounds(StartTransform.position, StartTransform.localScale);
        for (int i = 0; i < grid.Length; i++)
        {
            if (bound.Contains(grid[i]))
            {
                spawnPoints.Add(grid[i]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!GizmosDebug) return;
        if (grid == null || destinationIndex == -1 || startIndex == -1 || turretGrid == null) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < grid.Length; i++)
        {
            Gizmos.DrawWireSphere(grid[i], 0.3f);
            if (!turretGrid[i]) continue;
            Gizmos.DrawCube(grid[i], Vector3.one);
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(grid[destinationIndex] , 0.2f);
        Gizmos.DrawSphere(grid[startIndex] , 0.2f);

        DebugFlowField();
        DebugCostField();
        DebugIntegrationField();
        DebugShowSpawnsPoints();
    }
    
    private void DebugShowSpawnsPoints()
    {
        if (!SpawnsPointsDebug) return;
        if (spawnPoints == null) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Gizmos.DrawSphere(spawnPoints[i], 0.5f);
        }
    }

    private void DebugFlowField()
    {
        if(flowField == null || !FlowFieldDebug) return;
        for (int i = 0; i < grid.Length; i++)
        {
            DrawArrow.ForGizmo(grid[i],flowField[i]);
        }
    }

    private void DebugCostField()
    {
        if (debugCostField == null) return;
        if (!CostFieldDebug) return;
        
        for (int i = 0; i < grid.Length; i++)
        {
            int value = debugCostField[i];
            Handles.Label(grid[i], $"{value}");
        }
    }
    
    private void DebugIntegrationField()
    {
        if (debugIntegrationField == null) return;
        if (!IntegrationFieldDebug) return;
        
        for (int i = 0; i < grid.Length; i++)
        {
            int value = debugIntegrationField[i];
            Handles.Label(grid[i], $"{value}");
        }
    }
}
