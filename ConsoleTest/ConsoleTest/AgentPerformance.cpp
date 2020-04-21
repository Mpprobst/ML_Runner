#include "pch.h"
#include "AgentPerformance.h"


AgentPerformance::AgentPerformance()
{
	farData.successes = 1;
	farData.failures = 1;

	highData.successes = 1;
	highData.failures = 1;

	belowData.successes = 1;
	belowData.failures = 1;

	narrowData.successes = 1;
	narrowData.failures = 1;

	enemyData.successes = 1;
	enemyData.failures = 1;
}


AgentPerformance::~AgentPerformance()
{
}

//----------------------------------------------------------------------------------------
//----------------------------------------GETTERS-----------------------------------------
//----------------------------------------------------------------------------------------

BlockData AgentPerformance::GetFarData()
{
	return farData;
}
BlockData AgentPerformance::GetHighData()
{
	return highData;
}
BlockData AgentPerformance::GetBelowData()
{
	return belowData;
}
BlockData AgentPerformance::GetNarrowData()
{
	return narrowData;
}
BlockData AgentPerformance::GetEnemyData()
{
	return enemyData;
}

float AgentPerformance::GetFarP()
{
	return (float)farData.successes / (farData.successes + farData.failures);
}
float AgentPerformance::GetHighP()
{
	return (float)highData.successes / (highData.successes + highData.failures);
}
float AgentPerformance::GetBelowP()
{
	return (float)belowData.successes / (belowData.successes + belowData.failures);
}
float AgentPerformance::GetNarrowP()
{
	return (float)narrowData.successes / (narrowData.successes + narrowData.failures);
}
float AgentPerformance::GetEnemyP()
{
	return (float)enemyData.successes / (enemyData.successes + enemyData.failures);
}

void AgentPerformance::UpdateBlockData(Block block, bool success)
{
	if (block.GetFar())
	{
		if (success) farData.successes++;
		else farData.failures++;
	}
	if (block.GetHigh())
	{
		if (success) highData.successes++;
		else highData.failures++;
	}
	if (block.GetBelow())
	{
		if (success) belowData.successes++;
		else belowData.failures++;
	}
	if (block.GetNarrow())
	{
		if (success) narrowData.successes++;
		else narrowData.failures++;
	}
	if (block.GetEnemy())
	{
		if (success) enemyData.successes++;
		else enemyData.failures++;
	}

}