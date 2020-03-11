using Common.GameService;
using Common.GameTask;
using Common.PersistentManager;
using Common.TutorialManager;
using Zenject;

namespace Sample
{
	public class SceneInstaller : MonoInstaller<SceneInstaller>
	{
#pragma warning disable 649
		[Inject] private readonly IPersistentManager _persistentManager;
		[Inject] private readonly ITutorialManager _tutorialManager;
#pragma warning restore 649

		public override void InstallBindings()
		{
		}

		public override void Start()
		{
			var initialQueue = new GameTaskQueue();
			initialQueue.Add(new GameTaskInitService(_persistentManager));
			initialQueue.Add(new GameTaskInitService(_tutorialManager));
			initialQueue.CompleteEvent += OnInitialComplete;
			initialQueue.Start();
		}

		private void OnInitialComplete(IGameTask service)
		{
			service.CompleteEvent -= OnInitialComplete;
			Container.InstantiatePrefabResource("Ui");
		}
	}
}