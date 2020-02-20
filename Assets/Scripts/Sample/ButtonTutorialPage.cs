using System;
using Common.TutorialManager;
using DG.Tweening;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
	[RequireComponent(typeof(RawImage))]
	public class ButtonTutorialPage : TutorialPageControllerBase
	{
		protected override string[] RequiredStreamingAssets => new string[0];
		private readonly Lazy<RawImage> _rawImage;
		private Tween _tween;

		public ButtonTutorialPage()
		{
			_rawImage = new Lazy<RawImage>(GetComponent<RawImage>);
		}

		protected override void Start()
		{
			_rawImage.Value.color = new Color(0, 0, 0, 0);
			base.Start();
		}

		protected override void InitPage(Action callback)
		{
			Assert.IsNull(_tween);
			_rawImage.Value.color = new Color(0, 0, 0, 0);
			_tween = _rawImage.Value.DOColor(new Color(0, 0, 0, 0.5f), 1f).SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					_tween = null;
					callback?.Invoke();
				});
		}

		protected override void ClosePage(Action callback)
		{
			Assert.IsNull(_tween);
			_rawImage.Value.color = new Color(0, 0, 0, 0.5f);
			_tween = _rawImage.Value.DOColor(new Color(0, 0, 0, 0), 1f).SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					_tween = null;
					callback?.Invoke();
				});
		}
	}
}