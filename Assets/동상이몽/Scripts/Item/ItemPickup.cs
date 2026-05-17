using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("이 아이템의 종류")]
    public GameManager.EndingType itemType;
    // 👆 Inspector에서 어떤 아이템인지 선택
    // - UniverseConqueror (기본/없음)
    // - ChickenGod (치킨)
    // - BuddhaChicken (삼계탕)
    // - HotSpicy (불닭)

    [Header("효과음 (선택)")]
    public AudioClip pickupSound;

    // 플레이어와 충돌 시
    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그인지 확인
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        // GameManager에 아이템 저장
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetItem(itemType);
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager가 씬에 없습니다!");
        }

        // 효과음 재생 (있으면)
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // 아이템 사라짐
        Destroy(gameObject);
    }
}