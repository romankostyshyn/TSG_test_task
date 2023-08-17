using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools.ScrollComponent
{
    [RequireComponent(typeof(RectTransform))]
    public class ScrollComponent : RectComponent
    {
        //public int CountInCluster => Mathf.RoundToInt(Width - Offset.Width + Distance.x) / Mathf.RoundToInt(ItemPrefab.Width + Distance.x);

#pragma warning disable

        [SerializeField] private Vector2 distance;
        [SerializeField] private int countInCluster;
        [SerializeField] private Slider slider;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Transform parentTransformForList;
        [SerializeField] private DragComponent dragComponent;

#pragma warning restore

        private Vector2 ItemSize = new Vector2(550, 50); //need to set to every item

        private float VirtualPosition
        {
            get => virtualPosition;
            set
            {
                virtualPosition = value;
                virtualPosition = Mathf.Clamp(virtualPosition, 0, MaxVirtualPosition);
                if (MaxVirtualPosition != 0)
                {
                    //slider.value = virtualPosition / MaxVirtualPosition;
                    scrollbar.value = virtualPosition / MaxVirtualPosition;
                    scrollbar.size = Size.y / (Size.y + maxVirtualPosition);
                }
            }
        }

        private float MaxVirtualPosition
        {
            get => maxVirtualPosition;
            set
            {
                maxVirtualPosition = value;
                maxVirtualPosition = Mathf.Clamp(maxVirtualPosition, 0, maxVirtualPosition);
            }
        }

        private float maxVirtualPosition = 1;

        private float virtualPosition;

        private List<ScrollItem> items;

        #region Unity functions

        private void Awake()
        {
            InitDrag();
            InitGrab();
        }

        #endregion

        public void OnValueSliderChanged(float value)
        {
            VirtualPosition = MaxVirtualPosition * value;
            Refresh();
        }

        public void InitWith(ScrollItem prefab, int itemsCount)
        {
            RecalculateMaxVirtualPosition(itemsCount);
            GenerageScrollItemsFromPrefab(prefab);
            VirtualPosition = 0;
            Refresh();
        }

        private void RecalculateMaxVirtualPosition(int itemsCount)
        {
            int maxCountClasters = itemsCount / countInCluster + 1;
            MaxVirtualPosition = maxCountClasters * ItemSize.y - Height + (maxCountClasters - 1) * distance.y;
            MaxVirtualPosition = Mathf.Max(0, MaxVirtualPosition);
        }

        private void InitGrab()
        {
            dragComponent.OnTryGrab += (data, pos) =>
            {
                int startIndex = (int)virtualPosition / (int)(ItemSize.y + distance.y) * countInCluster;
                int index = GetIndexFromPos(pos);
                items[index].OnGrab(startIndex + index);
            };
        }

        private int GetIndexFromPos(Vector2 pos)
        {
            int index = 0;
            float firstPos = (int)virtualPosition % (int)(ItemSize.y + distance.y);
            int x = (int)pos.x / (int)ItemSize.x;
            int y = (int)(Screen.height - pos.y - dragComponent.OffsetGlobalTop + firstPos) / (int)(ItemSize.y + distance.y);
            index = x + y * countInCluster;
            return index;
        }

        private void InitDrag()
        {
            dragComponent.OnDragEvent += (data, f) => { VirtualPosition += data.delta.y; };
        }

        private void GenerageScrollItemsFromPrefab(ScrollItem prefab)
        {
            //minimal integer +1 to cover all visible and +1 to make virtualizing
            int countOfClusters = Mathf.FloorToInt(Height / (ItemSize.y + distance.y)) + 2;

            //destroy if have old data
            ClearItems();

            //create needable count of clusters
            for (int i = 0; i < countOfClusters; i++)
            {
                for (int j = 0; j < countInCluster; j++)
                {
                    //make item
                    ScrollItem item = Instantiate(prefab, parentTransformForList);
                    item.Size = new Vector2(ItemSize.x, ItemSize.y);
                    items.Add(item);
                }
            }
        }

        private void ClearItems()
        {
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Destroy(items[i].gameObject);
                }
            }

            items = new List<ScrollItem>();
        }

        private void Refresh()
        {
            //get current virtual position
            int startIndex = (int)virtualPosition / (int)(ItemSize.y + distance.y) * countInCluster;
            float firstPos = (int)virtualPosition % (int)(ItemSize.y + distance.y);

            int clusterCounter = 0;
            int counter = 0;
            for (int i = 0; i < items.Count; i++)
            {
                ScrollItem scrollItem = items[i];
                scrollItem.Position = new Vector2(counter * (ItemSize.x + distance.x), -clusterCounter * (ItemSize.y + distance.y) + firstPos);
                scrollItem.Refresh(i + startIndex);
                counter++;
                if (counter >= countInCluster)
                {
                    counter = 0;
                    clusterCounter++;
                }
            }
        }
    }
}
