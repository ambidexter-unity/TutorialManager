using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Common.TutorialManager
{
	/// <summary>
	/// Интерфейс страницы туториала.
	/// </summary>
	public interface ITutorialPage
	{
		/// <summary>
		/// Уникальный идентификатор страницы туториала.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Инстанцировать представление страницы.
		/// </summary>
		/// <param name="pageContainer">Контейнер страницы в менеджере туториала.</param>
		/// <param name="pageCurrentCompleteValue">Текущее восстановленное значение завершенности туториала.</param>
		/// <param name="callback">Коллбек, в который будет возвращен экземпляр созданной страницы.</param>
		/// <returns>Возвращает <code>true</code>, если страница будет создана.</returns>
		bool InstantiatePage(Transform pageContainer, int pageCurrentCompleteValue, Action<GameObject> callback);

		/// <summary>
		/// Значение полного завершения туториала.
		/// </summary>
		int PageFinalCompleteValue { get; }

		int PageCurrentCompleteValue { get; }

		/// <summary>
		/// Событие завершения этапа страницы туториала. В событии передается текущее значение завершенности туториала,
		/// если туториал завершен, значение будет равно PageCompleteValue, иначе меньше.
		/// </summary>
		event Action<int> CompleteTutorialPageEvent;

		/// <summary>
		/// Событие принудительного завершения выполнения страницы туториала. В событии передается флаг, указывающий
		/// отмечать страницу как завершенную.
		/// </summary>
		event Action<bool> TerminateTutorialPage;
	}
}