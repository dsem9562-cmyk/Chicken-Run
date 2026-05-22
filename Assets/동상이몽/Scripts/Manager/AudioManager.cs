using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM Settings")]
    public AudioSource bgmSource;       // BGM을 재생할 오디오 소스
    public AudioClip[] backgroundMusics; // 인스펙터에서 8개의 오디오 클립 할당 (0: 오프닝, 1~7: 배경)

    private void Awake()
    {
        // 싱글톤 세팅 (씬이 넘어가도 BGM이 끊기지 않게 하려면 DontDestroyOnLoad 사용)
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요시 주석 해제
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGM을 재생하는 함수 (인덱스 번호로 호출)
    public void PlayBGM(int bgmIndex)
    {
        // 인덱스 범위 예외 처리
        if (bgmIndex < 0 || bgmIndex >= backgroundMusics.Length)
        {
            Debug.LogWarning("BGM 인덱스가 범위를 벗어났습니다: " + bgmIndex);
            return;
        }

        // 이미 같은 BGM이 재생 중이면 무시 (섹션 이동 시 겹침 방지)
        if (bgmSource.clip == backgroundMusics[bgmIndex] && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = backgroundMusics[bgmIndex];
        bgmSource.loop = true; // 배경음악이므로 반복 재생
        bgmSource.Play();
    }

    // BGM 정지 함수
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}