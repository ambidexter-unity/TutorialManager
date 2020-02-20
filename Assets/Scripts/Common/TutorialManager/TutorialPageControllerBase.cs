using System;
using System.Collections.Generic;
using Common.Activatable;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Common.TutorialManager
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas))]
	public abstract class TutorialPageControllerBase : MonoBehaviour, IActivatable
	{
		protected enum VerticalPointerAlign
		{
			Middle,
			Top,
			Bottom
		}

		protected enum HorizontalPointerAlign
		{
			Center,
			Left,
			Right
		}

		private static int _pointerCtr;
		private readonly Dictionary<int, CanvasGroup> _arrowsMap = new Dictionary<int, CanvasGroup>();

		private readonly Dictionary<string, Object> _loadedAssets = new Dictionary<string, Object>();

		private ActivatableState _activatableState = ActivatableState.Inactive;

#pragma warning disable 649
		[SerializeField] private string _tutorialText;
#pragma warning restore 649

		protected string TutorialText =>
			string.IsNullOrEmpty(_tutorialText) ? "" : _tutorialText;

		// IActivatable

		public ActivatableState ActivatableState
		{
			get => _activatableState;
			private set
			{
				if (value == _activatableState) return;
				_activatableState = value;
				ActivatableStateChangedEvent?.Invoke(this, _activatableState);
			}
		}
		public event ActivatableStateChangedHandler ActivatableStateChangedEvent;

		public void Activate(bool immediately = false)
		{
			if (this.IsInactive())
			{
				ActivatableState = ActivatableState.ToActive;
			}

			InitPage(() => ActivatableState = ActivatableState.Active);
		}

		public void Deactivate(bool immediately = false)
		{
			Assert.IsTrue(this.IsActive());
			ActivatableState = ActivatableState.ToInactive;
			ClosePage(() =>
			{
				ActivatableState = ActivatableState.Inactive;
			});
		}

		// \IActivatable

		/// <summary>
		/// Список необходимых Streaming assets
		/// </summary>
		protected abstract string[] RequiredStreamingAssets { get; }

		protected IReadOnlyDictionary<string, Object> LoadedAssets => _loadedAssets;

		/// <summary>
		/// Метод инициализации страницы
		/// </summary>
		/// <param name="callback">Колбэк, вызываемый по окончании инициализации</param>
		protected abstract void InitPage(Action callback);

		/// <summary>
		/// Метод-деструктор
		/// </summary>
		/// <param name="callback">Коллбэк, вызываемый по окончании закрытия</param>
		protected abstract void ClosePage(Action callback);

		protected int Point(RectTransform @object, GameObject pointerPrefab,
			HorizontalPointerAlign horizontalAlign, VerticalPointerAlign verticalAlign)
		{
			var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform, @object);
			return Point(new Rect {min = bounds.min, max = bounds.max}, pointerPrefab, horizontalAlign, verticalAlign);
		}

		protected int Point(Rect rc, GameObject pointerPrefab,
			HorizontalPointerAlign horizontalAlign, VerticalPointerAlign verticalAlign)
		{
			float xPos, yPos;
			switch (horizontalAlign)
			{
				case HorizontalPointerAlign.Left:
					xPos = rc.xMin;
					break;
				case HorizontalPointerAlign.Right:
					xPos = rc.xMax;
					break;
				default:
					xPos = rc.center.x;
					break;
			}

			switch (verticalAlign)
			{
				case VerticalPointerAlign.Top:
					yPos = rc.yMax;
					break;
				case VerticalPointerAlign.Bottom:
					yPos = rc.yMin;
					break;
				default:
					yPos = rc.center.y;
					break;
			}

			var ptr = Instantiate(pointerPrefab, transform, false).GetOrCreateComponent<CanvasGroup>();
			ptr.GetOrCreateComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

			ptr.alpha = 0;
			ptr.DOFade(1, 0.5f);

			_arrowsMap.Add(++_pointerCtr, ptr);
			return _pointerCtr;
		}

		protected void Unpoint(int pointerId)
		{
			if (!_arrowsMap.TryGetValue(pointerId, out var ptr)) return;

			DOTween.Kill(ptr);
			ptr.DOFade(0, 0.5f).OnComplete(() =>
			{
				_arrowsMap.Remove(pointerId);
				Destroy(ptr.gameObject);
			});
		}

		protected virtual void Start()
		{
			GetComponent<Canvas>().sortingOrder = 3100;
		}

		protected virtual void OnDestroy()
		{
			foreach (var pair in _arrowsMap)
			{
				DOTween.Kill(pair.Value);
			}

			_arrowsMap.Clear();
		}
	}
}