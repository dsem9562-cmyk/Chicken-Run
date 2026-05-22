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

            // 씬이 바뀔 때마다 데이터를 리셋하기 위해 이벤트 연결
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    // 새로운 씬(스테이지)이 시작될 때마다 실행되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isDead = false;

        // 게임 플레이 씬일 때만 초기화
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
        // 요청하신 문구 그대로 적용
        Debug.Log("엔딩 타입 설정됨: " + type);
    }

    public void OnBossDead()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[BossDeathHandler] 보스 처치! 현재 아이템: " + currentEndingType);

        // ✅ 엔딩 해금
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