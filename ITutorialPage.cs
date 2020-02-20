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
		/// <param name="callback">Коллбек, в который будет возвращен экземпляр созданной страницы.</param>
		/// <returns>Возвращает <code>true</code>, если страница будет создана.</returns>
		bool InstantiatePage(Transform pageContainer, Action<GameObject> callback);

		/// <summary>
		/// Событие закрытия страницы туториала. В событии передается true, если туториал завершен, иначе false.
		/// </summary>
		event Action<bool> CloseTutorialPageEvent;
	}
}