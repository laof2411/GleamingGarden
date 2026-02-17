using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ActionInput actionInput;
    private InputAction mouseAction;

    private void Awake()
    {
        actionInput = new ActionInput();       
    }

    private void OnEnable()
    {
        actionInput.Enable();
        mouseAction = actionInput.Player.Click;
        mouseAction.performed += Click;
    }

    private void OnDisable()
    {
        actionInput.Disable();
    }

    private void Click(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction,Mathf.Infinity, LayerMask.GetMask("Switchable"));
        
        if (hit.collider != null)
        {
            if (Board.instance.isProcessingMove)
            {
                return;
            }
            ISwitchable switchable = hit.collider.gameObject.GetComponent<ISwitchable>();
            Board.instance.SelectSwitchable(switchable);
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }
}
