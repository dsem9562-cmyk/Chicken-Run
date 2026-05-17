using UnityEngine;

/// <summary>
/// 바닥 감지 헬퍼 클래스
/// 챕터별 바닥 이름을 인식할 수 있도록 도와줍니다.
/// </summary>
public static class GroundChecker
{
    /// <summary>
    /// 오브젝트가 바닥인지 확인합니다.
    /// Ground 태그 또는 바닥 이름 패턴을 체크합니다.
    /// </summary>
    /// <param name="gameObject">확인할 오브젝트</param>
    /// <returns>바닥이면 true</returns>
    public static bool IsGround(GameObject gameObject)
    {
        if (gameObject == null) return false;
        
        // Ground 태그 체크
        if (gameObject.CompareTag("Ground"))
            return true;
        
        // 바닥 이름 패턴 체크
        string name = gameObject.name;
        if (name.Contains("바닥") || name.Contains("바닥(챕터"))
            return true;
        
        return false;
    }
    
    /// <summary>
    /// Collider2D가 바닥인지 확인합니다.
    /// </summary>
    public static bool IsGround(Collider2D collider)
    {
        if (collider == null) return false;
        return IsGround(collider.gameObject);
    }
}
