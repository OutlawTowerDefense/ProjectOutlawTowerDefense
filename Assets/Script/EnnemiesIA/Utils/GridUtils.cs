using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
using float3 = Unity.Mathematics.float3;

[Flags]
public enum AdjacentCell : int
{
    Top         = 1 << 0,
    Right       = 1 << 1,
    Left        = 1 << 2,
    Bottom      = 1 << 3,
    TopLeft     = 1 << 4,
    TopRight    = 1 << 5,
    BottomRight = 1 << 6,
    BottomLeft  = 1 << 7,
}

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
    public static int GetIndexFromPositionOffset(in float2 pointPos, in int2 mapXY, int cellSize = 1)
    {
        float2 offset = mapXY / float2(2f);
        float2 percents = (pointPos + offset) / (mapXY * cellSize);
        percents = clamp(percents, 0, 1f);
        int2 xy =  clamp((int2)floor(mapXY * percents), 0, mapXY - 1);
        return mad(xy.y, mapXY.x/cellSize, xy.x);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFromPositionOffset(in Vector3 pointPos, int2 mapXY, int cellSize = 1)
    {
        float2 pos2D = new float2(pointPos.x, pointPos.z);
        float2 offset = mapXY / float2(2f);
        float2 percents = (pos2D + offset) / (mapXY * cellSize);
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AdjCellFromIndex(int index, AdjacentCell adjCell, in int2 pos, int width) 
    => adjCell switch
    {
        AdjacentCell.Left        when pos.x > 0                              => index - 1,
        AdjacentCell.Right       when pos.x < width - 1                      => index + 1,
        AdjacentCell.Top         when pos.y < width - 1                      => index + width,
        AdjacentCell.TopLeft     when pos.y < width - 1 && pos.x > 0         => (index + width) - 1,
        AdjacentCell.TopRight    when pos.y < width - 1 && pos.x < width - 1 => (index + width) + 1,
        AdjacentCell.Bottom      when pos.y > 0                              => index - width,
        AdjacentCell.BottomLeft  when pos.y > 0 && pos.x > 0                 => (index - width) - 1,
        AdjacentCell.BottomRight when pos.y > 0 && pos.x < width - 1         => (index - width) + 1,
        _ => -1,
    };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AdjCellFromIndex(int index, int adjCell, in int2 pos, int width) 
    => adjCell switch
    {
        (int)AdjacentCell.Left        when pos.x > 0                              => index - 1,
        (int)AdjacentCell.Right       when pos.x < width - 1                      => index + 1,
        (int)AdjacentCell.Top         when pos.y < width - 1                      => index + width,
        (int)AdjacentCell.TopLeft     when pos.y < width - 1 && pos.x > 0         => (index + width) - 1,
        (int)AdjacentCell.TopRight    when pos.y < width - 1 && pos.x < width - 1 => (index + width) + 1,
        (int)AdjacentCell.Bottom      when pos.y > 0                              => index - width,
        (int)AdjacentCell.BottomLeft  when pos.y > 0 && pos.x > 0                 => (index - width) - 1,
        (int)AdjacentCell.BottomRight when pos.y > 0 && pos.x < width - 1         => (index - width) + 1,
        _ => -1,
    };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AdjCellFromIndex(int index, int adjCell, in int2 pos, int width, int height) 
        => adjCell switch
        {
            (int)AdjacentCell.Left        when pos.x > 0                                => index - 1,
            (int)AdjacentCell.Right       when pos.x < width - 1                        => index + 1,
            (int)AdjacentCell.Top         when pos.y < height - 1                       => index + width,
            (int)AdjacentCell.TopLeft     when pos.y < height - 1 && pos.x > 0          => (index + width) - 1,
            (int)AdjacentCell.TopRight    when pos.y < height - 1 && pos.x < width - 1  => (index + width) + 1,
            (int)AdjacentCell.Bottom      when pos.y > 0                                => index - width,
            (int)AdjacentCell.BottomLeft  when pos.y > 0 && pos.x > 0                   => (index - width) - 1,
            (int)AdjacentCell.BottomRight when pos.y > 0 && pos.x < width - 1           => (index - width) + 1,
            _ => -1,
        };
}
