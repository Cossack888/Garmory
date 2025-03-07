using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public Action<Vector2> OnMove;
    public Action OnJump;
    public Action OnAttack;
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
        playerInputActions.Player.Jump.performed += ctx => HandleJump();
        playerInputActions.Player.Attack.performed += ctx => HandleAttack();
        playerInputActions.Player.NumericButton.performed += HandleNumericButtonPress;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Move.performed -= ctx => HandleMove(ctx);
        playerInputActions.Player.Jump.performed -= ctx => HandleJump();
        playerInputActions.Player.Attack.performed -= ctx => HandleAttack();
        playerInputActions.Player.NumericButton.performed -= HandleNumericButtonPress;

        playerInputActions.Disable();
    }

    private void HandleMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
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
        else
        {
            Debug.LogWarning("Unhandled key: " + pressedKey);
        }
    }

}
