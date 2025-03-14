using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public Action<Vector2> OnMove;
    public Action OnJump;
    public Action OnAttack;
    public Action<bool> OnDefend;
    public Action OnToggleWeapon;
    public Action OnToggleMenu;
    public Action<bool> OnToggleSprint;
    public Action<int> OnNumericButtonPressed;
    private PlayerBindings playerInputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerInputActions = new PlayerBindings();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.Player.Move.performed += ctx => HandleMove(ctx);
        playerInputActions.Player.Move.canceled += ctx => HandleMove(ctx);
        playerInputActions.Player.Defend.performed += ctx => HandleDefend(ctx);
        playerInputActions.Player.Defend.canceled += ctx => HandleDefend(ctx);
        playerInputActions.Player.Jump.performed += ctx => HandleJump();
        playerInputActions.Player.Attack.performed += ctx => HandleAttack();
        playerInputActions.Player.NumericButton.performed += HandleNumericButtonPress;
        playerInputActions.Player.ToggleWeapon.performed += ctx => HandleToggleWeapon();
        playerInputActions.Player.Sprint.performed += ctx => HandleSprint(ctx);
        playerInputActions.Player.Sprint.canceled += ctx => HandleSprint(ctx);
        playerInputActions.Player.Menu.performed += ctx => HandleMenu();
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Move.performed -= ctx => HandleMove(ctx);
            playerInputActions.Player.Move.canceled -= ctx => HandleMove(ctx);
            playerInputActions.Player.Defend.performed -= ctx => HandleDefend(ctx);
            playerInputActions.Player.Defend.canceled -= ctx => HandleDefend(ctx);
            playerInputActions.Player.Jump.performed -= ctx => HandleJump();
            playerInputActions.Player.Attack.performed -= ctx => HandleAttack();
            playerInputActions.Player.NumericButton.performed -= HandleNumericButtonPress;
            playerInputActions.Player.ToggleWeapon.performed -= ctx => HandleToggleWeapon();
            playerInputActions.Player.Sprint.performed -= ctx => HandleSprint(ctx);
            playerInputActions.Player.Sprint.canceled -= ctx => HandleSprint(ctx);
            playerInputActions.Player.Menu.performed -= ctx => HandleMenu();

            playerInputActions.Disable();
        }

    }
    private void HandleMenu()
    {
        OnToggleMenu?.Invoke();
    }
    private void HandleMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }
    private void HandleDefend(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            OnDefend?.Invoke(true);
        }
        else { OnDefend?.Invoke(false); }
    }
    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            OnToggleSprint?.Invoke(true);
        }
        else { OnToggleSprint?.Invoke(false); }
    }
    private void HandleToggleWeapon()
    {
        OnToggleWeapon?.Invoke();
    }

    private void HandleJump()
    {
        OnJump?.Invoke();
    }

    private void HandleAttack()
    {
        OnAttack?.Invoke();
    }

    private void HandleNumericButtonPress(InputAction.CallbackContext context)
    {
        string pressedKey = context.control.displayName;
        int itemNumber = -1;

        if (int.TryParse(pressedKey, out itemNumber))
        {
            if (itemNumber == 0)
                itemNumber = 10;
            else
                itemNumber -= 1;

            OnNumericButtonPressed?.Invoke(itemNumber);
        }
    }

}
