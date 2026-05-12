using NUnit.Framework;
using SpriteGlow;
using UnityEngine;

public class GitSpriteGlowCtr : MonoBehaviour
{
    [SerializeField]
    private SpriteGlowEffect glow;


    private void Start()
    {
        HideGlowEffect();
    }

    public void ShowGlowEffect()
    {
        glow.enabled = true;
    }

    public void HideGlowEffect()
    {
        glow.enabled = false;
    }
}
