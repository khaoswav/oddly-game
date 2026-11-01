using UnityEngine;

namespace Oddly
{
    public sealed class InteractionController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float reach = 4.5f;
        [SerializeField] private LayerMask interactionMask = ~0;
        [SerializeField] private float minimumHoldDistance = 1.2f;
        [SerializeField] private float maximumHoldDistance = 7f;
        [SerializeField] private float holdDistanceStep = 0.85f;

        private PerspectiveObject heldObject;
        private DialogueDirector dialogue;
        private BehaviorTracker behavior;
        private float currentHoldDistance = 1.8f;

        private void Start()
        {
            dialogue = FindObjectOfType<DialogueDirector>();
            behavior = FindObjectOfType<BehaviorTracker>();
            if (holdPoint != null)
                currentHoldDistance = Mathf.Clamp(holdPoint.localPosition.z,
                    minimumHoldDistance, maximumHoldDistance);
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            if (heldObject != null)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    currentHoldDistance = Mathf.Clamp(
                        currentHoldDistance + scroll * holdDistanceStep,
                        minimumHoldDistance, maximumHoldDistance);
                    heldObject.UpdateHeldPerspective(currentHoldDistance);
                }

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
                    MetaPuzzleNode metaNode = hit.collider.GetComponentInParent<MetaPuzzleNode>();
                    if (metaNode != null)
                    {
                        metaNode.Interact();
                    }
                    else
                    {
                        PerspectiveObject target = hit.collider.GetComponentInParent<PerspectiveObject>();
                        if (target != null)
                        {
                            heldObject = target;
                            currentHoldDistance = Mathf.Clamp(holdPoint.localPosition.z,
                                minimumHoldDistance, maximumHoldDistance);
                            heldObject.PickUp(holdPoint, playerCamera);
                            behavior?.RegisterPickup();
                            if (heldObject.LastPerspectiveScaleFactor < 0.82f ||
                                heldObject.LastPerspectiveScaleFactor > 1.18f)
                                dialogue?.Say("perspective_grab_scaled");
                            else
                                dialogue?.Say("picked_up");
                        }
                        else
                        {
                            DoorController door = hit.collider.GetComponentInParent<DoorController>();
                            if (door != null) door.TryOpen();
                            else behavior?.RegisterFailedInteraction();
                        }
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
            if (heldObject != null) return "SCROLL  MOVE IN / OUT     CLICK  DROP";
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
                out RaycastHit hit, reach, interactionMask))
            {
                MetaPuzzleNode metaNode = hit.collider.GetComponentInParent<MetaPuzzleNode>();
                if (metaNode != null) return metaNode.GetPrompt();
                if (hit.collider.GetComponentInParent<PerspectiveObject>() != null) return "E  PICK UP";
                if (hit.collider.GetComponentInParent<DoorController>() != null) return "E  TRY THE DOOR";
            }
            return "WASD  MOVE     MOUSE  LOOK";
        }
    }
}
