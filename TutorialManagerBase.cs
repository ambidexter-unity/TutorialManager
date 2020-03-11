//#define DISABLE_TUTORIAL

using System;
using System.Collections.Generic;
using Common.GameService;
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
		private bool _isReady;
		private bool _tutorialIsActive;
		protected ITutorialPage CurrentPage { get; private set; }

		private readonly CompletedTutorialPagesData _completeData = new CompletedTutorialPagesData();

		// ITutorialManager

		public virtual void Initialize(params object[] args)
		{
#if DISABLE_TUTORIAL
			IsReady = true;
#else
			RestoreCompleteTutorialPagesData(data =>
			{
				if (data != null)
				{
					_completeData.Restore(data);
				}
				else
				{
					Debug.LogError("Failed to restore complete tutorial pages data.");
				}

				IsReady = true;
			});
#endif
		}

		protected abstract void RestoreCompleteTutorialPagesData(Action<CompletedTutorialPagesData> callback);

		public bool IsReady
		{
			get => _isReady;
			private set
			{
				if (value == _isReady) return;
				_isReady = value;
				Assert.IsTrue(_isReady);
				ReadyEvent?.Invoke(this);
			}
		}

		public event GameServiceReadyHandler ReadyEvent;

		public bool SetCurrentPage(ITutorialPage page)
		{
			if (!IsReady)
			{
				throw new Exception("TutorialManager must be initialized.");
			}

			Assert.IsNotNull(page);

#if DISABLE_TUTORIAL
			return false;
#else
			if (CurrentPage?.Id == page.Id) return true;

			if (GetPageState(page.Id))
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
#endif
		}

		protected abstract bool InstantiateCurrentPage();

		public bool GetPageState(string pageId)
		{
			if (!IsReady)
			{
				throw new Exception("TutorialManager must be initialized.");
			}
#if DISABLE_TUTORIAL
			return true;
#else
			return _completeData.CompletedPages.Contains(pageId);
#endif
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
				if (!GetPageState(CurrentPage.Id))
				{
					_completeData.CompletedPages.Add(CurrentPage.Id);
					var dataClone = new CompletedTutorialPagesData();
					dataClone.Restore(_completeData);
					PersistCompleteTutorialPagesData(dataClone, b =>
					{
						if (!b) Debug.LogError("Failed to persist TutorialManager data.");
					});
				}
				else
				{
					Debug.LogErrorFormat("Tutorial page with Id [{0}] was multiple completed.", CurrentPage.Id);
				}
			}

			CurrentPage = null;
			TutorialIsActive = false;
		}

		protected abstract void PersistCompleteTutorialPagesData(CompletedTutorialPagesData data,
			Action<bool> readyCallback);
	}
}