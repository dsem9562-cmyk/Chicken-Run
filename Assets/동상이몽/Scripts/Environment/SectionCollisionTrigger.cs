using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// Section_XX_Collision 또는 Section_XX 이름에서 섹션 번호를 자동 추출하여
/// 플레이어가 통과할 때 카메라를 해당 섹션 모드로 전환합니다.
/// Section_04 이상에서는 카메라가 자동 스크롤됩니다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SectionCollisionTrigger : MonoBehaviour
{
    [Tooltip("비워두면 오브젝트 이름에서 자동 추출 (Section_04_Collision -> 4)")]
    public int sectionIndexOverride;

    private int _cachedSection = -1;

    int GetSectionIndex()
    {
        if (sectionIndexOverride > 0) return sectionIndexOverride;
        if (_cachedSection >= 0) return _cachedSection;

        var match = Regex.Match(gameObject.name, @"Section_0?(\d+)");
        _cachedSection = match.Success ? int.Parse(match.Groups[1].Value) : 0;
        return _cachedSection;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Camera.main == null) return;

        var cam = Camera.main.GetComponent<CameraController>();
        if (cam == null) return;

        int section = GetSectionIndex();
        // SectionTrigger처럼 경계(boundaryMinX) 전달 - 이전 챕터로 못 가게
        float boundaryX = float.NaN;
        if (section >= 1 && section <= 3)
        {
            var col = GetComponent<Collider2D>();
            boundaryX = (col != null && col.enabled) ? col.bounds.min.x : transform.position.x;
        }
        Debug.Log($"SectionCollisionTrigger: {gameObject.name} -> 섹션 {section} (경계 x={boundaryX:F2}, 카메라 자동이동: {(section >= 4 ? "예" : "아니오")})");
        cam.SetSection(section, boundaryX);
    }
}
