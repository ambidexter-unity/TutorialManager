using Common.PersistentManager;
using UnityEngine;
using Zenject;

namespace Common.TutorialManager
{
	public class TutorialManager : TutorialManagerBase
	{
		private TutorialLocker _locker;
		private static Transform _pageContentContainer;

#pragma warning disable 649
		[Inject] private readonly DiContainer _container;
		[Inject] private readonly IPersistentManager _persistentManager;
#pragma warning restore 649

		[Inject]
		private void Construct()
		{
			if (_pageContentContainer == null)
			{
				_pageContentContainer = new GameObject("TutorialManager").transform;
				Object.DontDestroyOnLoad(_pageContentContainer);
			}
			else
			{
				Debug.LogError("Tutorial Manager must be injected as Singleton.");
			}
		}

		protected override IPersistentManager PersistentManager => _persistentManager;

		protected override bool InstantiateCurrentPage()
		{
			CreateLocker();
			return CurrentPage.InstantiatePage(_pageContentContainer, null);
		}

		private void CreateLocker()
		{
			if (_locker != null) DestroyLocker();

			_locker = _container.InstantiateComponentOnNewGameObject<TutorialLocker>("TutorialLocker");
			_locker.transform.SetParent(_pageContentContainer, false);
		}

		private void DestroyLocker()
		{
			if (_locker == null) return;

			Object.Destroy(_locker.gameObject);
			_locker = null;
		}

		protected override void OnClosePage(bool complete)
		{
			base.OnClosePage(complete);

			foreach (Transform child in _pageContentContainer)
			{
				Object.Destroy(child.gameObject);
			}

			DestroyLocker();
		}
	}
}