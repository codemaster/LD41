using UnityEngine;
using UnityEngine.AI;
using Zenject;

/// <summary>
/// The installer for the gameplay scene
/// </summary>
public class GameplayInstaller : MonoInstaller<GameplayInstaller>
{
	/// <summary>
	/// The enemy prefab
	/// </summary>
	[SerializeField]
	private Enemy _enemyPrefab;

	/// <summary>
	/// The parent prefab for enemies
	/// </summary>
	[SerializeField]
	private Transform _enemyParentTransform;

	/// <summary>
	/// Installs the zenject bindings
	/// </summary>
    public override void InstallBindings()
    {
		Container.Bind<DownloadingPanel>().FromComponentInHierarchy().AsSingle();
		Container.Bind<NavMeshSurface>().FromComponentInHierarchy().AsSingle();
		Container.Bind<SoundController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<EnemyController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<WordController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<ScoreController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<HighScoresController>().FromComponentInHierarchy().AsSingle();
		Container.BindMemoryPool<Enemy, Enemy.Pool>().FromComponentInNewPrefab(_enemyPrefab)
			.UnderTransform(_enemyParentTransform);
    }
}
