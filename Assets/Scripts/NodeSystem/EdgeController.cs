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

        void Awake()
        {
            _lineRenderer = GetComponent<UILineRenderer>();
        }

        public void Initialize(RectTransform startRect, RectTransform targetRect, RectTransform canvasRect)
        {
            _startRect = startRect;
            _targetRect = targetRect;
            _canvasRect = canvasRect;
        }

        void LateUpdate()
        {
            UpdateLinePosition();
        }

        void UpdateLinePosition()
        {
            if (_lineRenderer == null || _startRect == null || _targetRect == null || _canvasRect == null)
                return;

            if (!_lineRenderer.enabled)
                return;

            Vector2 startPos = GetLocalPosition(_startRect);
            Vector2 endPos = GetLocalPosition(_targetRect);

            _lineRenderer.points[0] = startPos;
            _lineRenderer.points[1] = endPos;
            _lineRenderer.SetVerticesDirty();
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
            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = visible;
            }
        }

        public void SetColor(Color color)
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.color = color;
                _lineRenderer.SetVerticesDirty();
            }
        }

        public void SetThickness(float thickness)
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.thickness = thickness;
                _lineRenderer.SetVerticesDirty();
            }
        }
    }
}
