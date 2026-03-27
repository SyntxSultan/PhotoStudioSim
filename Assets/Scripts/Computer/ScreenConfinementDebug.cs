#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenConfinement))]
public class ScreenConfinementDebug : MonoBehaviour
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Canvas monitorCanvas;

    private void Update()
    {
        if (!ComputerState.Instance.IsPlayerSit) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Debug.Log("─────────────────────────────────────");

        // 1) Virtual mouse pozisyonu
        var vm = InputSystem.GetDevice("VirtualMouse") as Mouse;
        if (vm == null) { Debug.LogError("[Debug] VirtualMouse device YOK!"); return; }
        Vector2 vmPos = vm.position.ReadValue();
        Debug.Log($"[Debug] VirtualMouse screen pos: {vmPos}");

        // 2) EventSystem mevcut module
        var es = EventSystem.current;
        if (es == null) { Debug.LogError("[Debug] EventSystem YOK!"); return; }
        var module = es.currentInputModule;
        Debug.Log($"[Debug] InputModule: {module?.GetType().Name ?? "NULL"}");

        // 3) Canvas Event Camera
        if (monitorCanvas == null) { Debug.LogWarning("[Debug] monitorCanvas atanmamış"); }
        else
        {
            Debug.Log($"[Debug] Canvas renderMode: {monitorCanvas.renderMode}");
            Debug.Log($"[Debug] Canvas eventCamera: {(monitorCanvas.worldCamera != null ? monitorCanvas.worldCamera.name : "NULL — ATANMALI!")}");
        }

        // 4) Raycast testi
        var results = new System.Collections.Generic.List<RaycastResult>();
        var fakeEvent = new PointerEventData(es) { position = vmPos };
        es.RaycastAll(fakeEvent, results);

        if (results.Count == 0)
        {
            Debug.LogWarning("[Debug] Raycast HİÇBİR şeye çarpmadı!");

            // Kameradan manuel ray at
            if (renderCamera != null)
            {
                Ray ray = renderCamera.ScreenPointToRay(vmPos);
                Debug.Log($"[Debug] Manuel ray: origin={ray.origin:F2} dir={ray.direction:F2}");
                if (Physics.Raycast(ray, out RaycastHit hit, 20f))
                    Debug.Log($"[Debug] Physics hit: {hit.collider.name} (layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                else
                    Debug.Log("[Debug] Physics ray da hiçbir şeye çarpmadı");
            }
        }
        else
        {
            foreach (var r in results)
                Debug.Log($"[Debug] Raycast hit: {r.gameObject.name} | depth: {r.depth} | distance: {r.distance:F2}");
        }

        // 5) InputSystemUIInputModule pointer device kontrolü
        if (module is InputSystemUIInputModule inputModule)
        {
            Debug.Log($"[Debug] PointerBehavior: {inputModule.pointerBehavior}");
        }

        Debug.Log("─────────────────────────────────────");
    }
}
#endif