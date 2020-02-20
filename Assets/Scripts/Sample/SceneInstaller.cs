using Common.GameService;
using Common.PersistentManager;
using Zenject;

namespace Sample
{
	public class SceneInstaller : MonoInstaller<SceneInstaller>
	{
#pragma warning disable 649
		[Inject] private readonly IPersistentManager _persistentManager;
#pragma warning restore 649

		public override void InstallBindings()
		{
		}

		public override void Start()
		{
			if (!_persistentManager.IsReady)
			{
				_persistentManager.ReadyEvent += OnPersistentManagerReady;
				_persistentManager.Initialize();
			}
			else
			{
				CreateUi();
			}
		}

		private void OnPersistentManagerReady(IGameService service)
		{
			service.ReadyEvent -= OnPersistentManagerReady;
			CreateUi();
		}

		private void CreateUi()
		{
			Container.InstantiatePrefabResource("Ui");
		}
	}
}