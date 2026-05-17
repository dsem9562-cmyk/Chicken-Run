using UnityEngine;

public class SectionEnter : MonoBehaviour
{
    public ChapterController previousSection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            previousSection.LockSection();
            // 트리거 콜라이더만 비활성화 (gameObject 전체 비활성화 시 해당 섹션 배경이 사라짐)
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
}