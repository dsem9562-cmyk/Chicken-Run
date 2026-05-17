using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// 플레이어가 이 오브젝트에 닿으면 즉사(강제 게임오버) 처리합니다.
/// "강제 게임오버" 오브젝트에 부착하세요. 엔딩을 재생하지 않고 플레이어만 죽입니다.
/// Collider2D가 없으면 자동으로 추가됩니다. Is Trigger를 체크해야 합니다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ForceGameOverCutsceneTrigger : MonoBehaviour
{
    [Tooltip("즉사 시 적용할 데미지 (기본: 99999)")]
    public int instantDeathDamage = 99999;

    private bool _triggered;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log("ForceGameOverCutsceneTrigger: Collider2D를 Is Trigger로 설정했습니다.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        KillPlayer(other.gameObject);
    }

    void KillPlayer(GameObject player)
    {
        // PlayerHP 컴포넌트로 데미지
        var hp = player.GetComponent<PlayerHP>();
        if (hp != null)
        {
            hp.TakeDamage(instantDeathDamage);
        }

        // Visual Scripting "목 hp" 변수 사용 시
        try
        {
            Variables.Object(player).Set("목 hp", 0f);
        }
        catch (System.Exception)
        {
            // 변수가 없으면 무시
        }

        Debug.Log("ForceGameOverCutsceneTrigger: 플레이어 강제 게임오버 (즉사)");
    }
}
