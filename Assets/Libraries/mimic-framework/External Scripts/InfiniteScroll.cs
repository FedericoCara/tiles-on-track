/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/81/infinite-scrollrect
/// Demo - https://www.youtube.com/watch?v=uVTV7Udx78k  - configures automatically.  - works in both vertical and horizontal (but not both at the same time)  - drag and drop  - can be initialized by code (in case you populate your scrollview content from code)
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Infinite scroll view with automatic configuration 
    /// 
    /// Fields
    /// - InitByUSer - in case your scrollrect is populated from code, you can explicitly Initialize the infinite scroll after your scroll is ready
    /// by callin Init() method
    /// 
    /// Notes
    /// - doesn't work in both vertical and horizontal orientation at the same time.
    /// - in order to work it disables layout components and size fitter if present(automatically)
    /// 
    /// </summary>
    [AddComponentMenu("UI/Extensions/UI Infinite Scroll")]
    public class InfiniteScroll : MonoBehaviour
    {
        //if true user will need to call Init() method manually (in case the contend of the scrollview is generated from code or requires special initialization)
        [Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
        public bool InitByUser = false;
        private ScrollRect _scrollRect;
        private ContentSizeFitter _contentSizeFitter;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private HorizontalLayoutGroup _horizontalLayoutGroup;
        private GridLayoutGroup _gridLayoutGroup;
        private bool _isVertical = false;
        private bool _isHorizontal = false;
        private float _disableMarginX = 0;
        private float _disableMarginY = 0;
        private bool _hasDisabledGridComponents = false;
        private Vector2 _newAnchoredPosition = Vector2.zero;
        //TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
        private float _treshold = 100f;
        private int ItemCount {
            get{ return _scrollRect.content.childCount; }
        }
        private float _recordOffsetX = 0;
        private float _recordOffsetY = 0;

        bool AreGridComponentsEnabled {
            get{ return !_hasDisabledGridComponents; }
            set{               
                if (_verticalLayoutGroup) {
                    _verticalLayoutGroup.enabled = value;
                }
                if (_horizontalLayoutGroup) {
                    _horizontalLayoutGroup.enabled = value;
                }
                if (_contentSizeFitter) {
                    _contentSizeFitter.enabled = value;
                }
                if (_gridLayoutGroup) {
                    _gridLayoutGroup.enabled = value;
                }
                _hasDisabledGridComponents = !value;
            }
        }

        void Awake()
        {
            if (!InitByUser)
                Init();
        }

        public void Init()
        {
            if (GetComponent<ScrollRect>() != null)
            {
                _scrollRect = GetComponent<ScrollRect>();
                _scrollRect.onValueChanged.AddListener(OnScroll);
                _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

                if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
                {
                    _verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
                {
                    _horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
                {
                    _gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
                {
                    _contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
                }

                _isHorizontal = _scrollRect.horizontal;
                _isVertical = _scrollRect.vertical;

                if (_isHorizontal && _isVertical)
                {
                    Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, plase choose one direction (horizontal or vertical)");
                }
            }
            else
            {
                Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
            }
        }

        public void OnContentChanged() {
            AreGridComponentsEnabled = true;
            // yield return new WaitForEndOfFrame();
            // DisableGridComponents();
        }

        void DisableGridComponents()
        {
            if (_isVertical) {
                _recordOffsetY = GetItem(0).anchoredPosition.y - GetItem(1).anchoredPosition.y;
                _disableMarginY = _recordOffsetY * ItemCount / 2;// _scrollRect.GetComponent<RectTransform>().rect.height/2 + items[0].sizeDelta.y;
            }
            if (_isHorizontal) {
                _recordOffsetX = GetItem(1).anchoredPosition.x - GetItem(0).anchoredPosition.x;
                _disableMarginX = _recordOffsetX * ItemCount / 2;//_scrollRect.GetComponent<RectTransform>().rect.width/2 + items[0].sizeDelta.x;
            }
            AreGridComponentsEnabled = false;
        }

        private RectTransform GetItem(int index) {
            return _scrollRect.content.GetChild(index).GetComponent<RectTransform>();
        }

        public void AutomaticScroll() {
            OnScroll(_scrollRect.normalizedPosition);
        }

        
        public void OnScroll(Vector2 pos)
        {
            if (!_hasDisabledGridComponents)
                DisableGridComponents();

            RectTransform item;
            for (int i = 0; i < ItemCount; i++) {
                item = GetItem(i);
                if (_isHorizontal) {
                    if (_scrollRect.transform.InverseTransformPoint(item.gameObject.transform.position).x > _disableMarginX + _treshold) {
                        _newAnchoredPosition = item.anchoredPosition;
                        _newAnchoredPosition.x -= ItemCount * _recordOffsetX;
                        item.anchoredPosition = _newAnchoredPosition;
                        GetItem(ItemCount - 1).transform.SetAsFirstSibling();
                    } else if (_scrollRect.transform.InverseTransformPoint(item.gameObject.transform.position).x < -_disableMarginX) {
                        _newAnchoredPosition = item.anchoredPosition;
                        _newAnchoredPosition.x += ItemCount * _recordOffsetX;
                        item.anchoredPosition = _newAnchoredPosition;
                        GetItem(0).transform.SetAsLastSibling();
                    }
                }

                if (_isVertical)
                {
                    if (_scrollRect.transform.InverseTransformPoint(item.gameObject.transform.position).y > _disableMarginY + _treshold)
                    {
                        _newAnchoredPosition = item.anchoredPosition;
                        _newAnchoredPosition.y -= ItemCount * _recordOffsetY;
                        item.anchoredPosition = _newAnchoredPosition;
                        GetItem(ItemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (_scrollRect.transform.InverseTransformPoint(item.gameObject.transform.position).y < -_disableMarginY)
                    {
                        _newAnchoredPosition = item.anchoredPosition;
                        _newAnchoredPosition.y += ItemCount * _recordOffsetY;
                        item.anchoredPosition = _newAnchoredPosition;
                        GetItem(0).transform.SetAsLastSibling();
                    }
                }
            }
        }
    }
}