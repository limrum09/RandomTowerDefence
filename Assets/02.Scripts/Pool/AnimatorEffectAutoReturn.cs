using UnityEngine;

public class AnimatorEffectAutoReturn : MonoBehaviour
{
    [SerializeField]
    private Animator effect;

    private void OnEnable()
    {
        effect.Rebind();
        effect.Update(0f);
    }

    public void AnimationEnd()
    {
        Managers.Pool.Push(gameObject);
    }
}
