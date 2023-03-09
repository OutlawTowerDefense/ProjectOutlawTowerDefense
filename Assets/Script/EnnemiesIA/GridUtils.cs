using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
using float3 = Unity.Mathematics.float3;

public static class GridUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 GetXY2(int i, int gridWidth)
    {
        int y = i / gridWidth;
        int x = i - (y * gridWidth);
        return new int2(x, y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int,int) GetXY(int i, int gridWidth)
    {
        int y = i / gridWidth;
        int x = i - (y * gridWidth);
        return (x, y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndex(in int2 coord, int gridWidth)
    {
        return coord.y * gridWidth + coord.x;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFromPosition(in float2 pointPos, in int2 mapXY, int cellSize = 1)
    {
        float2 percents = pointPos / (mapXY * cellSize);
        percents = clamp(percents, 0, 1f);
        int2 xy =  clamp((int2)floor(mapXY * percents), 0, mapXY - 1);
        return mad(xy.y, mapXY.x/cellSize, xy.x);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFromPositionOffset(in float2 pointPos, in int2 mapXY, in int2 offset, int cellSize = 1)
    {
        float2 percents = (pointPos - offset) / (mapXY * cellSize);
        percents = clamp(percents, 0, 1f);
        int2 xy =  clamp((int2)floor(mapXY * percents), 0, mapXY - 1);
        return mad(xy.y, mapXY.x/cellSize, xy.x);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFromPosition(in float3 pointPos, in int2 mapXY, int cellSize = 1)
    {
        float2 percents = pointPos.xz / (mapXY * cellSize);
        percents = clamp(percents, 0, 1f);
        int2 xy =  clamp((int2)floor(mapXY * percents), 0, mapXY - 1);
        return mad(xy.y, mapXY.x/cellSize, xy.x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFromPosition(in Vector3 pointPos, in int2 mapXY, int cellSize = 1)
    {
        float2 pos2D = new float2(pointPos.x, pointPos.z);
        float2 percents = pos2D / (mapXY * cellSize);
        percents = clamp(percents, 0, 1f);
        int2 xy =  clamp((int2)floor(mapXY * percents), 0, mapXY - 1);
        return mad(xy.y, mapXY.x/cellSize, xy.x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 GetCellCenterFromPosition(in Vector3 positionInWorld, in int2 mapXY, int cellSize = 1)
    {
        float2 pos2D = new float2(positionInWorld.x, positionInWorld.z);
        int index = GetIndexFromPosition(pos2D, mapXY, cellSize);
        float2 cellCoord = GetXY2(index,mapXY.x/cellSize) * cellSize + float2(cellSize/2f);
        return new Vector3(cellCoord.x,0,cellCoord.y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 GetCellCenterFromIndex(int index, in int2 mapXY, int cellSize = 1)
    {
        float2 cellCoord = GetXY2(index, mapXY.x/cellSize) * cellSize + float2(cellSize/2f);
        return new Vector3(cellCoord.x,0,cellCoord.y);
    }
}
