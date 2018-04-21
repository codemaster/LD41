using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Extensions for the NavMeshAgent class
/// </summary>
public static class NavMeshAgentExtensions
{
	/// <summary>
	/// If the NavMeshAgent has completed pathing
	/// </summary>
	/// <param name="agent">The nav agent</param>
	/// <returns>If pathing is complete</returns>
	public static bool IsComplete(this NavMeshAgent agent)
	{
		return Vector3.Distance(agent.destination, agent.transform.position) <=
			agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
	}
}
