using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
	public class ClearSavedData : EditorWindow
	{
		private bool _clearPlayerPrefs = true;
		private bool _showMessage;
		private float _showMessageTime;

		[MenuItem("Tools/Clear Saved Data")]
		public static void ShowWindow()
		{
			GetWindow<ClearSavedData>(false, "Clear saved data").Show();
		}

		private void OnGUI()
		{
			var isPlaying = Application.isPlaying;
			if (_showMessage)
			{
				EditorGUILayout.HelpBox(@"Data cleared", MessageType.Info);
				_showMessageTime -= Time.deltaTime;

				if (_showMessageTime <= 0)
				{
					_showMessage = false;
				}
			}
			else
			{
				if (isPlaying)
				{
					EditorGUILayout.HelpBox(@"Can't clear when Application is playing.", MessageType.Error);
				}
				else
				{
					EditorGUILayout.HelpBox(@"Select the data that you want to clear.", MessageType.Info);
				}
			}

			EditorGUI.BeginDisabledGroup(isPlaying);
			_clearPlayerPrefs = EditorGUILayout.Toggle("Clear player prefs", _clearPlayerPrefs);
			EditorGUI.EndDisabledGroup();

			GUILayout.Space(10);

			EditorGUI.BeginDisabledGroup(isPlaying || !_clearPlayerPrefs);
			if (GUILayout.Button("Clear")) DoClear();
			EditorGUI.EndDisabledGroup();
		}

		private void DoClear()
		{
			if (_clearPlayerPrefs)
			{
				var cleared = false;
				if (PlayerPrefs.HasKey(Sample.PersistentManager.Key))
				{
					PlayerPrefs.DeleteKey(Sample.PersistentManager.Key);
					cleared = true;
				}

				if (cleared) Debug.Log("Player prefs was cleared.");
			}

			_showMessage = true;
			_showMessageTime = 0.5f;

			var window = GetWindow<ClearSavedData>();
			if (!IsDocked(window))
			{
				window.Close();
			}
		}

		private bool IsDocked(EditorWindow window)
		{
			if (window == null) return false;
			return (bool) (typeof(EditorWindow).GetProperty("docked",
					               BindingFlags.Public |
					               BindingFlags.NonPublic |
					               BindingFlags.Instance |
					               BindingFlags.Static)?.GetGetMethod(true)
				               .Invoke(this, null) ?? false);
		}
	}
}