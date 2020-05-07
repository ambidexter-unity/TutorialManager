using System;
using Common.Activatable;
using Common.TutorialManager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Sample
{
	[DisallowMultipleComponent]
	public class ButtonController : MonoBehaviour, ITutorialPage
	{
#pragma warning disable 649
		[Inject] private readonly DiContainer _container;
		[Inject] private readonly ITutorialManager _tutorialManager;
#pragma warning restore 649

		// ITutorialPage

		public string Id => "test_button_tutorial";

		public bool InstantiatePage(Transform pageContainer, int pageCurrentCompleteValue, Action<GameObject> callback)
		{
			PageCurrentCompleteValue = pageCurrentCompleteValue;
			if (pageCurrentCompleteValue >= PageFinalCompleteValue)
			{
				Debug.LogErrorFormat("Trying to instantiate completed page {0}.", Id);
				return false;
			}

			var page = _container.InstantiatePrefabResourceForComponent<ButtonTutorialPage>(
				"ButtonTutorialPage", pageContainer, new object[] {GetComponent<Button>()});
			page.ActivatableStateChangedEvent += PageOnActivatableStateChangedEvent;
			page.Activate();
			callback?.Invoke(page.gameObject);
			return true;
		}

		private void PageOnActivatableStateChangedEvent(IActivatable activatable, ActivatableState state)
		{
			if (state != ActivatableState.Inactive) return;

			activatable.ActivatableStateChangedEvent -= PageOnActivatableStateChangedEvent;
			PageCurrentCompleteValue += 1;
			CompleteTutorialPageEvent?.Invoke(PageCurrentCompleteValue);
		}

		public event Action<int> CompleteTutorialPageEvent;

		public event Action<bool> TerminateTutorialPageEvent;

		public int PageFinalCompleteValue => 1;

		public int PageCurrentCompleteValue { get; private set; }

		// \ITutorialPage

		private void Start()
		{
			if (_tutorialManager.SetCurrentPage(this))
			{
			}
		}
	}
}