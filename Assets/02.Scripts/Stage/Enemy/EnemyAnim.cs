using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// 적의 이동이나 사망 등 애니메이션을 담당하는 클래스
/// 적 애니메이션 및 SpriteLibrary 설정
/// 타워 기본 이동 및 사망 애니메이션 제어
/// </summary>
public class EnemyAnim : MonoBehaviour
{
    // SpriteLibrary를 참조하여 일반 걷기가 4개의 Sprite로 구성되었다면 사용할 컨트롤러
    [SerializeField]
    private RuntimeAnimatorController run4Ctr;
    // SpriteLibrary를 참조하여 일반 걷기가 6개의 Sprite로 구성되었다면 사용할 컨트롤러
    [SerializeField]
    private RuntimeAnimatorController run6Ctr;
    [SerializeField]
    private Animator anim;                      // 적 애니메이션 제어룔 
    [SerializeField]
    private SpriteLibrary spriteLibrary;        // 적 외형 교체용 SpriteLibrary
    [SerializeField]
    private Sprite baseSprite;                  // 기본 Sprite

    /// <summary>
    /// 애니메이션 적용
    /// 받은 UID로 찾은 EnemyData의 값을 이용하여 애니메이션 적용
    /// SpriteLibrary의 "Run" 레이블의 개수를 확인하여 각기 다른 컨트롤러를 사용
    /// </summary>
    /// <param name="uid">스폰할 적 UID</param>
    public void SetAnim(string uid)
    {
        // UID를 사용하여 적의 정보를 가져옴
        EnemyData data = Managers.EnemyData.GetEnemyData(uid);

        // 데이터를 가져오지 못한다면 올바르지 않는 적이이게 제거
        if(data == null)
        {
            Destroy(this.gameObject);
            return;
        }

        // SpriteLibrary 가져오기
         SpriteLibraryAsset loadLibrary = Resources.Load<SpriteLibraryAsset>($"Enemy/SpriteLibrary/{data.enemyUID}");

        if (spriteLibrary == null)
            Debug.LogWarning("Enemy의 Sprite Library를 가져오지 못함");

        spriteLibrary.spriteLibraryAsset = loadLibrary;

        if (anim == null)
            return;

        // 'Run'이라는 Label의 개수가 6개라면 run6Ctr을 사용하고, 아니라면 run4Ctr을 사용
        if (loadLibrary.GetCategoryLabelNames("Run").Count() == 6)
            anim.runtimeAnimatorController = run6Ctr;
        else
            anim.runtimeAnimatorController = run4Ctr;
    }

    /// <summary>
    /// 적 사망 처리시 호출
    /// </summary>
    public void Die()
    {
        if (anim == null)
            return;

        // 애니메이션 상태 젼경
        anim.SetBool("IsDie", true);
    }
}
