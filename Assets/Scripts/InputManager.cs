using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Handles player input using the new Input System. 
/// Also handles key binding and cursor locking.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance {  get; private set; }

    private PlayerInputActions actions;

    //Sprint
    private bool localSprint = false;
    private float sprintTimer = 0f;
    private const float sprintDelay = 0.15f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        actions = new PlayerInputActions();
        actions.Enable();

        BindInputEvents();
    }

    private void OnDestroy()
    {
        UnbindInputEvents();
    }

    private void BindInputEvents()
    {
        actions.Player.Pause.performed += Pause_Performed;
    }

    private void UnbindInputEvents()
    {
        actions.Player.Pause.performed -= Pause_Performed;
    } 

    private void Pause_Performed(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePauseGame();
    }

    private void Update()
    {
        SprintTimer();
    }

    private void SprintTimer()
    {
        if (actions.Player.Sprint.IsInProgress())
        {
            sprintTimer += Time.deltaTime;
            if (sprintTimer >= sprintDelay)
            {
                localSprint = true;
            }
        }
        else
        {
            sprintTimer = 0f;
            localSprint = false;
        }
    }

    public static void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    public Vector2 GetMovementInput() => actions.Player.Move.ReadValue<Vector2>();

    public Vector2 GetMouseDelta() => actions.Player.Look.ReadValue<Vector2>();

    public bool GetJumpInput() => actions.Player.Jump.WasPressedThisFrame();

    public bool GetCrouchInput() => actions.Player.Crouch.WasPressedThisFrame();

    public bool GetSprintInput() => localSprint;

    public bool InteractKeyPressed() => actions.Player.Interact.WasPressedThisFrame();

    public bool DropKeyPressed() => actions.Player.Drop.WasPressedThisFrame();

    /// <summary>Sol tık bu frame basıldı mı? (Kullanımı başlat)</summary>
    public bool GetUseInputDown()
        => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

    /// <summary>Sol tık bu frame bırakıldı mı? (Kullanımı durdur)</summary>
    public bool GetUseInputUp()
        => Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

    /// <summary>Sol tık şu an basılı tutuluyor mu?</summary>
    public bool GetUseInput()
        => Mouse.current != null && Mouse.current.leftButton.isPressed;

}
