#if UNITY_EDITOR
using UnityEngine;

public class StageSceneDebugger : MonoBehaviour
{
    private enum StageDebugDifficulty
    {
        EASY,
        NORMAL,
        HARD,
        HELL
    }

    [SerializeField]
    private StageManager stage;
    [SerializeField]
    private StageDebugDifficulty difficulty = StageDebugDifficulty.EASY;
    [SerializeField]
    private int waveNumber = 1;
    [SerializeField]
    private int gold = 10000;
    [SerializeField]
    private int life = 5;
    [SerializeField]
    private int obstacle = 5;

    public void ApplyDifficulty()
    {
        stage?.DebugSetDifficulty(difficulty.ToString());
    }

    public void ApplyWave()
    {
        stage?.DebugPrepareWave(difficulty.ToString(), waveNumber);
    }

    public void AddGold()
    {
        stage?.UsingGold(GoldChangedReason.GAIN, gold);
    }

    public void AddLife()
    {
        stage?.RunSession.HealLife(life);
    }

    public void AddFreeObstacle()
    {
        stage?.RunSession.GetFreeObstacle(obstacle);
    }
}
#endif
