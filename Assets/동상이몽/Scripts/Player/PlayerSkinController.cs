using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// 아이템을 먹었을 때 플레이어 스킨(Animator + Variables)을 해당 스킨으로 바꿉니다.
/// 불닭 아이템 → 붉닭 스킨, 치킨 아이템 → 치킨 스킨, 삼계탕 아이템 → 삼계탕 스킨
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerSkinController : MonoBehaviour
{
    private Animator animator;
    private string lastSyncedAbility = "";
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Variables에 저장된 "활성화된 능력"을 Animator에 반영. 그래프가 덮어써도 매 프레임 우리 값으로 복구.
    /// </summary>
    void LateUpdate()
    {
        if (animator == null || !animator.enabled) return;
        string ability = GetActiveAbilityFromVariables();
        if (ability == lastSyncedAbility) return;
        lastSyncedAbility = ability;
        SyncAnimatorToAbility(ability);
        PlaySkinState(ability); // 스킨 간 트랜지션이 없어서 상태 직접 재생
    }
    
    string GetActiveAbilityFromVariables()
    {
        try
        {
            var v = Variables.Object(gameObject).Get("활성화된 능력");
            if (v is string s && !string.IsNullOrEmpty(s)) return s;
        }
        catch { }
        try
        {
            if (Variables.Object(gameObject).Get<bool>("불닭")) return "불닭";
            if (Variables.Object(gameObject).Get<bool>("치킨")) return "치킨";
            if (Variables.Object(gameObject).Get<bool>("삼계탕")) return "삼계탕";
        }
        catch { }
        return "기본";
    }
    
    void SyncAnimatorToAbility(string ability)
    {
        animator.SetBool("Is불닭", ability == "불닭");
        animator.SetBool("Is치킨", ability == "치킨");
        animator.SetBool("Is삼계탕", ability == "삼계탕");
    }
    
    /// <summary>
    /// 컨트롤러에는 "기본 정지"에서만 각 스킨으로 가는 트랜지션만 있어서,
    /// 스킨 상태(불닭/치킨/삼계탕)에서 다른 스킨으로는 전환이 안 됨.
    /// 그래서 스킨 변경 시 Play()로 해당 상태를 직접 재생.
    /// </summary>
    void PlaySkinState(string skinName, int layerIndex = 0)
    {
        bool isMoving = animator.GetBool("기본 이동");
        string stateName = GetStateNameForSkin(skinName, isMoving);
        animator.Play(stateName, layerIndex);
    }
    
    static string GetStateNameForSkin(string skinName, bool isMoving)
    {
        string motion = isMoving ? "이동 모션" : "정지 모션";
        if (skinName == "불닭") return "불닭 " + motion;
        if (skinName == "치킨") return "치킨 " + motion;
        if (skinName == "삼계탕") return "삼계탕 " + motion;
        return "기본 " + motion;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || other.gameObject == null) return;
        
        string tag = other.gameObject.tag;
        // 부모에서 태그 찾기 (자식 콜라이더인 경우)
        if (string.IsNullOrEmpty(tag) && other.transform.parent != null)
            tag = other.transform.parent.gameObject.tag;
        
        if (tag == "불닭") { ApplySkin("불닭"); return; }
        if (tag == "치킨") { ApplySkin("치킨"); return; }
        if (tag == "삼계탕") { ApplySkin("삼계탕"); return; }
    }
    
    /// <summary>
    /// 스킨 적용: Animator 파라미터와 Variables 동기화
    /// </summary>
    public void ApplySkin(string skinName)
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) return;
        
        // 1) Variables 먼저 설정 (SkillEffectManager·LateUpdate 등이 이걸 기준으로 함)
        try
        {
            Variables.Object(gameObject).Set("활성화된 능력", skinName);
            Variables.Object(gameObject).Set("기본", skinName == "기본" || string.IsNullOrEmpty(skinName));
            Variables.Object(gameObject).Set("불닭", skinName == "불닭");
            Variables.Object(gameObject).Set("치킨", skinName == "치킨");
            Variables.Object(gameObject).Set("삼계탕", skinName == "삼계탕");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"PlayerSkinController: Variables 설정 실패 - {e.Message}");
        }
        
        // 2) 파라미터 설정 + 스킨 상태 간 전환이 없으므로 Play()로 해당 상태 직접 재생
        lastSyncedAbility = skinName;
        SyncAnimatorToAbility(skinName);
        PlaySkinState(skinName);
        
        Debug.Log($"[PlayerSkinController] 스킨 적용: {skinName}");
    }
}
