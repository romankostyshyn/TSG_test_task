using TSG.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI.PlayerActionButtons
{
    public class MoveLeftButton : MonoBehaviour, IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TopBar topBar;
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

        public void Interact()
        {
            topBar.MoveLeft();
        }
    }
}