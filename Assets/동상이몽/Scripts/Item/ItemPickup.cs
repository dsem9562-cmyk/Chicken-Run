using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("이 아이템이 부여할 엔딩 타입")]
    public GameManager.EndingType endingType;

    public void OnItemPickup()
    {
        if (GameManager.Instance != null)
        {
            // 1. 매니저에게 정보 전달
            GameManager.Instance.SetItem(endingType);

            // 2. 확인 로그 출력
            Debug.Log("[ItemPickup] 아이템 획득! 엔딩타입: \" + endingType");
        }
        else
        {
            Debug.LogError("[ItemPickup] GameManager 없음!");
        }
    }
}