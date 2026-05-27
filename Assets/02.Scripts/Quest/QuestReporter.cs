using System.Linq;
using UnityEngine;

public class QuestReporter : MonoBehaviour
{
    [SerializeField]
    private Category category;
    [SerializeField]
    private TaskTarget target;
    [SerializeField]
    private int successCount;

    [SerializeField]
    private string[] targetTags;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColliderToReport(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ColliderToReport(collision);
    }

    private void ColliderToReport(Component other)
    {
        if (targetTags.Any(x => other.CompareTag(x)))
            Report();
    }

    public void Report()
    {
        Managers.QuestMgr.QuestRecieveReport(category, target, successCount);
    }
}
