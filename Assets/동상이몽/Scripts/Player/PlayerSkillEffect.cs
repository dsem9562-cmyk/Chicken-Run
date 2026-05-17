using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// 현재 능력에 따라 적절한 스킬 효과만 표시하도록 관리하는 스크립트
/// 기존 Visual Scripting 그래프의 Variables를 활용합니다.
/// 기본: 날개
/// 불닭: 매운 기운
/// 삼계탕: 삼계탕 방울
/// 치킨: 치킨 가루
/// </summary>
public class SkillEffectManager : MonoBehaviour
{
    [Header("스킬 효과 프리팹 (자동으로 찾을 수도 있습니다)")]
    [SerializeField] private GameObject 날개Prefab;
    [SerializeField] private GameObject 매운기운Prefab;
    [SerializeField] private GameObject 삼계탕방울Prefab;
    [SerializeField] private GameObject 치킨가루Prefab;
    
    private GameObject currentSkillEffect;
    private Variables variables;
    private string lastAbility = "";
    
    void Start()
    {
        // Variables 컴포넌트 찾기
        variables = GetComponent<Variables>();
        if (variables == null)
        {
            Debug.LogError("SkillEffectManager: Variables 컴포넌트를 찾을 수 없습니다!");
            enabled = false;
            return;
        }
        
        // 프리팹은 Inspector에서 할당하거나, GUID로 직접 참조할 수 있습니다
        // Inspector에서 할당하는 것을 권장합니다
    }
    
    void Update()
    {
        if (variables == null) return;
        
        string 활성화된능력 = GetActiveAbility();
        if (활성화된능력 != lastAbility)
        {
            lastAbility = 활성화된능력;
            UpdateSkillEffect(활성화된능력);
        }
    }
    
    /// <summary>
    /// 그래프가 생성한 다른 스킬 효과를 제거하고, 현재 능력에 맞는 스킬만 남깁니다.
    /// (Z 키로 스킬이 켜진 뒤, 같은 프레임/다음 프레임에 그래프가 만든 날개 등이 보이지 않도록 함)
    /// </summary>
    void LateUpdate()
    {
        if (variables == null) return;
        
        string 활성화된능력 = GetActiveAbility();
        var skillTransforms = new List<Transform>();
        CollectSkillEffectsRecursive(transform, skillTransforms);
        
        bool hasWrongEffect = false;
        bool hasCorrectEffect = false;
        foreach (Transform t in skillTransforms)
        {
            if (t == null) continue;
            bool isCorrect = IsCorrectSkillForAbility(t.name, 활성화된능력);
            if (!isCorrect) hasWrongEffect = true;
            if (isCorrect) hasCorrectEffect = true;
        }
        
        // 스킬 효과가 있으면: 잘못된 게 있거나, 올바른 게 없으면 → 전부 제거 후 현재 능력에 맞는 것만 생성
        if (skillTransforms.Count > 0 && (hasWrongEffect || !hasCorrectEffect))
        {
            RemoveExistingSkillEffects();
            CreateSkillForAbility(활성화된능력);
        }
    }
    
    /// <summary>
    /// 자식 이름이 주어진 능력에 맞는 스킬인지 여부
    /// </summary>
    private bool IsCorrectSkillForAbility(string childName, string ability)
    {
        if (string.IsNullOrEmpty(childName)) return false;
        if (string.IsNullOrEmpty(ability) || ability == "기본") return childName.Contains("날개");
        if (ability == "불닭") return childName.Contains("매운 기운") || childName.Contains("매운기운");
        if (ability == "삼계탕") return childName.Contains("삼계탕 방울") || childName.Contains("삼계탕방울");
        if (ability == "치킨") return childName.Contains("치킨 가루") || childName.Contains("치킨가루");
        return false;
    }
    
