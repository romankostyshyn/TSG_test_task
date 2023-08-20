using TSG.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI.PlayerActionButtons
{
    public class ShootButton : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Overlay overlay;
        private bool isPressed;

        public void OnUpdateSelected(BaseEventData data)
        {
            if (isPressed)
            {
                Interact();
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            isPressed = true;
        }

        public void OnPointerUp(PointerEventData data)
        {
            isPressed = false;
        }

        private void Interact()
        {
            overlay.Shoot();
        }
    }
}