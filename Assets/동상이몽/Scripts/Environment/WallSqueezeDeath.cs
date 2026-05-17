using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// Section 4~7 (AutoScroll)에서 플레이어가 카메라 왼쪽 흰 벽(화면 가장자리)에 끼였을 때
/// 즉사 대신 체력이 감소하도록 처리합니다.
/// </summary>
public class AutoScrollDeathChecker : MonoBehaviour
{
    [Header("체력 감소 설정")]
    [Tooltip("벽에 끼였을 때 한 번에 감소할 체력")]
    public float squeezeDamage = 10f;
    [Tooltip("데미지 적용 간격 (초). 이 간격마다 체력이 감소합니다.")]
    public float damageInterval = 0.4f;
    [Tooltip("흰 벽에 끼였다고 판정하는 구역 (카메라 왼쪽 가장자리 기준, 이 값 이내면 끼인 것으로 간주)")]
    public float squeezeZoneWidth = 1.5f;

    private float lastDamageTime;
    private const string HpVariableName = "목 hp";

    void Update()
    {
        if (Camera.main == null) return;

        var camController = Camera.main.GetComponent<CameraController>();
        if (camController == null) return;
        if (camController.mode != CameraController.CameraMode.AutoScroll) return;

        float cameraLeftEdge = Camera.main.transform.position.x - 9f;
        float playerX = transform.position.x;

        // 플레이어가 흰 벽(카메라 왼쪽 가장자리)에 끼인 구역에 있는지 확인
        bool isSqueezed = playerX < cameraLeftEdge + squeezeZoneWidth;

        if (isSqueezed)
        {
            // 데미지 쿨다운 후 체력 감소
            if (Time.time - lastDamageTime >= damageInterval)
            {
                lastDamageTime = Time.time;
                ApplySqueezeDamage();
            }
        }
    }

    void ApplySqueezeDamage()
    {
        try
        {
            float currentHp = Variables.Object(gameObject).Get<float>(HpVariableName);
            float newHp = Mathf.Max(0f, currentHp - squeezeDamage);
            Variables.Object(gameObject).Set(HpVariableName, newHp);

            if (newHp <= 0f)
            {
                Debug.Log("Player Dead (체력 소진 - 벽에 끼임)");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"AutoScrollDeathChecker: 체력 변수 '{HpVariableName}' 접근 실패 - {e.Message}");
        }
    }
}
