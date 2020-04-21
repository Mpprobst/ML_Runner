#pragma once
#include "Block.h"
struct BlockData
{
public:
	int successes;
	int failures;
};

class AgentPerformance
{


public:

	AgentPerformance();
	~AgentPerformance();

	BlockData GetFarData();
	BlockData GetHighData();
	BlockData GetBelowData();
	BlockData GetNarrowData();
	BlockData GetEnemyData();

	float GetFarP();
	float GetHighP();
	float GetBelowP();
	float GetNarrowP();
	float GetEnemyP();

	void UpdateBlockData(Block block, bool success);

private:
	BlockData farData;
	BlockData highData;
	BlockData belowData;
	BlockData narrowData;
	BlockData enemyData;

};

