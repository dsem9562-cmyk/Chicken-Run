using UnityEngine;

/// <summary>
/// 플레이어가 "배경에 밀리는지" vs "바닥이 안 잡히는지" 구분용 진단 스크립트.
/// 플레이어에 임시로 붙인 뒤 플레이해서 Console 로그를 보세요. 원인 파악 후 제거해도 됩니다.
/// </summary>
public class PlayerMovementDiagnostic : MonoBehaviour
{
    [Header("진단 설정")]
    [Tooltip("로그 출력 간격 (초)")]
    public float logInterval = 0.5f;

    private bool groundContact;
    private float lastLogTime;
    private Vector3 lastPosition;
    private Vector2 lastVelocity;

    void Start()
    {
        lastPosition = transform.position;
        var rb = GetComponent<Rigidbody2D>();
        lastVelocity = rb != null ? rb.linearVelocity : Vector2.zero;
        Debug.Log("[진단] PlayerMovementDiagnostic 시작 - 바닥 접촉/속도/위치 로그 출력");
    }

    void OnCollisionStay2D(Collision2D col)
    {
        Debug.Log($"[충돌] OnCollisionStay2D: {col.gameObject.name} (태그: {col.gameObject.tag})");
        if (col.gameObject.CompareTag("Ground"))
        {
            groundContact = true;
            Debug.Log($"[충돌] ✓ Ground 태그 인식됨: {col.gameObject.name}");
        }
        else
        {
            Debug.Log($"[충돌] ⚠ Ground 태그 아님: {col.gameObject.name} (태그: {col.gameObject.tag})");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log($"[충돌] OnCollisionEnter2D: {col.gameObject.name} (태그: {col.gameObject.tag})");
        if (col.gameObject.CompareTag("Ground"))
        {
            groundContact = true;
            Debug.Log($"[충돌] ✓ Ground 태그 인식됨: {col.gameObject.name}");
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log($"[충돌] OnCollisionExit2D: {col.gameObject.name} (태그: {col.gameObject.tag})");
        if (col.gameObject.CompareTag("Ground"))
            groundContact = false;
    }

    void Update()
    {
        if (Time.time - lastLogTime < logInterval) return;
        lastLogTime = Time.time;

        var rb = GetComponent<Rigidbody2D>();
        Vector2 vel = rb != null ? rb.linearVelocity : Vector2.zero;
        Vector3 pos = transform.position;

        // 갑자기 큰 속도나 위치 변화 → 밀림 가능성
        float velJump = (vel - lastVelocity).magnitude;
        float posJump = (pos - lastPosition).magnitude;
        lastPosition = pos;
        lastVelocity = vel;

        string contact = groundContact ? "O (바닥 접촉 중)" : "X (바닥 없음)";
        string push = (velJump > 5f || posJump > 2f) ? " [의심: 밀림/튐]" : "";
        Debug.Log($"[진단] 바닥접촉={contact} | velocity=({vel.x:F1}, {vel.y:F1}) | pos=({pos.x:F1}, {pos.y:F1}){push}");
    }
}
