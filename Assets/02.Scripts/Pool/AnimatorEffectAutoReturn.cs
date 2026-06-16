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
        Debug.Log("호출");
        Managers.Pool.Push(gameObject);
    }
}
