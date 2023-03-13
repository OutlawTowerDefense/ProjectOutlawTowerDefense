using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private int2 turretGridBounds;
    public bool[] TurretGrid{ get; private set; }
    
    private int2 iaGridBounds;
    public bool[] IAGrid{ get; private set; }

    public event Action OnNewTurret;
    
    private void Awake()
    {
        Instance = Instance == null ? this : Instance;
    }

    private void OnDestroy()
    {
        ClearAllSubscriber();
    }
    
    // Clear all subscriber
    private void ClearAllSubscriber()
    {
        if (OnNewTurret == null) return;
        foreach (Delegate action in OnNewTurret.GetInvocationList())
        {
            OnNewTurret -= (Action)action;
        }
    }

    // ============================================================================
    // Turret Grid
    
    public void InitTurretGrid(int width, int height)
    {
        turretGridBounds = new int2(width,height);
        TurretGrid = new bool[width * height];
    }

    bool IsGridInit()
    {
        Debug.LogError("YOU DONKEY, INIT THE GRID BEFORE USING IT!");
        return TurretGrid != null;
    }

    public bool AddTurret(int x, int y)
    {
        if (IsGridInit()) return false;
        if (TurretGrid[turretGridBounds.x * y + x]) return false;
        TurretGrid[turretGridBounds.x * y + x] = true;
        OnNewTurret?.Invoke();
        return true;
    }
    
    public bool RemoveTurret(int x, int y)
    {
        if (IsGridInit()) return false;
        if (!TurretGrid[turretGridBounds.x * y + x]) return false;
        TurretGrid[turretGridBounds.x * y + x] = false;
        OnNewTurret?.Invoke();
        return true;
    }
    
    // ============================================================================
    
    // ============================================================================
    // Turret Grid
    
    public void InitIAGrid(int width, int height)
    {
        iaGridBounds = new int2(width,height);
        IAGrid = new bool[width * height];
    }
    
    bool IsIAGridInit()
    {
        Debug.LogError("YOU DONKEY, INIT THE GRID BEFORE USING IT!");
        return IAGrid != null;
    }

    public bool UpdateIAGrid(bool[] newGrid)
    {
        if (IAGrid == null)
        {
            Debug.LogError("YOU DONKEY, INIT THE GRID BEFORE USING IT!");
            return false;
        }
        if (newGrid.Length != IAGrid.Length) return false;
        newGrid.CopyTo(IAGrid.AsSpan());
        return true;
    }
    // ============================================================================
}
