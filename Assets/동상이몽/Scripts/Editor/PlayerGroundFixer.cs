using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어가 바닥을 인식하지 못하는 문제를 해결하는 유틸리티
/// </summary>
public class PlayerGroundFixer : EditorWindow
{
    [MenuItem("Tools/플레이어 설정/바닥 인식 문제 해결")]
    public static void FixPlayerGroundDetection()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        
        if (!activeScene.IsValid())
        {
            Debug.LogError("활성 씬이 없습니다!");
            EditorUtility.DisplayDialog("오류", "활성 씬이 없습니다!", "확인");
            return;
        }
        
        // 모든 오브젝트 찾기
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        System.Collections.Generic.List<GameObject> floors = new System.Collections.Generic.List<GameObject>();
        System.Collections.Generic.List<GameObject> players = new System.Collections.Generic.List<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene)
            {
                // 바닥 오브젝트 찾기 (이름에 "바닥", "Ground", "Floor", "양계장" 포함 또는 Ground 태그)
                // 단, Section 오브젝트는 제외 (트리거용이므로)
                string objName = obj.name.ToLower();
                if (objName.StartsWith("section_"))
                {
                    // Section 오브젝트는 바닥으로 인식하지 않음
                    continue;
                }
                
                if (objName.Contains("바닥") || objName.Contains("챕터") || 
                    objName.Contains("양계장") || objName.Contains("플랫폼") ||
                    objName.StartsWith("ground") || objName.StartsWith("floor") || 
                    obj.CompareTag("Ground"))
                {
                    floors.Add(obj);
                }
                
                // 플레이어 오브젝트 찾기
                if (obj.CompareTag("Player") || obj.name.Contains("플레이어"))
                {
                    players.Add(obj);
                }
            }
        }
        
        string report = "=== 바닥 인식 문제 진단 ===\n\n";
        
        // 플레이어 확인
        report += $"플레이어 오브젝트: {players.Count}개\n";
        if (players.Count == 0)
        {
            report += "⚠️ 플레이어 오브젝트를 찾을 수 없습니다!\n";
            report += "플레이어에 'Player' 태그가 설정되어 있는지 확인하세요.\n\n";
        }
        else
        {
            foreach (var player in players)
            {
                report += $"  - {player.name} (태그: {player.tag})\n";
                
                // Collider2D 확인
                var collider = player.GetComponent<Collider2D>();
                if (collider == null)
                {
                    report += "    ⚠️ Collider2D가 없습니다!\n";
                }
                else
                {
                    report += $"    ✓ Collider2D 발견 (활성화: {collider.enabled}, IsTrigger: {collider.isTrigger})\n";
                    if (!collider.enabled)
                    {
                        report += "      ⚠️ Collider2D가 비활성화되어 있습니다!\n";
                    }
                    if (collider.isTrigger)
                    {
                        report += "      ⚠️ Collider2D가 Trigger로 설정되어 있습니다! (충돌 불가)\n";
                    }
                }
                
                // Rigidbody2D 확인
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    report += "    ⚠️ Rigidbody2D가 없습니다!\n";
                }
                else
                {
                    report += $"    ✓ Rigidbody2D 발견 (Gravity Scale: {rb.gravityScale})\n";
                }
                
                // Layer 확인
                report += $"    Layer: {LayerMask.LayerToName(player.layer)} ({player.layer})\n";
            }
            report += "\n";
        }
        
        // 바닥 확인
        report += $"바닥 오브젝트: {floors.Count}개\n";
        if (floors.Count == 0)
        {
            report += "⚠️ 바닥 오브젝트를 찾을 수 없습니다!\n\n";
        }
        else
        {
            int groundTaggedCount = 0;
            int missingTagCount = 0;
            
            foreach (var floor in floors)
            {
                if (floor.CompareTag("Ground"))
                {
                    groundTaggedCount++;
                }
                else
                {
                    missingTagCount++;
                    report += $"  ⚠️ {floor.name} - Ground 태그 없음 (현재: {floor.tag})\n";
                }
                
                // Collider2D 확인
                var collider = floor.GetComponent<Collider2D>();
                if (collider == null)
                {
                    report += $"    ⚠️ {floor.name} - Collider2D가 없습니다!\n";
                }
                else
                {
                    report += $"    ✓ {floor.name} - Collider2D 발견 (활성화: {collider.enabled}, IsTrigger: {collider.isTrigger})\n";
                    if (!collider.enabled)
                    {
                        report += $"      ⚠️ Collider2D가 비활성화되어 있습니다!\n";
                    }
                    if (collider.isTrigger)
                    {
                        report += $"      ⚠️ Collider2D가 Trigger로 설정되어 있습니다! (충돌 불가)\n";
                    }
                }
                
                // Layer 확인
                report += $"    Layer: {LayerMask.LayerToName(floor.layer)} ({floor.layer})\n";
            }
            
            report += $"\nGround 태그 설정됨: {groundTaggedCount}개\n";
            report += $"Ground 태그 없음: {missingTagCount}개\n\n";
            
            if (missingTagCount > 0)
            {
                string missingNames = "";
                int count = 0;
                foreach (var floor in floors)
                {
                    if (!floor.CompareTag("Ground"))
                    {
                        if (count < 5) // 처음 5개만 표시
                            missingNames += $"\n- {floor.name} (현재 태그: {floor.tag})";
                        count++;
                    }
                }
                if (count > 5)
                    missingNames += $"\n... 외 {count - 5}개";
                
                bool fix = EditorUtility.DisplayDialog("바닥 인식 문제 발견",
                    $"총 {floors.Count}개의 바닥 오브젝트 중 {missingTagCount}개에 'Ground' 태그가 없습니다.{missingNames}\n\n" +
                    "플레이어가 바닥을 인식하지 못하는 이유는 Visual Scripting이 'Ground' 태그만 체크하기 때문입니다.\n\n" +
                    "모든 바닥 오브젝트에 'Ground' 태그를 자동으로 설정하시겠습니까?",
                    "태그 설정", "취소");
                
                if (fix)
                {
                    int fixedCount = 0;
                    foreach (var floor in floors)
                    {
                        if (!floor.CompareTag("Ground"))
                        {
                            floor.tag = "Ground";
                            fixedCount++;
                            Debug.Log($"Ground 태그 설정: {floor.name}");
                        }
                    }
                    
                    EditorSceneManager.MarkSceneDirty(activeScene);
                    
                    EditorUtility.DisplayDialog("수정 완료",
                        $"{fixedCount}개의 바닥 오브젝트에 'Ground' 태그가 설정되었습니다.\n\n" +
                        "이제 플레이어가 바닥을 정상적으로 인식할 수 있습니다!",
                        "확인");
                    
                    return;
                }
            }
            else
            {
                report += "✅ 모든 바닥에 Ground 태그가 설정되어 있습니다!\n\n";
            }
        }
        
        // Background 오브젝트 확인 (밀림 원인 가능성)
        report += "=== Background 오브젝트 확인 ===\n";
        int backgroundWithCollider = 0;
        System.Collections.Generic.List<GameObject> backgroundsToFix = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene && (obj.name.Contains("Background") || obj.name.Contains("양계장")))
            {
                var collider = obj.GetComponent<Collider2D>();
                if (collider != null && collider.enabled && !collider.isTrigger)
                {
                    backgroundWithCollider++;
                    backgroundsToFix.Add(obj);
                    report += $"  ⚠️ {obj.name} - Collider2D가 활성화되어 있습니다! (플레이어를 밀 수 있음)\n";
                }
            }
        }
        if (backgroundWithCollider == 0)
        {
            report += "✅ Background 오브젝트에 문제 없음\n";
        }
        else
        {
            report += $"\n⚠️ 총 {backgroundWithCollider}개의 Background/양계장 오브젝트가 Collider2D를 가지고 있습니다.\n";
            report += "배경은 Collider2D가 없어야 합니다. 바닥만 별도 오브젝트로 두세요.\n";
            
            bool fix = EditorUtility.DisplayDialog("배경 Collider2D 문제 발견",
                $"총 {backgroundWithCollider}개의 배경 오브젝트(양계장 포함)가 Collider2D를 가지고 있습니다.\n\n" +
                "배경은 시각적 요소일 뿐이므로 Collider2D가 없어야 합니다.\n" +
                "플레이어가 밟을 수 있는 바닥은 '바닥(챕터1~3)' 오브젝트만 사용하세요.\n\n" +
                "배경 오브젝트의 Collider2D를 비활성화하시겠습니까?",
                "Collider2D 비활성화", "취소");
            
            if (fix)
            {
                int fixedCount = 0;
                foreach (var bg in backgroundsToFix)
                {
                    var collider = bg.GetComponent<Collider2D>();
                    if (collider != null && collider.enabled)
                    {
                        collider.enabled = false;
                        fixedCount++;
                        Debug.Log($"배경 Collider2D 비활성화: {bg.name}");
                    }
                }
                
                EditorSceneManager.MarkSceneDirty(activeScene);
                
                EditorUtility.DisplayDialog("수정 완료",
                    $"{fixedCount}개의 배경 오브젝트의 Collider2D가 비활성화되었습니다.\n\n" +
                    "이제 플레이어는 '바닥(챕터1~3)' 오브젝트만 밟을 수 있습니다!",
                    "확인");
            }
        }
        report += "\n";
        
        // Section 오브젝트 확인 (트리거가 아닌 Collider2D 문제)
        report += "=== Section 오브젝트 확인 ===\n";
        int sectionWithNonTriggerCollider = 0;
        System.Collections.Generic.List<GameObject> sectionsToFix = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene && obj.name.StartsWith("Section_"))
            {
                var collider = obj.GetComponent<Collider2D>();
                if (collider != null)
                {
                    // Section_02 특별 확인
                    if (obj.name == "Section_02")
                    {
                        report += $"  [Section_02 상세 정보]\n";
                        report += $"    - Collider2D 활성화: {collider.enabled}\n";
                        report += $"    - Is Trigger: {collider.isTrigger}\n";
                        report += $"    - Collider2D 타입: {collider.GetType().Name}\n";
                        if (collider is BoxCollider2D boxCollider)
                        {
                            report += $"    - 크기: {boxCollider.size}\n";
                            report += $"    - 오프셋: {boxCollider.offset}\n";
                            // Collider2D의 실제 범위 계산
                            Vector2 colliderMin = (Vector2)obj.transform.position + boxCollider.offset - boxCollider.size * 0.5f;
                            Vector2 colliderMax = (Vector2)obj.transform.position + boxCollider.offset + boxCollider.size * 0.5f;
                            report += $"    - Collider2D 범위: x({colliderMin.x:F2} ~ {colliderMax.x:F2}), y({colliderMin.y:F2} ~ {colliderMax.y:F2})\n";
                        }
                        report += $"    - 위치: {obj.transform.position}\n";
                        report += $"    - 자식 오브젝트 수: {obj.transform.childCount}\n";
                    }
                    
                    // Section_01도 확인
                    if (obj.name == "Section_01")
                    {
                        var section01Collider = obj.GetComponent<Collider2D>();
                        if (section01Collider != null && section01Collider is BoxCollider2D section01Box)
                        {
                            Vector2 section01Min = (Vector2)obj.transform.position + section01Box.offset - section01Box.size * 0.5f;
                            Vector2 section01Max = (Vector2)obj.transform.position + section01Box.offset + section01Box.size * 0.5f;
                            report += $"  [Section_01 Collider2D 범위: x({section01Min.x:F2} ~ {section01Max.x:F2})]\n";
                        }
                    }
                    
                    if (collider.enabled && !collider.isTrigger)
                    {
                        sectionWithNonTriggerCollider++;
                        sectionsToFix.Add(obj);
                        report += $"  ⚠️ {obj.name} - Collider2D가 Is Trigger로 설정되지 않았습니다! (플레이어를 밀 수 있음)\n";
                    }
                }
            }
        }
        if (sectionWithNonTriggerCollider == 0)
        {
            report += "✅ Section 오브젝트에 문제 없음\n";
        }
        else
        {
            report += $"\n⚠️ 총 {sectionWithNonTriggerCollider}개의 Section 오브젝트가 Is Trigger로 설정되지 않았습니다.\n";
            report += "Section 오브젝트는 트리거용이므로 Is Trigger를 체크해야 합니다.\n";
            
            bool fix = EditorUtility.DisplayDialog("Section 오브젝트 문제 발견",
                $"총 {sectionWithNonTriggerCollider}개의 Section 오브젝트가 Is Trigger로 설정되지 않았습니다.\n\n" +
                "이것들이 플레이어를 밀거나 충돌을 일으킬 수 있습니다.\n\n" +
                "모든 Section 오브젝트의 Collider2D를 Is Trigger로 자동 설정하시겠습니까?",
                "Is Trigger 설정", "취소");
            
            if (fix)
            {
                int fixedCount = 0;
                foreach (var section in sectionsToFix)
                {
                    var collider = section.GetComponent<Collider2D>();
                    if (collider != null && !collider.isTrigger)
                    {
                        collider.isTrigger = true;
                        fixedCount++;
                        Debug.Log($"Section Is Trigger 설정: {section.name}");
                    }
                }
                
                EditorSceneManager.MarkSceneDirty(activeScene);
                
                EditorUtility.DisplayDialog("수정 완료",
                    $"{fixedCount}개의 Section 오브젝트에 Is Trigger가 설정되었습니다.\n\n" +
                    "이제 플레이어가 Section 오브젝트를 통과할 수 있습니다!",
                    "확인");
            }
        }
        report += "\n";
        
        // Layer 충돌 확인
        report += "=== Layer 충돌 확인 ===\n";
        if (players.Count > 0 && floors.Count > 0)
        {
            var player = players[0];
            int playerLayer = player.layer;
            int groundLayer = floors[0].layer;
            
            bool layersCollide = Physics2D.GetLayerCollisionMask(playerLayer) == (Physics2D.GetLayerCollisionMask(playerLayer) | (1 << groundLayer));
            
            if (!layersCollide)
            {
                report += $"⚠️ 플레이어 Layer({LayerMask.LayerToName(playerLayer)})와 바닥 Layer({LayerMask.LayerToName(groundLayer)})가 충돌하지 않습니다!\n";
                report += "Edit > Project Settings > Physics 2D > Layer Collision Matrix에서 확인하세요.\n";
            }
            else
            {
                report += $"✓ 플레이어 Layer({LayerMask.LayerToName(playerLayer)})와 바닥 Layer({LayerMask.LayerToName(groundLayer)}) 충돌 설정 정상\n";
            }
        }
        report += "\n";
        
        // 벽 오브젝트 확인 (플레이어 이동 방해 가능성)
        report += "=== 벽/장애물 오브젝트 확인 ===\n";
        int wallWithCollider = 0;
        System.Collections.Generic.List<GameObject> wallsToCheck = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene)
            {
                string objName = obj.name.ToLower();
                // 벽 관련 오브젝트 찾기 (더 광범위하게)
                if (objName.Contains("벽") || objName.Contains("wall") || 
                    objName.Contains("철조망") || objName.Contains("fence") ||
                    objName.Contains("문") || objName.Contains("door") ||
                    objName.Contains("위쪽") || objName.Contains("아래쪽") ||
                    objName.Contains("좌측") || objName.Contains("우측") ||
                    objName.Contains("left") || objName.Contains("right") ||
                    objName.Contains("top") || objName.Contains("bottom"))
                {
                    var collider = obj.GetComponent<Collider2D>();
                    if (collider != null && collider.enabled && !collider.isTrigger)
                    {
                        wallWithCollider++;
                        wallsToCheck.Add(obj);
                        report += $"  ⚠️ {obj.name} - Collider2D가 활성화되어 있습니다! (플레이어 이동 방해 가능)\n";
                    }
                }
            }
        }
        
        // Section 오브젝트의 자식 오브젝트들도 확인
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene && obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                if (parent.name.StartsWith("Section_"))
                {
                    // Section_02의 자식 오브젝트 특별 확인
                    if (parent.name == "Section_02")
                    {
                        report += $"  [Section_02 자식: {obj.name}]\n";
                        report += $"    - 위치: {obj.transform.position}\n";
                        report += $"    - 활성화: {obj.activeSelf}\n";
                    }
                    
                    var collider = obj.GetComponent<Collider2D>();
                    if (collider != null)
                    {
                        if (parent.name == "Section_02")
                        {
                            report += $"    - Collider2D 활성화: {collider.enabled}\n";
                            report += $"    - Is Trigger: {collider.isTrigger}\n";
                            report += $"    - Collider2D 타입: {collider.GetType().Name}\n";
                        }
                        
                        if (collider.enabled && !collider.isTrigger)
                        {
                            // 바닥이 아닌 경우만 체크
                            if (!obj.CompareTag("Ground") && !obj.name.Contains("바닥"))
                            {
                                wallWithCollider++;
                                wallsToCheck.Add(obj);
                                report += $"  ⚠️ {obj.name} (부모: {parent.name}) - Collider2D가 활성화되어 있습니다! (플레이어 이동 방해 가능)\n";
                            }
                        }
                    }
                    else if (parent.name == "Section_02")
                    {
                        report += $"    - Collider2D 없음\n";
                    }
                }
            }
        }
        
        // Section_01과 Section_02 사이의 오브젝트 확인
        report += "\n=== Section_01 ~ Section_02 사이 오브젝트 확인 ===\n";
        GameObject section01 = null;
        GameObject section02 = null;
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene)
            {
                if (obj.name == "Section_01") section01 = obj;
                if (obj.name == "Section_02") section02 = obj;
            }
        }
        
        if (section01 != null && section02 != null)
        {
            float section01X = section01.transform.position.x;
            float section02X = section02.transform.position.x;
            float minX = Mathf.Min(section01X, section02X);
            float maxX = Mathf.Max(section01X, section02X);
            
            report += $"Section_01 위치: x={section01X:F2}\n";
            report += $"Section_02 위치: x={section02X:F2}\n";
            report += $"확인 범위: x={minX:F2} ~ {maxX:F2}\n\n";
            
            int blockingObjects = 0;
            System.Collections.Generic.List<GameObject> blockingObjectsList = new System.Collections.Generic.List<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.scene == activeScene && obj != section01 && obj != section02)
                {
                    float objX = obj.transform.position.x;
                    // Section 사이에 있는 오브젝트 확인
                    if (objX >= minX && objX <= maxX)
                    {
                        var collider = obj.GetComponent<Collider2D>();
                        if (collider != null && collider.enabled && !collider.isTrigger)
                        {
                            // 플레이어는 제외
                            if (obj.CompareTag("Player"))
                                continue;
                                
                            // Section 오브젝트나 바닥이 아닌 경우만 체크
                            if (!obj.name.StartsWith("Section_") && 
                                !obj.CompareTag("Ground") && 
                                !obj.name.Contains("바닥") &&
                                (obj.transform.parent == null || !obj.transform.parent.name.StartsWith("Section_")))
                            {
                                blockingObjects++;
                                blockingObjectsList.Add(obj);
                                report += $"  ⚠️ {obj.name} (위치: x={objX:F2}) - Collider2D가 활성화되어 있습니다! (플레이어 이동 방해 가능)\n";
                                report += $"      Is Trigger: {collider.isTrigger}, 활성화: {collider.enabled}\n";
                                if (collider is BoxCollider2D boxCollider)
                                {
                                    Vector2 colliderMin = (Vector2)obj.transform.position + boxCollider.offset - boxCollider.size * 0.5f;
                                    Vector2 colliderMax = (Vector2)obj.transform.position + boxCollider.offset + boxCollider.size * 0.5f;
                                    report += $"      Collider2D 범위: x({colliderMin.x:F2} ~ {colliderMax.x:F2})\n";
                                }
                            }
                        }
                    }
                }
            }
            
            if (blockingObjects == 0)
            {
                report += "✅ Section_01과 Section_02 사이에 막는 오브젝트 없음\n";
            }
            else
            {
                report += $"\n⚠️ 총 {blockingObjects}개의 오브젝트가 Section 사이에 있습니다.\n";
                report += "이 오브젝트들은 배경 오브젝트일 가능성이 높으므로 Collider2D를 비활성화하거나 Is Trigger로 설정해야 합니다.\n";
                
                bool fix = EditorUtility.DisplayDialog("Section 사이 막는 오브젝트 발견",
                    $"총 {blockingObjects}개의 오브젝트가 Section_01과 Section_02 사이에 있어 플레이어 이동을 방해할 수 있습니다.\n\n" +
                    "이 오브젝트들은 배경 오브젝트일 가능성이 높습니다.\n\n" +
                    "이 오브젝트들의 Collider2D를 비활성화하시겠습니까? (배경 오브젝트인 경우 권장)",
                    "Collider2D 비활성화", "취소");
                
                if (fix)
                {
                    int fixedCount = 0;
                    foreach (var blockingObj in blockingObjectsList)
                    {
                        var collider = blockingObj.GetComponent<Collider2D>();
                        if (collider != null && collider.enabled)
                        {
                            collider.enabled = false;
                            fixedCount++;
                            Debug.Log($"Section 사이 막는 오브젝트 Collider2D 비활성화: {blockingObj.name}");
                        }
                    }
                    
                    EditorSceneManager.MarkSceneDirty(activeScene);
                    
                    EditorUtility.DisplayDialog("수정 완료",
                        $"{fixedCount}개의 오브젝트의 Collider2D가 비활성화되었습니다.\n\n" +
                        "이제 플레이어가 Section_01에서 Section_02로 이동할 수 있습니다!",
                        "확인");
                }
            }
        }
        else
        {
            report += "⚠️ Section_01 또는 Section_02를 찾을 수 없습니다.\n";
        }
        report += "\n";
        
        // 양계장의 자식 오브젝트들도 확인 (철조망 등)
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene == activeScene && obj.transform.parent != null)
            {
                GameObject parent = obj.transform.parent.gameObject;
                if (parent.name.Contains("양계장"))
                {
                    var collider = obj.GetComponent<Collider2D>();
                    if (collider != null && collider.enabled && !collider.isTrigger)
                    {
                        // 바닥이 아닌 경우만 체크
                        if (!obj.CompareTag("Ground") && !obj.name.Contains("바닥"))
                        {
                            wallWithCollider++;
                            wallsToCheck.Add(obj);
                            report += $"  ⚠️ {obj.name} (부모: 양계장) - Collider2D가 활성화되어 있습니다! (플레이어 이동 방해 가능)\n";
                        }
                    }
                }
            }
        }
        
        if (wallWithCollider == 0)
        {
            report += "✅ 벽/장애물 오브젝트에 문제 없음\n";
        }
        else
        {
            report += $"\n⚠️ 총 {wallWithCollider}개의 벽/장애물 오브젝트가 Collider2D를 가지고 있습니다.\n";
            report += "플레이어가 통과해야 하는 벽은 Is Trigger로 설정하거나 Collider2D를 제거하세요.\n";
            
            bool fix = EditorUtility.DisplayDialog("벽/장애물 Collider2D 문제 발견",
                $"총 {wallWithCollider}개의 벽/장애물 오브젝트가 Collider2D를 가지고 있습니다.\n\n" +
                "이것들이 플레이어 이동을 방해할 수 있습니다.\n\n" +
                "플레이어가 통과해야 하는 벽은:\n" +
                "- Is Trigger로 설정하거나\n" +
                "- Collider2D를 비활성화하세요\n\n" +
                "벽 오브젝트 목록을 Console에 출력하시겠습니까?",
                "목록 출력", "취소");
            
            if (fix)
            {
                Debug.Log("=== 벽/장애물 오브젝트 목록 ===");
                foreach (var wall in wallsToCheck)
                {
                    var collider = wall.GetComponent<Collider2D>();
                    Debug.Log($"- {wall.name} (위치: {wall.transform.position}, Collider2D 활성화: {collider.enabled}, IsTrigger: {collider.isTrigger})");
                }
                Debug.Log("위 오브젝트들을 확인하고 필요시 Is Trigger로 설정하거나 Collider2D를 비활성화하세요.");
            }
        }
        report += "\n";
        
        // Visual Scripting 확인
        report += "=== Visual Scripting 확인 ===\n";
        report += "플레이어 움직임 Visual Scripting은 'Ground' 태그를 체크합니다.\n";
        report += "바닥 이름이 '바닥(챕터1)' 등으로 변경되어도 태그가 'Ground'이면 인식됩니다.\n\n";
        
        Debug.Log(report);
        EditorUtility.DisplayDialog("진단 완료", report, "확인");
    }
}
