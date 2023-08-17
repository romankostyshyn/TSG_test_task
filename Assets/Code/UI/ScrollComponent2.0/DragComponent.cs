using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tools.ScrollComponent
{
    public class DragComponent : RectComponent, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData, float> OnDragEvent;
        public event Action<PointerEventData, Vector2> OnBeginDragEvent;
        public event Action<PointerEventData, float> OnEndDragEvent;
        public event Action<PointerEventData, Vector2> OnTryGrab; // попытка взять элемент

        private AverageVector2 averageImpulse = new AverageVector2();
        private Vector2 startPoint;
        private Vector2 startPointGrab = Vector2.zero;
        private float xTreshhold = 5;

        public Vector2 StartPoint => startPoint;

        public void OnDrag(PointerEventData eventData)
        {
            if (startPointGrab == Vector2.zero)
            {
                averageImpulse.AddNext(eventData.delta);
                OnDragEvent?.Invoke(eventData, averageImpulse.Average.y);
                if (Mathf.Abs(averageImpulse.Average.x) > Mathf.Abs(averageImpulse.Average.y * 2) && averageImpulse.Average.x > xTreshhold)
                {
                    startPointGrab = eventData.position;
                    OnTryGrab?.Invoke(eventData, startPointGrab);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPoint = eventData.position;
            averageImpulse.Clear();
            startPointGrab = Vector2.zero;
            OnBeginDragEvent?.Invoke(eventData, startPoint);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            averageImpulse.AddNext(eventData.delta);
            OnEndDragEvent?.Invoke(eventData, averageImpulse.Average.y);
        }
    }
}
