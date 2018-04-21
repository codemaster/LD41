using UnityEngine;
using Zenject;

public class EnemyController : MonoBehaviour
{
	[Inject]
	private Enemy.Pool _enemyPool;
}
