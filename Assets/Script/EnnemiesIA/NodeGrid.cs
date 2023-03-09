using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

using static GridUtils;

public readonly struct Node
{
    public readonly int CameFromNodeIndex;
    public readonly int GCost; //Distance from Start Node
    public readonly int HCost; // distance from End Node
    public readonly int FCost;
    public readonly int2 Coord;

    public Node(int cameFromNodeIndex, int gCost, int hCost, in int2 coord)
    {
        CameFromNodeIndex = cameFromNodeIndex;
        GCost = gCost;
        HCost = hCost;
        FCost = GCost + HCost;
        Coord = coord;
    }

    public Node(in int2 coord)
    {
        CameFromNodeIndex = -1;
        GCost = int.MaxValue;
        HCost = default;
        FCost = GCost + HCost;
        Coord = coord;
    }
}

public class NodeGrid : MonoBehaviour
{
    [SerializeField] private int CellSize;
    [SerializeField] private int2 MapXY;
    [SerializeField] private int2 NumCellXY;
    private Node[] GridArray;
    
    // Start is called before the first frame update
    private void Awake()
    {
        CellSize = math.max(1, CellSize);
        MapXY = math.ceilpow2(MapXY);
        NumCellXY = MapXY >> math.floorlog2(CellSize);
        GridArray = new Node[NumCellXY.x * NumCellXY.y];
        
        for (int i = 0; i < GridArray.Length; i++)
        {
            GridArray[i] = new Node(GetXY2(i,MapXY.x));
        }
    }

    //==============================================================================================================
    //CELLS INFORMATION
    //==============================================================================================================

    public Vector3 GetCellCenter(int index)
    {
        float2 cellCoord = GetXY2(index,NumCellXY.x) * CellSize + new float2(CellSize/2f);
        return new Vector3(cellCoord.x,0,cellCoord.y);
    }
    
    //==============================================================================================================
    //ARRAY MANIPULATION
    //==============================================================================================================
    
    public void CopyFrom(Node[] otherArray)
    {
        otherArray.CopyTo((Span<Node>) GridArray);
    }
    
    public Node this[int cellIndex]
    {
        get => GridArray[cellIndex];
        set => SetValue(cellIndex, value);
    }
    
    public Node GetValue(int index)
    {
        return GridArray[index];
    }

    public void SetValue(int index, Node value)
    {
        GridArray[index] = value;
    }
    
    //Operation from World Position
    //==============================================================================================================
    public int IndexFromPosition(in Vector3 position)
    {
        float2 pos2D = new float2(position.x, position.z);
        return GetIndexFromPosition(pos2D, MapXY, CellSize);
    }
}
