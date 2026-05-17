using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

/// <summary>
/// Visual Scripting의 Ground 체크를 이름으로도 인식하도록 수정하는 유틸리티
/// </summary>
public class VisualScriptingGroundUpdater : EditorWindow
{
    [MenuItem("Tools/바닥 설정/Visual Scripting - 이름으로 바닥 인식 추가")]
    public static void UpdateVisualScriptingForFloorNames()
    {
        Debug.Log("Visual Scripting 수정 가이드:");
        Debug.Log("1. 플레이어 오브젝트를 선택합니다");
        Debug.Log("2. Inspector에서 Visual Scripting 컴포넌트를 찾습니다");
        Debug.Log("3. 그래프를 엽니다");
        Debug.Log("4. OnCollisionStay2D와 OnCollisionExit2D 이벤트에서");
        Debug.Log("5. CompareTag('Ground') 노드를 찾습니다");
        Debug.Log("6. 이 노드 옆에 이름 체크를 추가합니다:");
        Debug.Log("   - Get GameObject Name 노드 추가");
        Debug.Log("   - String Contains 노드 추가 (검색어: '바닥')");
        Debug.Log("   - OR 노드로 두 조건을 연결");
        Debug.Log("");
        Debug.Log("또는 더 간단한 방법:");
        Debug.Log("모든 바닥 오브젝트에 'Ground' 태그를 설정하면");
        Debug.Log("기존 Visual Scripting이 그대로 작동합니다!");
        
        EditorUtility.DisplayDialog("Visual Scripting 수정 안내",
            "Visual Scripting을 수정하려면:\n\n" +
            "1. 플레이어 오브젝트 선택\n" +
            "2. Visual Scripting 그래프 열기\n" +
            "3. OnCollisionStay2D/OnCollisionExit2D에서\n" +
            "   CompareTag('Ground') 노드 찾기\n" +
            "4. 이름 체크 추가 (String Contains '바닥')\n" +
            "5. OR 노드로 연결\n\n" +
            "또는:\n" +
            "모든 바닥에 'Ground' 태그를 설정하면\n" +
            "기존 코드가 그대로 작동합니다!",
            "확인");
    }
}
