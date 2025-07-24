using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Disney.ClubPenguin.CPModuleUtils
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectSnapTo : MonoBehaviour, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		public enum HorizontalSnapPosition
		{
			left,
			right,
			center
		}

		public enum VerticalSnapPosition
		{
			top,
			bottom,
			center
		}

		public delegate void ElementSelectedDelegate(GameObject gameObject);

		private const float MIN_SPEED = 0.01f;

		private const float MIN_SNAP_SPEED = 0.1f;

		public ElementSelectedDelegate ElementSelected;

		public Action StartedScrolling;

		public HorizontalSnapPosition HorizontalSnapPos = HorizontalSnapPosition.center;

		public VerticalSnapPosition VerticalSnapPos = VerticalSnapPosition.center;

		public float SnapAtSpeed = 100f;

		public float SnapTweenTimeSec = 0.5f;

		private bool snapped = true;

		private bool isDragging;

		private bool isTweeningToSnapPoint;

		private ScrollRect scrollRect;

		private Vector2 snapPoint;

		private Vector2 tweenDirection = default(Vector2);

		private Vector3 tweenPositionHelper = default(Vector3);

		private float tweenTimeElapsedSec;

		private float SnapTweenTimeSecRecip = 1f;

		private int previousNumContentChildren;

		private bool disableTheScroll;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			snapped = false;
			isTweeningToSnapPoint = false;
		}

		public void DisableScroll()
		{
			disableTheScroll = true;
		}

		public void EnableScroll()
		{
			disableTheScroll = false;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!disableTheScroll)
			{
				isDragging = true;
				snapped = false;
				isTweeningToSnapPoint = false;
				if (StartedScrolling != null)
				{
					StartedScrolling();
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!disableTheScroll)
			{
				isDragging = false;
			}
		}

		private bool NumContentChildrenChanged()
		{
			if (previousNumContentChildren != scrollRect.content.childCount)
			{
				previousNumContentChildren = scrollRect.content.childCount;
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (disableTheScroll || scrollRect.content == null || scrollRect.content.childCount == 0)
			{
				return;
			}
			RestrictBounds();
			if (isDragging)
			{
				return;
			}
			if (NumContentChildrenChanged())
			{
				DetermineSnapToElementAndStartTween();
				snapped = true;
			}
			else if (isTweeningToSnapPoint)
			{
				TweenToSnapPoint();
			}
			else
			{
				if (snapped)
				{
					return;
				}
				float magnitude = scrollRect.velocity.magnitude;
				if (magnitude <= SnapAtSpeed)
				{
					if (SnapAtSpeed < 0.1f)
					{
						SnapAtSpeed = 0.1f;
					}
					snapped = true;
					DetermineSnapToElementAndStartTween();
				}
			}
		}

		private void DetermineSnapToElementAndStartTween()
		{
			snapPoint = GetSnapPointInScrollRectSpace();
			RectTransform rectTransform = null;
			float num = float.MaxValue;
			Vector2 contentChildToSnapPoint = default(Vector2);
			Vector2 vector = default(Vector2);
			for (int i = 0; i < scrollRect.content.childCount; i++)
			{
				RectTransform rectTransform2 = scrollRect.content.GetChild(i) as RectTransform;
				Vector3 vector2 = scrollRect.content.localPosition + rectTransform2.localPosition;
				vector.x = snapPoint.x - vector2.x;
				vector.y = snapPoint.y - vector2.y;
				float magnitude = vector.magnitude;
				if (rectTransform == null || magnitude < num)
				{
					num = magnitude;
					contentChildToSnapPoint.Set(vector.x, vector.y);
					rectTransform = rectTransform2;
				}
			}
			UpdateTweenToContentChild(rectTransform, contentChildToSnapPoint);
		}

		public void TweenToContentChildWithIndex(int index)
		{
			if (!disableTheScroll && !(scrollRect.content == null) && scrollRect.content.childCount > index)
			{
				snapPoint = GetSnapPointInScrollRectSpace();
				RectTransform rectTransform = scrollRect.content.GetChild(index) as RectTransform;
				Vector3 vector = scrollRect.content.localPosition + rectTransform.localPosition;
				Vector2 contentChildToSnapPoint = default(Vector2);
				contentChildToSnapPoint.x = snapPoint.x - vector.x;
				contentChildToSnapPoint.y = snapPoint.y - vector.y;
				UpdateTweenToContentChild(rectTransform, contentChildToSnapPoint);
			}
		}

		private void UpdateTweenToContentChild(RectTransform contentChild, Vector2 contentChildToSnapPoint)
		{
			float num = 0f;
			if (HorizontalSnapPos != HorizontalSnapPosition.center)
			{
				num = ((HorizontalSnapPos != 0) ? ((0f - contentChild.rect.width) * 0.5f) : (contentChild.rect.width * 0.5f));
			}
			float num2 = 0f;
			if (VerticalSnapPos != VerticalSnapPosition.center)
			{
				num2 = ((VerticalSnapPos != 0) ? ((0f - contentChild.rect.height) * 0.5f) : (contentChild.rect.height * 0.5f));
			}
			scrollRect.StopMovement();
			tweenDirection.x = contentChildToSnapPoint.x + num;
			tweenDirection.y = contentChildToSnapPoint.y + num2;
			ResetTweenData();
			isTweeningToSnapPoint = true;
			if (ElementSelected != null)
			{
				ElementSelected(contentChild.gameObject);
			}
		}

		private void ResetTweenData()
		{
			tweenTimeElapsedSec = 0f;
			SnapTweenTimeSecRecip = 1f / SnapTweenTimeSec;
		}

		private void TweenToSnapPoint()
		{
			if (tweenTimeElapsedSec >= SnapTweenTimeSec)
			{
				isTweeningToSnapPoint = false;
				return;
			}
			float num = (!(tweenTimeElapsedSec + Time.deltaTime <= SnapTweenTimeSec)) ? (SnapTweenTimeSec - tweenTimeElapsedSec) : Time.deltaTime;
			tweenTimeElapsedSec += num;
			float num2 = num * SnapTweenTimeSecRecip;
			Vector3 localPosition = scrollRect.content.localPosition;
			float new_x = localPosition.x + tweenDirection.x * num2;
			Vector3 localPosition2 = scrollRect.content.localPosition;
			float new_y = localPosition2.y + tweenDirection.y * num2;
			Vector3 localPosition3 = scrollRect.content.localPosition;
			tweenPositionHelper.Set(new_x, new_y, localPosition3.z);
			scrollRect.content.localPosition = tweenPositionHelper;
		}

		private Vector2 GetSnapPointInScrollRectSpace()
		{
			Vector2 result = default(Vector2);
			if (HorizontalSnapPos == HorizontalSnapPosition.center)
			{
				result.x = 0f;
			}
			else
			{
				result.x = ((HorizontalSnapPos != 0) ? (scrollRect.GetComponent<RectTransform>().rect.width * 0.5f) : ((0f - scrollRect.GetComponent<RectTransform>().rect.width) * 0.5f));
			}
			if (VerticalSnapPos == VerticalSnapPosition.center)
			{
				result.y = 0f;
			}
			else
			{
				result.y = ((VerticalSnapPos != 0) ? (scrollRect.GetComponent<RectTransform>().rect.height * 0.5f) : ((0f - scrollRect.GetComponent<RectTransform>().rect.height) * 0.5f));
			}
			return result;
		}

		private void RestrictBounds()
		{
			Vector3 localPosition = scrollRect.content.localPosition;
			float x = localPosition.x;
			float y = localPosition.y;
			bool flag = false;
			float num = scrollRect.content.rect.width * 0.5f;
			float num2 = scrollRect.content.rect.height * 0.5f;
			Vector2 snapPointInScrollRectSpace = GetSnapPointInScrollRectSpace();
			if (localPosition.x > snapPointInScrollRectSpace.x + num)
			{
				x = snapPointInScrollRectSpace.x + num;
				flag = true;
			}
			else if (localPosition.x < snapPointInScrollRectSpace.x - num)
			{
				x = snapPointInScrollRectSpace.x - num;
				flag = true;
			}
			if (localPosition.y > snapPointInScrollRectSpace.y + num2)
			{
				y = snapPointInScrollRectSpace.y + num2;
				flag = true;
			}
			else if (localPosition.y < snapPointInScrollRectSpace.y - num2)
			{
				y = snapPointInScrollRectSpace.y - num2;
				flag = true;
			}
			if (flag)
			{
				scrollRect.StopMovement();
				scrollRect.content.localPosition = new Vector3(x, y, localPosition.z);
			}
		}
	}
}
