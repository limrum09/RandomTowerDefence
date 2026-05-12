using UnityEngine;

public class ObjectCheckPlayer<T> : MonoBehaviour where T : Component
{
    [SerializeField]
    private GitSpriteGlowCtr glowCtr;

    protected virtual void PlayerEnter() { }

    protected virtual void PlayerExit() { }
    protected virtual bool PlayerMouseClick()
    {
        if (Managers.InputData.IsPointerOverUI<TowerUIRaycastTarget>())
            return false;

        if (!Managers.InputData.TryGetMouseComponent(Camera.main, out T target))
            return false;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            glowCtr.ShowGlowEffect();
            PlayerEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            glowCtr.HideGlowEffect();
            PlayerExit();
        }
    }
}
