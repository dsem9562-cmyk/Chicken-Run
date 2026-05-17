using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public Image endingImage;

    public Sprite[] endingUniverseConqueror;
    public Sprite[] endingChickenGod;
    public Sprite[] endingBuddhaChicken;
    public Sprite[] endingHotSpicy;

    public float timePerCut = 2.5f;
    public float endDelay = 1f;

    private Sprite[] currentEnding;

    void Start()
    {
        // ✅ GameManager 존재 확인
        if (GameManager.Instance == null)
        {
            Debug.LogError("[EndingManager] GameManager.Instance가 null입니다! DontDestroyOnLoad 확인 필요");
            return;
        }

        // ✅ 현재 엔딩 타입 로그 확인
        Debug.Log("[EndingManager] 현재 엔딩 타입: " + GameManager.Instance.currentItem);

        // ✅ 엔딩 해금
        GameManager.Instance.UnlockEnding(GameManager.Instance.currentItem);

        // ✅ 엔딩 스프라이트 배열 가져오기
        currentEnding = GetEndingByType(GameManager.Instance.currentItem);

        // ✅ 엔딩 컷 수 로그 확인
        Debug.Log("[EndingManager] 엔딩 컷 수: " + currentEnding.Length);

        // ✅ 스프라이트 배열이 비어있으면 중단
        if (currentEnding == null || currentEnding.Length == 0)
        {
            Debug.LogError("[EndingManager] 엔딩 스프라이트 배열이 비어있거나 null입니다! Inspector에서 스프라이트를 할당했는지 확인하세요.");
            return;
        }

        // ✅ EndingImage 초기화 (Alpha 0 방지, 비활성화 방지)
        if (endingImage == null)
        {
            Debug.LogError("[EndingManager] endingImage가 연결되지 않았습니다! Inspector에서 EndingImage를 연결하세요.");
            return;
        }

        endingImage.gameObject.SetActive(true);
        endingImage.color = Color.white;   // Alpha 강제로 1로
        endingImage.sprite = currentEnding[0]; // 첫 번째 컷 미리 표시

        StartCoroutine(PlayEnding());
    }

    Sprite[] GetEndingByType(GameManager.EndingType type)
    {
        switch (type)
        {
            case GameManager.EndingType.ChickenGod:
                Debug.Log("[EndingManager] ChickenGod 엔딩 선택됨");
                return endingChickenGod;
            case GameManager.EndingType.BuddhaChicken:
                Debug.Log("[EndingManager] BuddhaChicken 엔딩 선택됨");
                return endingBuddhaChicken;
            case GameManager.EndingType.HotSpicy:
                Debug.Log("[EndingManager] HotSpicy 엔딩 선택됨");
                return endingHotSpicy;
            default:
                Debug.Log("[EndingManager] UniverseConqueror(기본) 엔딩 선택됨");
                return endingUniverseConqueror;
        }
    }

    IEnumerator PlayEnding()
    {
        for (int i = 0; i < currentEnding.Length; i++)
        {
            // ✅ 각 스프라이트 null 체크
            if (currentEnding[i] == null)
            {
                Debug.LogWarning("[EndingManager] currentEnding[" + i + "]이 null입니다! Inspector에서 스프라이트 슬롯을 확인하세요.");
                continue;
            }

            Debug.Log("[EndingManager] 컷 표시 중: " + i + " / " + (currentEnding.Length - 1));
            endingImage.sprite = currentEnding[i];
            yield return new WaitForSeconds(timePerCut);
        }

        Debug.Log("[EndingManager] 엔딩 재생 완료. " + endDelay + "초 후 MainMenu로 이동");
        yield return new WaitForSeconds(endDelay);

        SceneManager.LoadScene("MainMenu");
    }
}