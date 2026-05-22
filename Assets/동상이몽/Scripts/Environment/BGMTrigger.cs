using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    [Tooltip("이 구역에 들어오면 재생할 BGM의 인덱스 번호 (0: 오프닝, 1~7: 각 배경)")]
    public int bgmIndexToPlay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 콜라이더(트리거) 영역에 닿았을 때 실행
        if (collision.CompareTag("Player"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(bgmIndexToPlay);
            }
        }
    }
}