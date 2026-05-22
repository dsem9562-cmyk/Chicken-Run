using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject currentItem;

    public enum EndingType { UniverseConqueror, ChickenGod, BuddhaChicken, HotSpicy };
    public EndingType currentEndingType = EndingType.UniverseConqueror;

    [Header("씬 전환 딜레이 (초)")]
    public float delay = 1f;

    private bool isDead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ✅ 게임 실행할 때마다 엔딩 해금 초기화
            ResetEndings();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    // ✅ 엔딩 해금 데이터만 초기화
    void ResetEndings()
    {
        foreach (EndingType type in System.Enum.GetValues(typeof(EndingType)))
        {
            PlayerPrefs.DeleteKey("Ending_" + type.ToString());
        }

        PlayerPrefs.Save();
        Debug.Log("[GameManager] 엔딩 해금 데이터 초기화 완료");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isDead = false;

        // 게임 플레이 씬일 때만 기본 엔딩 타입 초기화
        if (scene.name == "GameScene")
        {
            currentEndingType = EndingType.UniverseConqueror;
        }
    }

    public void UnlockEnding(EndingType type)
    {
        PlayerPrefs.SetInt("Ending_" + type.ToString(), 1);
        PlayerPrefs.Save();

        Debug.Log($"<color=cyan>[GameManager]</color> {type} 엔딩 해금!");
    }

    public bool IsEndingUnlocked(EndingType type)
    {
        return PlayerPrefs.GetInt("Ending_" + type.ToString(), 0) == 1;
    }

    public void PlayEndingFromGallery(EndingType type)
    {
        if (!IsEndingUnlocked(type))
        {
            Debug.LogWarning($"[GameManager] {type} 엔딩이 아직 해금되지 않았습니다.");
            return;
        }

        currentEndingType = type;
        Debug.Log("[GameManager] 갤러리에서 엔딩 재생: " + type);
        SceneManager.LoadScene("EndingScene");
    }

    public void SetItem(EndingType type)
    {
        currentEndingType = type;
        Debug.Log("엔딩 타입 설정됨: " + type);
    }

    public void OnBossDead()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[BossDeathHandler] 보스 처치! 현재 아이템: " + currentEndingType);

        UnlockEnding(currentEndingType);

        StartCoroutine(LoadEndingScene());
    }

    System.Collections.IEnumerator LoadEndingScene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("EndingScene");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}