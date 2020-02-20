using UnityEngine;
using UnityEngine.UI;

namespace Common.TutorialManager
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(RawImage))]
	public class TutorialLocker : MonoBehaviour
	{
		private int _lockId;

		private void Awake()
		{
			var canvas = GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 3000;

			var canvasScaler = GetComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

			var graphicRaycaster = GetComponent<GraphicRaycaster>();
			graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.All;

			var rawImage = GetComponent<RawImage>();
			rawImage.color = Color.clear;
		}

		private void Start()
		{
			_lockId = TouchHelper.TouchHelper.Lock();
		}

		private void OnDestroy()
		{
			TouchHelper.TouchHelper.Unlock(_lockId);
		}
	}
}