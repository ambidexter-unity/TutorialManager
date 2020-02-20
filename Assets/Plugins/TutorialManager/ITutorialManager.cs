using System;

// ReSharper disable once CheckNamespace
namespace Common.TutorialManager
{
	public delegate void TutorialIsActiveHandler(ITutorialManager manager, bool tutorialIsActive);

	public interface ITutorialManager
	{
		/// <summary>
		/// Отображаемая страница туториала.
		/// </summary>
		bool SetCurrentPage(ITutorialPage page);

		/// <summary>
		/// Проверка можно ли показать страницу тутора
		/// </summary>
		[Obsolete("Use comparison with CurrentPageId instead.")]
		bool IsPageShowed(ITutorialPage page);

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
	}
}