#pragma once
#include "Block.h"
#include "AgentPerformance.h"
class BlockManager
{
public:
	BlockManager();
	~BlockManager();

	void SetDifficulty(float d);
	void SetSeed(int s);

	float GetDifficulty();
	float GetProbability(BlockData data);
	float GetRandomFloat(float a, float b);

	void UpdateDifficulty();

	// give this block to the agent
	Block SpawnBlock(AgentPerformance performance);

private:
	float difficulty;
	int seed;
};

