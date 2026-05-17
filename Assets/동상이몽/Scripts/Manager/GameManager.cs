using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 🌟 싱글톤 - 어디서든 GameManager.Instance로 접근 가능
    public static GameManager Instance;

    // ===== 엔딩 종류 =====
    public enum EndingType
    {
        UniverseConqueror = 0,  // 🌌 우주정복자 (아이템 없음)
        ChickenGod = 1,         // 🙏 강림! 치느님! (치킨)
        BuddhaChicken = 2,      // ☸️ 대자대비 닭미륵 (삼계탕)
        HotSpicy = 3            // 🔥 유니버스 핫스파이시 (불닭)
    }

    // ===== 현재 게임 상태 =====
    [Header("현재 보유 아이템")]
    public EndingType currentItem = EndingType.UniverseConqueror;
    // 👆 기본값: 아이템 없음 = 우주정복자 엔딩

    // ===== Awake: 싱글톤 세팅 =====
    void Awake()
    {
        // 이미 있으면 새 거 삭제 (중복 방지)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 바뀌어도 유지
    }

    // ===== 아이템 획득 =====
    public void SetItem(EndingType item)
    {
        currentItem = item;
        Debug.Log($"🎒 아이템 획득: {item}");
    }

    // ===== 새 게임 시작 시 초기화 =====
    public void ResetGame()
    {
        currentItem = EndingType.UniverseConqueror;
        Debug.Log("🔄 게임 데이터 초기화");
    }

    // ===== 엔딩 잠금 해제 =====
    public void UnlockEnding(EndingType ending)
    {
        string key = "Ending_" + (int)ending;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"🏆 엔딩 잠금 해제: {ending}");
    }

    // ===== 엔딩이 잠금 해제됐는지 확인 =====
    public bool IsEndingUnlocked(EndingType ending)
    {
        string key = "Ending_" + (int)ending;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    // ===== 모든 엔딩 기록 삭제 (디버그용) =====
    public void ResetAllEndings()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.DeleteKey("Ending_" + i);
        }
        PlayerPrefs.Save();
        Debug.Log("🗑️ 모든 엔딩 기록 삭제됨");
    }
}