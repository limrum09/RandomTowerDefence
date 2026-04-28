using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField]
    private RuntimeAnimatorController run4Ctr;
    [SerializeField]
    private RuntimeAnimatorController run6Ctr;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private SpriteLibrary spriteLibrary;
    [SerializeField]
    private Sprite baseSprite;

    public void SetAnim(string uid)
    {
        EnemyData data = Managers.EnemyData.GetEnemyData(uid);

         SpriteLibraryAsset loadLibrary = Resources.Load<SpriteLibraryAsset>($"Enemy/SpriteLibrary/{data.enemyUID}");

        if (spriteLibrary == null)
            Debug.LogWarning("Enemy의 Sprite Library를 가져오지 못함");

        spriteLibrary.spriteLibraryAsset = loadLibrary;

        if (anim == null)
            return;

        if (loadLibrary.GetCategoryLabelNames("Run").Count() == 6)
            anim.runtimeAnimatorController = run6Ctr;
        else
            anim.runtimeAnimatorController = run4Ctr;
    }

    public void Die()
    {
        if (anim == null)
            return;

        anim.SetBool("IsDie", true);
    }
}
