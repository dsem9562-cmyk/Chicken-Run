using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject player;
    public Image lastHpImage;        // 👈 감시할 마지막 하트 이미지

    private Sprite fullHeartSprite;  // 처음 꽉 찬 하트 그림 기억용
    private bool isGameOver = false;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // 게임 시작할 때 '꽉 찬 하트' 그림이 뭔지 몰래 기억해둠!
        if (lastHpImage != null)
        {
            fullHeartSprite = lastHpImage.sprite;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        bool isDead = false;

        // 🔍 감지 1: 닭이 파괴되었거나 꺼진 경우
        if (player != null && !player.activeInHierarchy) isDead = true;

        // 🔍 감지 2: [핵심] 마지막 하트 그림이 '꽉 찬 하트'에서 '빈 하트'로 바뀐 경우!
        if (lastHpImage != null && fullHeartSprite != null)
        {
            if (lastHpImage.sprite != fullHeartSprite)
            {
                isDead = true; // 그림이 바뀌었다 = 목숨을 잃었다!
            }
        }

        if (isDead)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("사망 그림 감지 성공! 게임 오버...");
        StartCoroutine(ForceShowPanel());
    }

    IEnumerator ForceShowPanel()
    {
        // 0.5초 대기 (절대 안 멈춤)
        yield return new WaitForSecondsRealtime(0.5f);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}