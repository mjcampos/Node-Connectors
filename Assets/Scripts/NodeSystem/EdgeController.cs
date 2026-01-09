using UnityEngine;
using Plugins.Radishmouse;

namespace NodeSystem
{
    [RequireComponent(typeof(UILineRenderer))]
    public class EdgeController : MonoBehaviour
    {
        RectTransform _startRect;
        RectTransform _targetRect;
        RectTransform _canvasRect;
        UILineRenderer _lineRenderer;
        Material _materialInstance;

        public RectTransform TargetRect => _targetRect;
        public RectTransform StartRect => _startRect;

        UILineRenderer LineRenderer
        {
            get
            {
                if (_lineRenderer == null)
                {
                    _lineRenderer = GetComponent<UILineRenderer>();
                }
                return _lineRenderer;
            }
        }

        void Awake()
        {
            _lineRenderer = GetComponent<UILineRenderer>();
        }

        void OnDestroy()
        {
            if (_materialInstance != null)
            {
                Destroy(_materialInstance);
            }
        }

        public void Initialize(RectTransform startRect, RectTransform targetRect, RectTransform canvasRect)
        {
            _startRect = startRect;
            _targetRect = targetRect;
            _canvasRect = canvasRect;
            
            if (LineRenderer != null && LineRenderer.material != null)
            {
                _materialInstance = new Material(LineRenderer.material);
                LineRenderer.material = _materialInstance;
            }
            
            UpdateLinePosition();
        }

        void LateUpdate()
        {
            UpdateLinePosition();
        }

        void UpdateLinePosition()
        {
            if (LineRenderer == null)
            {
                return;
            }
            
            if (_startRect == null || _targetRect == null || _canvasRect == null)
            {
                return;
            }

            if (!LineRenderer.enabled)
                return;

            if (LineRenderer.points == null || LineRenderer.points.Length < 2)
            {
                LineRenderer.points = new Vector2[2];
            }

            Vector2 startPos = GetLocalPosition(_startRect);
            Vector2 endPos = GetLocalPosition(_targetRect);

            LineRenderer.points[0] = startPos;
            LineRenderer.points[1] = endPos;
            LineRenderer.SetVerticesDirty();
        }

        Vector2 GetLocalPosition(RectTransform rectTransform)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                RectTransformUtility.WorldToScreenPoint(null, rectTransform.position),
                null,
                out localPos
            );
            return localPos;
        }

        public void SetVisibility(bool visible)
        {
            if (LineRenderer != null)
            {
                LineRenderer.enabled = visible;
            }
        }

        public void SetAlpha(float alpha)
        {
            if (LineRenderer != null)
            {
                Color color = LineRenderer.color;
                color.a = alpha;
                LineRenderer.color = color;
                LineRenderer.SetVerticesDirty();
            }
        }

        public void SetColor(Color color)
        {
            if (LineRenderer != null)
            {
                LineRenderer.color = color;
                LineRenderer.SetVerticesDirty();
            }
        }

        public void SetThickness(float thickness)
        {
            if (LineRenderer != null)
            {
                LineRenderer.thickness = thickness;
                LineRenderer.SetVerticesDirty();
            }
        }
    }
}
