using UnityEngine;
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
		Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<EnemyController>().FromComponentInHierarchy().AsSingle();
		Container.BindMemoryPool<Enemy, Enemy.Pool>().FromComponentInNewPrefab(_enemyPrefab)
			.UnderTransform(_enemyParentTransform);
    }
}
