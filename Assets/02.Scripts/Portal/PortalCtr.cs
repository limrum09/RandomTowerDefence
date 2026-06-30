using UnityEngine;

public class PortalCtr : MonoBehaviour
{
    [SerializeField]
    private Transform goalPos;
    [SerializeField]
    private SpriteRenderer render;

    public void SetDir()
    {
        Transform pos = this.transform;
        float dirX = goalPos.position.x - pos.position.x;

        if (Mathf.Abs(dirX) < 0.01f)
            return;

        render.flipX = dirX < 0f;
    }
}
