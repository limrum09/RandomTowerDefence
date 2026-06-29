using UnityEngine;
using UnityEngine.U2D.Animation;

public class TowerAttackPreview : MonoBehaviour
{
    [Header("Preview Settings")]
    [SerializeField]
    private TowerType towerType;
    [SerializeField]
    [Min(1)]
    private int grade = 1;
    [SerializeField]
    [Min(0.01f)]
    private float attackSpeed = 1f;

    [Header("References")]
    [SerializeField]
    private Animator towerAnimator;
    [SerializeField]
    private SpriteLibrary spriteLibrary;
    [SerializeField]
    private Transform enemyTarget;
    [SerializeField]
    private Transform effectRoot;

    private GameObject previewEffect;
    private Animator effectAnimator;
    private float effectElapsedTime;
    private float effectDuration;

    public float AttackSpeed => Mathf.Max(attackSpeed, 0.01f);

    private float GetAttackType()
    {
        if (towerType == TowerType.Elf && grade >= 3)
            return 1f;

        if (towerType == TowerType.Dragonian)
            return 2f;

        return 0f;
    }

    public void ApplyTower()
    {
        TowerDataManager dataManager = new TowerDataManager();
        dataManager.Init();

        TowerData data = dataManager.GetTowerData(towerType, grade);

        if (data == null)
        {
            Debug.LogWarning($"Tower preview data missing: {towerType}, grade {grade}");
            return;
        }

        string path = $"Tower/SpriteLibrary/{data.iconPath}/{data.iconPath}_{data.grade}";

        SpriteLibraryAsset library =
            ResourceCache.Load<SpriteLibraryAsset>(path);

        if (library == null)
        {
            Debug.LogWarning($"SpriteLibrary missing: {path}");
            return;
        }

        spriteLibrary.spriteLibraryAsset = library;

        towerAnimator.Rebind();
        towerAnimator.Update(0f);

        towerAnimator.SetFloat(
            "AttackType",
            GetAttackType());

        towerAnimator.SetFloat(
            "AttackSpeed",
            Mathf.Max(attackSpeed, 0.01f));
    }

    public void PlayAttack()
    {
        if (towerAnimator == null)
            return;

        ClearAttackEffect();

        towerAnimator.SetFloat("AttackType", GetAttackType());
        towerAnimator.SetFloat("AttackSpeed", AttackSpeed);
        towerAnimator.ResetTrigger("IsAttack");
        towerAnimator.SetTrigger("IsAttack");
        towerAnimator.Update(0f);

        CreateAttackEffect();
    }

    public void UpdatePreview(float deltaTime)
    {
        if (towerAnimator == null)
            return;

        towerAnimator.Update(deltaTime);

        if (effectAnimator == null)
            return;

        effectAnimator.Update(deltaTime);
        effectElapsedTime += deltaTime;

        if (effectElapsedTime >= effectDuration)
            ClearAttackEffect();
    }

    public void StopPreview()
    {
        ClearAttackEffect();

        if (towerAnimator == null)
            return;

        towerAnimator.Rebind();
        towerAnimator.SetFloat("AttackType", GetAttackType());
        towerAnimator.SetFloat("AttackSpeed", AttackSpeed);
        towerAnimator.Update(0f);
    }

    private void CreateAttackEffect()
    {
        if (enemyTarget == null || effectRoot == null)
        {
            Debug.LogWarning("EnemyTarget or EffectRoot is not assigned.");
            return;
        }

        string effectName = GetAttackEffectName();
        GameObject effectPrefab =
            ResourceCache.Load<GameObject>($"Effects/{effectName}");

        if (effectPrefab == null)
        {
            Debug.LogWarning($"AttackEffect missing: Effects/{effectName}");
            return;
        }

        previewEffect = Instantiate(
            effectPrefab,
            enemyTarget.position,
            Quaternion.identity,
            effectRoot);

        previewEffect.name = $"{effectName}_Preview";
        previewEffect.hideFlags = HideFlags.DontSaveInEditor;

        AnimatorEffectAutoReturn autoReturn =
            previewEffect.GetComponent<AnimatorEffectAutoReturn>();

        if (autoReturn != null)
            DestroyImmediate(autoReturn);

        effectAnimator = previewEffect.GetComponentInChildren<Animator>();

        if (effectAnimator == null)
        {
            Debug.LogWarning($"Animator missing: {effectName}");
            ClearAttackEffect();
            return;
        }

        effectAnimator.Rebind();
        effectAnimator.speed = AttackSpeed;
        effectAnimator.Update(0f);

        AnimationClip[] clips =
            effectAnimator.runtimeAnimatorController.animationClips;

        effectDuration = clips.Length > 0
            ? clips[0].length / AttackSpeed
            : 1f / AttackSpeed;

        effectElapsedTime = 0f;

        bool isFacing = enemyTarget.position.x < transform.position.x;
        SpriteRenderer towerSprite =
            spriteLibrary.GetComponent<SpriteRenderer>();
        SpriteRenderer effectSprite =
            previewEffect.GetComponentInChildren<SpriteRenderer>();

        if (towerSprite != null)
            towerSprite.flipX = isFacing;

        if (effectSprite != null)
            effectSprite.flipX = isFacing;
    }

    private string GetAttackEffectName()
    {
        switch (towerType)
        {
            case TowerType.Dragonian:
                return "FireBallAttackEffect";
            case TowerType.Elf:
                return grade >= 3
                    ? "ArrowAttackEffect"
                    : "KnifeAttackEffect";
            case TowerType.Human:
                return "HumanAttackEffect";
            case TowerType.Dwarf:
                return "DwarfAttackEffect";
            case TowerType.Orc:
                return "OrcAttackEffect";
            case TowerType.Werebeast:
                return "WerebeastAttackEffect";
            default:
                return string.Empty;
        }
    }

    private void ClearAttackEffect()
    {
        effectAnimator = null;
        effectElapsedTime = 0f;
        effectDuration = 0f;

        if (previewEffect == null)
            return;

        if (Application.isPlaying)
            Destroy(previewEffect);
        else
            DestroyImmediate(previewEffect);

        previewEffect = null;
    }
}
