using System;
using System.Collections.Generic;
using Common.PersistentManager;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace Common.TutorialManager
{
	[Serializable]
	public class CompletedTutorialPagesData : IPersistent<CompletedTutorialPagesData>
	{
		public string PersistentId => "completed_tutorial_pages";

		// ReSharper disable once InconsistentNaming
		public List<string> CompletedPages = new List<string>();

		public void Restore<T1>(T1 data) where T1 : IPersistent<CompletedTutorialPagesData>
		{
			var src = data as CompletedTutorialPagesData;
			Assert.IsNotNull(src);

			CompletedPages.Clear();
			CompletedPages.AddRange(src.CompletedPages);
		}
	}

	public abstract class TutorialManagerBase : ITutorialManager
	{
		private bool _tutorialIsActive;
		protected ITutorialPage CurrentPage { get; private set; }

		private readonly Lazy<CompletedTutorialPagesData> _completeData;

		protected abstract IPersistentManager PersistentManager { get; }

		protected TutorialManagerBase()
		{
			_completeData = new Lazy<CompletedTutorialPagesData>(
				() => PersistentManager.GetPersistentValue<CompletedTutorialPagesData>());
		}

		// ITutorialManager

		public bool SetCurrentPage(ITutorialPage page)
		{
			Assert.IsNotNull(page);

			if (CurrentPage?.Id == page.Id) return true;

			if (_completeData.Value.CompletedPages.Contains(page.Id))
			{
				// Trying to open tutorial page, but this page was already completed.
				return false;
			}

			if (CurrentPage != null)
			{
				Debug.LogErrorFormat("Current tutorial page {0} wasn't closed before the next page.",
					CurrentPage.Id);
				OnClosePage(false);
			}

			CurrentPage = page;
			CurrentPage.CloseTutorialPageEvent += OnClosePage;
			TutorialIsActive = true;

			return InstantiateCurrentPage();
		}

		protected abstract bool InstantiateCurrentPage();

		public bool GetPageState(string pageId)
		{
			return _completeData.Value.CompletedPages.Contains(pageId);
		}

		public bool TutorialIsActive
		{
			get => _tutorialIsActive;
			private set
			{
				if (value == _tutorialIsActive) return;
				_tutorialIsActive = value;
				TutorialIsActiveChangeEvent?.Invoke(this, _tutorialIsActive);
			}
		}

		public event TutorialIsActiveHandler TutorialIsActiveChangeEvent;

		public string CurrentPageId => CurrentPage?.Id;

		// \ITutorialManager

		protected virtual void OnClosePage(bool complete)
		{
			Assert.IsNotNull(CurrentPage);
			CurrentPage.CloseTutorialPageEvent -= OnClosePage;

			if (complete)
			{
				if (!_completeData.Value.CompletedPages.Contains(CurrentPage.Id))
				{
					_completeData.Value.CompletedPages.Add(CurrentPage.Id);
					PersistentManager.Persist(_completeData.Value, true);
				}
				else
				{
					Debug.LogErrorFormat("Tutorial page with Id [{0}] was multiple completed.", CurrentPage.Id);
				}
			}

			CurrentPage = null;
			TutorialIsActive = false;
		}
	}
}