using System.Net.NetworkInformation;
using UnityEngine;

public class ObstacleInfo : MonoBehaviour
{
    private int cost;

    public int Cost => cost;
    public void GetCost(int cost)
    {
        this.cost = cost;
    }
}
