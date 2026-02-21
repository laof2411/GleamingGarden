using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Board boardReference;
    [SerializeField] private LevelManager levelManagerReference;

    private InputAction mouseAction;
    private ActionInput actionInput;
    
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
        if (levelManagerReference.isGameEnded)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction,Mathf.Infinity, LayerMask.GetMask("Switchable"));
        
        if (hit.collider != null)
        {
            if (boardReference.isProcessingMove)
            {
                return;
            }
            ISwitchable switchable = hit.collider.gameObject.GetComponent<ISwitchable>();
            boardReference.SelectSwitchable(switchable);
        }
    }
}
