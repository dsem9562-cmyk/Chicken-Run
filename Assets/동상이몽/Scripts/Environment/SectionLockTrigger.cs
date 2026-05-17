using UnityEngine;

public class EnterTrigger : MonoBehaviour
{
    [Header("이전에 잠글 챕터")]
    public ChapterController previousChapter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어만 반응
        if (!other.CompareTag("Player")) return;

        // 이전 챕터 잠그기
        if (previousChapter != null)
        {
            previousChapter.LockSection();
        }
        else
        {
            Debug.LogWarning("PreviousChapter가 연결되지 않았습니다.");
        }

        // 한 번만 작동하도록 트리거 콜라이더만 비활성화 (gameObject 전체 비활성화 시 해당 섹션 배경이 사라짐)
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}