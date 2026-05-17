using UnityEngine;

public class ChapterController : MonoBehaviour
{
    public GameObject backBlocker;
    public SpriteRenderer brokenRenderer;

    public void LockSection()
    {
        if (backBlocker != null)
        {
            backBlocker.SetActive(true);
            // BackBlocker의 BoxCollider2D 활성화 (이전 섹션으로 복귀 방지)
            var col = backBlocker.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }
        if (brokenRenderer != null)
            brokenRenderer.sortingOrder = 5;
    }
}