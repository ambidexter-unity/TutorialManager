using Common.PersistentManager;
using Common.TutorialManager;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
	public override void InstallBindings()
	{
		Container.Bind<IPersistentManager>()
			.FromComponentInNewPrefabResource(@"PersistentManager").AsSingle();
		Container.Bind<ITutorialManager>().To<TutorialManager>().AsSingle();
	}
}