using System.Collections.Generic;

public class PublicMetaSaveData
{
    public MetaUpgradeType type;
    public int level;
}

public class PublicMetaUpgradeData
{
    public List<PublicMetaSaveData> upgrades = new List<PublicMetaSaveData>();
}

public class PublicMetaUpgradeManager
{
    private PublicMetaUpgradeData upgradeData = new PublicMetaUpgradeData();

    private PublicMetaSaveData GetMetaSaveData(MetaUpgradeType getType)
    {
        PublicMetaSaveData data = upgradeData.upgrades.Find(x => x.type == getType);

        if(data == null)
        {
            data = new PublicMetaSaveData
            {
                type = getType,
                level = 0
            };

            upgradeData.upgrades.Add(data);
        }

        return data;
    }

    public bool PublicMetaUpgrade(MetaUpgradeType getType, int upValue)
    {
        PublicMetaSaveData data = GetMetaSaveData(getType);
        data.level += upValue;

        return true;
    }

    public bool GetPublicMetaType(string value, out MetaUpgradeType type)
    {
        return System.Enum.TryParse(value, true, out type);
    }

    public int GetPublicMetaDataLevel(MetaUpgradeType getType)
    {
        return GetMetaSaveData(getType).level;
    }

    public string GetTypeName(MetaUpgradeType getType)
    {
        switch (getType)
        {
            case MetaUpgradeType.StartingGold:
                return "시작 시 골드 획득량";
            case MetaUpgradeType.FreeObstacle:
                return "무료 장애물 설치";
            case MetaUpgradeType.FreeTerrainReroll:
                return "지형 재설정";
            case MetaUpgradeType.DropGold:
                return "몬스터 드랍 골드";
            default:
                return "찾을 수 없음";
        }
    }

    public string GetTypeInfoStr(MetaUpgradeType getType)
    {
        switch (getType)
        {
            case MetaUpgradeType.StartingGold:
                return "스테이지 시작 시 획득하는 골드 증가";
            case MetaUpgradeType.FreeObstacle:
                return "무료 장애물 설치권 개수 증가";
            case MetaUpgradeType.FreeTerrainReroll:
                return "지형 재설정 횟수 증가";
            case MetaUpgradeType.DropGold:
                return "몬스터가 드랍하는 골드의 개수 증가";
            default:
                return "찾을 수 없음";
        }
    }

    public string GetTypeCountStr(MetaUpgradeType getType)
    {
        switch (getType)
        {
            case MetaUpgradeType.StartingGold:
                return "획득 골드";
            case MetaUpgradeType.FreeObstacle:
                return "무료 개수";
            case MetaUpgradeType.FreeTerrainReroll:
                return "재설정횟수";
            case MetaUpgradeType.DropGold:
                return "드랍 골드";
            default:
                return "찾을 수 없음";
        }
    }
}
