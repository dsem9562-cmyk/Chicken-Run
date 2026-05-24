using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    // Inspector 창에서 메인 메뉴에 틀고 싶은 음악의 인덱스 번호를 지정할 수 있습니다.
    public int bgmIndex = 0;

    void Start()
    {
        // 씬이 시작되자마자(Start) AudioManager를 통해 음악을 바로 틉니다.
        // 주의: PlayBGM 부분은 현재 프로젝트에 작성해두신 실제 오디오 재생 함수 이름으로 바꿔주세요!
        AudioManager.Instance.PlayBGM(bgmIndex);
    }
}