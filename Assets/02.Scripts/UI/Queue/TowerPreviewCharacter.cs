using UnityEngine;
using UnityEngine.U2D.Animation;

public class TowerPreviewCharacter : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private SpriteLibrary spriteLibrary;

    public Sprite TowerSprite => spriteLibrary.GetSprite("Block", "0");

    public void SetTower(string uid)
    {
        TowerData temp = Managers.TowerData.GetTowerData(uid);

        string iconPath = temp.iconPath;
        int grade = temp.grade;
        SpriteLibraryAsset library = ResourceCache.Load<SpriteLibraryAsset>($"Tower/SpriteLibrary/{iconPath}/{iconPath}_{grade}");

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
