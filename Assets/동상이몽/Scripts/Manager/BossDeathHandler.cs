using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDeathHandler : MonoBehaviour
{
    [Header("씬 전환 딜레이 (초)")]
    public float delay = 1f;

    private bool isDead = false;

    // Visual Scripting 그래프에서 이 함수 호출!
    public void OnBossDead()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.Instance == null)
        {
            Debug.LogError("[BossDeathHandler] GameManager 없음!");
            return;
        }

        Debug.Log("[BossDeathHandler] 보스 처치! 엔딩: "
                  + GameManager.Instance.currentItem);

        StartCoroutine(LoadEndingScene());
    }

    System.Collections.IEnumerator LoadEndingScene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("EndingScene");
    }
}