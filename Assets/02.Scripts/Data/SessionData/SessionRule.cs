using System.Collections.Generic;
using UnityEngine;

public class SessionRule
{
    private readonly Dictionary<int, int> needExpToLevel = new Dictionary<int, int>()
    {
        { 1, 3 },
        { 2, 6 },
        { 3, 9 },
        { 4, 12 },
        { 5, 15 },
        { 6, 18 },
        { 7, 21 },
        { 8, 24 },
        { 9, 27 },
        { 10, 30 },
        { 11, 33 },
        { 12, 36 },
    };

    private readonly Dictionary<int, int> limitTowerCountToLevel = new Dictionary<int, int>()
    {
        { 1, 3 },
        { 2, 5 },
        { 3, 7 },
        { 4, 9 },
        { 5, 11 },
        { 6, 13 },
        { 7, 15 },
        { 8, 17 },
        { 9, 20 },
        { 10, 22 },
        { 11, 24 },
        { 12, 26 },
        { 13, 30 }
    };

    public int GetNeedEXP(int level)
    {
        if (needExpToLevel.TryGetValue(level, out int needExp))
        {
            return needExp;
        }

        return 999999;
    }

    public int limitTowerCnt(int level)
    {
        if (level <= 0)
            return 0;

        if (level >= 13)
            return 30;

        if(limitTowerCountToLevel.TryGetValue(level,out int towerLimit))
            return towerLimit;

        return 3;
    }
}