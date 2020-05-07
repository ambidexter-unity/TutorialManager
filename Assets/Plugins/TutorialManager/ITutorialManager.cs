using System;
using Common.GameService;

// ReSharper disable once CheckNamespace
namespace Common.TutorialManager
{
	public delegate void TutorialIsActiveHandler(ITutorialManager manager, bool tutorialIsActive);

	public interface ITutorialManager : IGameService
	{
		/// <summary>
		/// Отображаемая страница туториала.
		/// </summary>
		bool SetCurrentPage(ITutorialPage page);

		/// <summary>
		/// Флаг, указывающий на активность туториала.
		/// </summary>
		bool TutorialIsActive { get; }

		/// <summary>
		/// Событие изменения состояния активности туториала.
		/// </summary>
		event TutorialIsActiveHandler TutorialIsActiveChangeEvent;

		/// <summary>
		/// Идентификатор текущей открытой страницы туториала.
		/// </summary>
		string CurrentPageId { get; }

		/// <summary>
		/// Возвращает состояние завершения для страницы с указанным ключом.
		/// </summary>
		/// <param name="pageId">Ключ страницы туториала.</param>
		/// <returns>Возвращает <code>true</code>, если страница уже была показана.</returns>
		bool IsPageFinished(string pageId);

		/// <summary>
		/// Возвращает состояние завершенности для страницы с указанным ключом.
		/// </summary>
		/// <param name="pageId">Ключ страницы туториала.</param>
		/// <param name="pageCompleteValue">Исследуемое знаение завершенности.</param>
		/// <returns>Возвращает <code>true</code>, если значение завершенности страницы больше
		/// или равно исследуемому значению.</returns>
		bool IsPageCompleteUntil(string pageId, int pageCompleteValue);

		/// <summary>
		/// Восстановить страницу туториала.
		/// </summary>
		/// <param name="pageId">Идентификатор восстанавливаемой страницы.</param>
		/// <param name="pageCurrentCompleteValue">Восстанавливаемое текущее значение завершенности страницы.</param>
		/// <param name="readyCallback">Коллбек, вызываемый по завершении восстановления.</param>
		void RestoreTutorialPage(string pageId, int pageCurrentCompleteValue = 0, Action<bool> readyCallback = null);
	}
}