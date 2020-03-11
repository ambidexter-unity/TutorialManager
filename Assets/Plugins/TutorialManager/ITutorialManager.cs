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
		/// Возвращает состояние для страницы с указанным ключом.
		/// </summary>
		/// <param name="pageId">Ключ страницы туториала.</param>
		/// <returns>Возвращает <code>true</code>, если страница уже была показана.</returns>
		bool GetPageState(string pageId);
	}
}