    /// <summary>
    /// 현재 능력에 맞는 스킬 효과만 생성 (RemoveExistingSkillEffects 호출 후 사용)
    /// </summary>
    private void CreateSkillForAbility(string 활성화된능력)
    {
        GameObject skillPrefab = null;
        if (string.IsNullOrEmpty(활성화된능력) || 활성화된능력 == "기본")
            skillPrefab = 날개Prefab;
        else if (활성화된능력 == "불닭")
            skillPrefab = 매운기운Prefab;
        else if (활성화된능력 == "삼계탕")
            skillPrefab = 삼계탕방울Prefab;
        else if (활성화된능력 == "치킨")
            skillPrefab = 치킨가루Prefab;
        
        if (skillPrefab != null)
        {
            currentSkillEffect = Instantiate(skillPrefab, transform);
            currentSkillEffect.transform.localPosition = Vector3.zero;
            currentSkillEffect.transform.localRotation = Quaternion.identity;
        }
    }
    
    /// <summary>
    /// Variables에서 현재 활성화된 능력 가져오기
    /// </summary>
    private string GetActiveAbility()
    {
        try
        {
            // Variables.Object API 사용 (가장 안정적인 방법)
            try
            {
                var 활성화된능력 = Variables.Object(gameObject).Get("활성화된 능력");
                if (활성화된능력 != null)
                {
                    string ability = 활성화된능력 as string;
                    if (!string.IsNullOrEmpty(ability))
                        return ability;
                }
            }
            catch (System.Exception e1)
            {
                Debug.LogWarning($"SkillEffectManager: 활성화된 능력 변수 읽기 실패 - {e1.Message}");
            }
            
            // Boolean 변수들로 확인 (기본, 불닭, 치킨, 삼계탕)
            try
            {
                if (Variables.Object(gameObject).Get<bool>("불닭"))
                    return "불닭";
                if (Variables.Object(gameObject).Get<bool>("치킨"))
                    return "치킨";
                if (Variables.Object(gameObject).Get<bool>("삼계탕"))
                    return "삼계탕";
                if (Variables.Object(gameObject).Get<bool>("기본"))
                    return "기본";
            }
            catch (System.Exception e2)
            {
                Debug.LogWarning($"SkillEffectManager: Boolean 변수 읽기 실패 - {e2.Message}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"SkillEffectManager: 능력 변수 읽기 실패 - {e.Message}");
        }
        
        return "기본"; // 기본값
    }
    
    /// <summary>
    /// 현재 능력에 따라 스킬 효과를 업데이트
    /// </summary>
    public void UpdateSkillEffect(string 활성화된능력 = null)
    {
        if (활성화된능력 == null)
            활성화된능력 = GetActiveAbility();
        
        RemoveExistingSkillEffects();
        CreateSkillForAbility(활성화된능력);
    }
    
    /// <summary>
    /// 스킬 효과 오브젝트 이름인지 여부 (공백 유무 모두 처리)
    /// </summary>
    private static bool IsSkillEffectName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        return name.Contains("날개") || name.Contains("매운기운") || name.Contains("매운 기운") ||
               name.Contains("삼계탕방울") || name.Contains("삼계탕 방울") ||
               name.Contains("치킨가루") || name.Contains("치킨 가루");
    }
    
    /// <summary>
    /// 하위 모든 트랜스폼에서 스킬 효과 이름을 가진 오브젝트 수집 (재귀)
    /// </summary>
    private void CollectSkillEffectsRecursive(Transform root, List<Transform> outList)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform c = root.GetChild(i);
            if (IsSkillEffectName(c.name))
                outList.Add(c);
            CollectSkillEffectsRecursive(c, outList);
        }
    }
    
    /// <summary>
    /// 기존 스킬 효과 전부 제거 (깊이 제한 없이 재귀 검사. 다른 스킨으로 바뀌었을 때 매운 기운 등이 반드시 사라지도록)
    /// </summary>
    private void RemoveExistingSkillEffects()
    {
        var toDestroy = new List<Transform>();
        CollectSkillEffectsRecursive(transform, toDestroy);
        for (int i = 0; i < toDestroy.Count; i++)
        {
            if (toDestroy[i] != null)
                Destroy(toDestroy[i].gameObject);
        }
        
        if (currentSkillEffect != null)
        {
            Destroy(currentSkillEffect);
            currentSkillEffect = null;
        }
    }
    
    /// <summary>
    /// 능력이 변경될 때 호출 (Visual Scripting Custom Event에서 호출 가능)
    /// </summary>
    public void OnAbilityChanged()
    {
        UpdateSkillEffect();
    }
}
