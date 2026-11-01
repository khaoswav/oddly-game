using UnityEngine;

namespace Oddly
{
    public sealed class InteractionController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float reach = 4.5f;
        [SerializeField] private LayerMask interactionMask = ~0;

        private PerspectiveObject heldObject;
        private DialogueDirector dialogue;
        private BehaviorTracker behavior;

        private void Start()
        {
            dialogue = FindObjectOfType<DialogueDirector>();
            behavior = FindObjectOfType<BehaviorTracker>();
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            if (heldObject != null)
            {
                heldObject.Resize(Input.mouseScrollDelta.y);
                if (Input.GetMouseButtonDown(0))
                {
                    heldObject.Drop();
                    heldObject = null;
                    behavior?.RegisterDrop();
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
                    out RaycastHit hit, reach, interactionMask))
                {
                    PerspectiveObject target = hit.collider.GetComponentInParent<PerspectiveObject>();
                    if (target != null)
                    {
                        heldObject = target;
                        heldObject.PickUp(holdPoint);
                        behavior?.RegisterPickup();
                        dialogue?.Say("picked_up");
                    }
                    else
                    {
                        DoorController door = hit.collider.GetComponentInParent<DoorController>();
                        if (door != null) door.TryOpen();
                        else behavior?.RegisterFailedInteraction();
                    }
                }
                else
                {
                    behavior?.RegisterFailedInteraction();
                }
            }
        }

        public string GetPrompt()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return "CLICK  RESUME";
            if (heldObject != null) return "SCROLL  RESIZE     CLICK  DROP";
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
                out RaycastHit hit, reach, interactionMask))
            {
                if (hit.collider.GetComponentInParent<PerspectiveObject>() != null) return "E  PICK UP";
                if (hit.collider.GetComponentInParent<DoorController>() != null) return "E  TRY THE DOOR";
            }
            return "WASD  MOVE     MOUSE  LOOK";
        }
    }
}
