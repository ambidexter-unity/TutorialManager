//#define DISABLE_TUTORIAL

using System;
using System.Collections.Generic;
using System.Linq;
using Common.GameService;
using Common.PersistentManager;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace Common.TutorialManager
{
	[Serializable]
	public class CompletedTutorialPagesDataRecord : ICloneable, IEquatable<CompletedTutorialPagesDataRecord>
	{
		// ReSharper disable InconsistentNaming
		public string PageId;
		public int CompletedValue;
		public bool IsFinished;
		// ReSharper restore InconsistentNaming

		public object Clone()
		{
			return new CompletedTutorialPagesDataRecord
			{
				PageId = PageId,
				CompletedValue = CompletedValue,
				IsFinished = IsFinished
			};
		}

		public bool Equals(CompletedTutorialPagesDataRecord other)
		{
			return other != null && PageId == other.PageId && CompletedValue == other.CompletedValue &&
			       IsFinished == other.IsFinished;
		}

		public void SetValues(CompletedTutorialPagesDataRecord other)
		{
			SetValues(other.PageId, other.CompletedValue, other.IsFinished);
		}

		public void SetValues(string pageId, int completedValue, bool isFinished)
		{
			PageId = pageId;
			CompletedValue = completedValue;
			IsFinished = isFinished;
		}
	}

	[Serializable]
	public class CompletedTutorialPagesData : IPersistent<CompletedTutorialPagesData>
	{
		public string PersistentId => "completed_tutorial_pages";

		// ReSharper disable once InconsistentNaming
		public List<CompletedTutorialPagesDataRecord> CompletedPages = new List<CompletedTutorialPagesDataRecord>();

		public void Restore<T1>(T1 data) where T1 : IPersistent<CompletedTutorialPagesData>
		{
			var src = data as CompletedTutorialPagesData;
			Assert.IsNotNull(src);

			CompletedPages.Clear();
			CompletedPages.AddRange(src.CompletedPages.Select(record => record.Clone())
				.Cast<CompletedTutorialPagesDataRecord>());
		}
	}

	public abstract class TutorialManagerBase : ITutorialManager
	{
		private bool _isReady;
		private bool _tutorialIsActive;
		protected ITutorialPage CurrentPage { get; private set; }

		protected readonly CompletedTutorialPagesData CompletedData = new CompletedTutorialPagesData();

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
					CompletedData.Restore(data);
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
			protected set
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

			if (IsPageFinished(page.Id))
			{
				// Trying to open tutorial page, but this page was already completed.
				return false;
			}

			if (CurrentPage != null)
			{
				Debug.LogErrorFormat("Current tutorial page {0} wasn't closed before the next page.",
					CurrentPage.Id);
				OnTerminatePageEvent(false);
			}

			CurrentPage = page;
			CurrentPage.CompleteTutorialPageEvent += OnCompletePage;
			CurrentPage.TerminateTutorialPageEvent += OnTerminatePageEvent;
			TutorialIsActive = true;

			return InstantiateCurrentPage(page.PageCurrentCompleteValue);
#endif
		}

		protected abstract bool InstantiateCurrentPage(int pageCurrentCompleteValue);

		public bool IsPageFinished(string pageId)
		{
			if (!IsReady)
			{
				throw new Exception("TutorialManager must be initialized.");
			}
#if DISABLE_TUTORIAL
			return true;
#else
			var record = CompletedData.CompletedPages.SingleOrDefault(dataRecord => dataRecord.PageId == pageId);
			return record?.IsFinished ?? false;
#endif
		}

		public bool IsPageCompleteUntil(string pageId, int pageCompleteValue)
		{
			if (IsPageFinished(pageId)) return true;

			var record = CompletedData.CompletedPages.SingleOrDefault(dataRecord => dataRecord.PageId == pageId);
			return (record?.CompletedValue ?? 0) >= pageCompleteValue;
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

		public void RestoreTutorialPage(string pageId, int pageCurrentCompleteValue = 0,
			Action<bool> readyCallback = null)
		{
			var doPersist = false;
			var record = CompletedData.CompletedPages.SingleOrDefault(dataRecord => dataRecord.PageId == CurrentPageId);
			if (record == null)
			{
				if (pageCurrentCompleteValue > 0)
				{
					record = new CompletedTutorialPagesDataRecord
					{
						PageId = pageId,
						CompletedValue = pageCurrentCompleteValue
					};
					CompletedData.CompletedPages.Add(record);
					doPersist = true;
				}
			}
			else
			{
				if (pageCurrentCompleteValue > 0)
				{
					var restoredRecord = new CompletedTutorialPagesDataRecord
					{
						PageId = pageId,
						CompletedValue = pageCurrentCompleteValue
					};
					if (!record.Equals(restoredRecord))
					{
						record.SetValues(restoredRecord);
						doPersist = true;
					}
				}
				else
				{
					CompletedData.CompletedPages.Remove(record);
					doPersist = true;
				}
			}

			if (doPersist)
			{
				var dataClone = new CompletedTutorialPagesData();
				dataClone.Restore(CompletedData);
				PersistCompleteTutorialPagesData(dataClone, readyCallback);
			}
			else
			{
				readyCallback?.Invoke(true);
			}
		}

		public event TutorialIsActiveHandler TutorialIsActiveChangeEvent;

		public string CurrentPageId => CurrentPage?.Id;

		// \ITutorialManager

		private void OnCompletePage(int pageCurrentCompleteValue)
		{
			Assert.IsNotNull(CurrentPage);

			var newRecord = new CompletedTutorialPagesDataRecord
			{
				PageId = CurrentPageId,
				CompletedValue = pageCurrentCompleteValue,
				IsFinished = pageCurrentCompleteValue >= CurrentPage.PageFinalCompleteValue
			};

			bool doPersist;
			var record = CompletedData.CompletedPages.SingleOrDefault(dataRecord => dataRecord.PageId == CurrentPageId);
			if (record == null)
			{
				record = newRecord;
				CompletedData.CompletedPages.Add(record);
				doPersist = true;
			}
			else
			{
				doPersist = !record.Equals(newRecord);
				record.SetValues(newRecord);
			}

			if (doPersist)
			{
				var dataClone = new CompletedTutorialPagesData();
				dataClone.Restore(CompletedData);
				PersistCompleteTutorialPagesData(dataClone, b =>
				{
					if (!b) Debug.LogError("Failed to persist TutorialManager data.");
				});
			}

			if (record.IsFinished)
			{
				FinishPage(CurrentPage);

				CurrentPage = null;
				TutorialIsActive = false;
			}
		}

		private void OnTerminatePageEvent(bool markAsFinished)
		{
			Assert.IsNotNull(CurrentPage);

			bool doPersist = true;
			var record = CompletedData.CompletedPages.SingleOrDefault(dataRecord => dataRecord.PageId == CurrentPageId);
			if (record == null)
			{
				record = new CompletedTutorialPagesDataRecord
				{
					PageId = CurrentPageId,
					IsFinished = markAsFinished
				};
				CompletedData.CompletedPages.Add(record);
			}
			else if (markAsFinished)
			{
				record.IsFinished = true;
			}
			else
			{
				doPersist = false;
			}

			if (doPersist)
			{
				var dataClone = new CompletedTutorialPagesData();
				dataClone.Restore(CompletedData);
				PersistCompleteTutorialPagesData(dataClone, b =>
				{
					if (!b) Debug.LogError("Failed to persist TutorialManager data.");
				});
			}

			FinishPage(CurrentPage);

			CurrentPage = null;
			TutorialIsActive = false;
		}

		protected virtual void FinishPage(ITutorialPage page)
		{
			CurrentPage.CompleteTutorialPageEvent -= OnCompletePage;
			CurrentPage.TerminateTutorialPageEvent -= OnTerminatePageEvent;
		}

		protected abstract void PersistCompleteTutorialPagesData(CompletedTutorialPagesData data,
			Action<bool> readyCallback);
	}
}