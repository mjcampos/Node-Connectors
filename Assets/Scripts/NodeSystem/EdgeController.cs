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

        public RectTransform TargetRect => _targetRect;

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

        public void Initialize(RectTransform startRect, RectTransform targetRect, RectTransform canvasRect)
        {
            _startRect = startRect;
            _targetRect = targetRect;
            _canvasRect = canvasRect;
            
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
                Debug.LogError("[EdgeController] UILineRenderer component not found!");
                return;
            }
            
            if (_startRect == null || _targetRect == null || _canvasRect == null)
            {
                return;
            }

            if (!LineRenderer.enabled)
                return;

            Vector2 startPos = GetLocalPosition(_startRect);
            Vector2 endPos = GetLocalPosition(_targetRect);

            if (LineRenderer.points == null || LineRenderer.points.Length < 2)
            {
                LineRenderer.points = new Vector2[2];
            }

            LineRenderer.points[0] = Vector2.zero;
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
