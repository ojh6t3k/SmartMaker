using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace SmartMaker
{    
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("SmartMaker/Unity3D/UI/UiJoystick")]
    public class UiJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform handle;
        public bool interactable = true;
        public Vector2 minArea = -Vector2.one;
        public Vector2 maxArea = Vector2.one;

        public UnityEvent OnDragStart;
        public UnityEvent OnDrag;
        public UnityEvent OnDragEnd;

        private RectTransform _rectTransform;
        private bool _drag = false;
        private Vector2 _axis = Vector2.zero;

        // Use this for initialization
        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            _drag = true;
            OnDragStart.Invoke();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            Vector3 pos3;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out pos3))
            {
                handle.position = pos3;

                Vector2 pos2 = handle.anchoredPosition;                

                Vector2 min = minArea * _rectTransform.rect.width;
                Vector2 max = maxArea * _rectTransform.rect.height;
                pos2.x = Mathf.Clamp(pos2.x, min.x, max.x);
                pos2.y = Mathf.Clamp(pos2.y, min.y, max.y);

                float radius = _rectTransform.rect.width * 0.5f;
                float radius2 = _rectTransform.rect.height * 0.5f;
                if(radius == radius2)
                {
                    if (pos2.magnitude > radius)
                        pos2 = pos2.normalized * radius;
                }

                _axis.x = Mathf.Clamp(pos2.x / radius, -1f, 1f);
                _axis.y = Mathf.Clamp(pos2.y / radius2, -1f, 1f);

                handle.anchoredPosition = pos2;
            }
            OnDrag.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            handle.anchoredPosition = Vector2.zero;
            _axis = Vector3.zero;
            _drag = false;
            OnDragEnd.Invoke();
        }

        public float HorizontalAxis
        {
            get
            {
                return _axis.x;
            }
        }

        public float VerticalAxis
        {
            get
            {
                return _axis.y;
            }
        }

        public Vector2 Axis
        {
            get
            {
                return _axis;
            }
        }
    }
}
