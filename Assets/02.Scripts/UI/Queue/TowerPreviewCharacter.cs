using UnityEngine;
using UnityEngine.U2D.Animation;

public class TowerPreviewCharacter : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private SpriteLibrary spriteLibrary;

    public void SetTower(string uid)
    {
        TowerData temp = Managers.TowerData.GetTowerData(uid);

        string iconPath = temp.iconPath;
        int grade = temp.grade;
        SpriteLibraryAsset library = Resources.Load<SpriteLibraryAsset>($"Tower/SpriteLibrary/{iconPath}/{iconPath}_{grade}");

        if (spriteLibrary == null)
        {
            Debug.LogWarning("Sprite Library 로드 실패 : ");
            return;
        }

        if (library == null)
        {
            Debug.LogWarning("Library 로드 실패 : ");
            return;
        }

        spriteLibrary.spriteLibraryAsset = library;

        anim.SetBool("IsAttack", false);
        anim.SetBool("IsBow", false);
        anim.SetBool("IsMagic", false);
    }

    public void SetShow()
    {
        this.gameObject.SetActive(true);
    }

    public void SetHide()
    {
        this.gameObject.SetActive(false);
    }
}
