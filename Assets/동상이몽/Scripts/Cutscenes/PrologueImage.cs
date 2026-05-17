using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 게임 시작 시 이미지를 2초 보여준 뒤 2초 동안 페이드아웃 (총 4초)
/// </summary>
public class PrologueImageController : MonoBehaviour
{
    [Header("이미지")]
    [Tooltip("Scenes/컷씬/03455167199a4516.png 를 여기에 드래그 (Sprite 또는 Texture2D)")]
    public Sprite prologueImage;
    [Tooltip("Sprite 대신 Texture2D 사용 시 (PNG 기본 임포트)")]
    public Texture2D prologueTexture;

    [Header("시간 설정")]
    [Tooltip("이미지가 선명하게 보이는 시간 (초)")]
    public float showDuration = 2f;
    [Tooltip("페이드아웃에 걸리는 시간 (초)")]
    public float fadeDuration = 2f;

    [Header("종료 후")]
    [Tooltip("이미지 종료 후 시작할 카메라 섹션 (1 = 플레이)")]
    public int sectionToStartAfter = 1;

    private GameObject _canvasRoot;

    void Start()
    {
        if (prologueImage == null && prologueTexture == null)
        {
            Debug.LogWarning("PrologueImageController: prologueImage 또는 prologueTexture를 Inspector에서 지정하세요.");
            StartGameDirectly();
            return;
        }

        StartCoroutine(ShowAndFadeOut());
    }

    IEnumerator ShowAndFadeOut()
    {
        // 게임 일시정지
        float savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // 풀스크린 Canvas + Image 생성
        _canvasRoot = new GameObject("PrologueImageCanvas");
        var canvas = _canvasRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        var scaler = _canvasRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        _canvasRoot.AddComponent<GraphicRaycaster>();

        var imageGo = new GameObject("PrologueImage");
        imageGo.transform.SetParent(_canvasRoot.transform, false);

        Graphic graphic;
        if (prologueTexture != null)
        {
            var rawImage = imageGo.AddComponent<RawImage>();
            rawImage.texture = prologueTexture;
            rawImage.uvRect = new Rect(0, 0, 1, 1);
            graphic = rawImage;
        }
        else
        {
            var image = imageGo.AddComponent<Image>();
            image.sprite = prologueImage;
            graphic = image;
        }
        graphic.color = Color.white;
        graphic.raycastTarget = false;

        var rect = graphic.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;

        // 2초 동안 보이기 (unscaledDeltaTime 사용 - Time.timeScale=0이어도 동작)
        yield return new WaitForSecondsRealtime(showDuration);

        // 2초 동안 페이드아웃
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            graphic.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // 정리
        Time.timeScale = savedTimeScale;
        Destroy(_canvasRoot);

        // 한 프레임 대기 후 적용 (물리/다른 스크립트가 위치를 덮어쓰는 것 방지)
        yield return null;
        StartGameDirectly();
    }

    void StartGameDirectly()
    {
        // 프롤로그 종료 시 플레이어를 스폰 위치로 재설정 (나무선반 등 씬 기본 위치 충돌 방지)
        var initializer = FindFirstObjectByType<GameStartInitializer>();
        if (initializer != null)
            initializer.ApplySpawnPosition();

        var cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.SetSection(sectionToStartAfter);
        }
    }
}
