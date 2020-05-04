#include "pch.h"
#include "BlockManager.h"
#include <random>
#include <iostream>

BlockManager::BlockManager()
{
}

BlockManager::~BlockManager()
{
}

void BlockManager::SetDifficulty(float d)
{
	difficulty = d;
}

void BlockManager::SetSeed(int s)
{	
	seed = s;
	srand(seed);
}

float BlockManager::GetDifficulty()
{
	return difficulty;
}

float BlockManager::GetProbability(BlockData data)
{
	float prob = 1;
	
	std::cout << "s=" << data.successes << "f=" << data.failures << " ";
	//prob = (float)(data.successes - (data.failures / difficulty)) / data.successes;
	// should fail around 45 blocks
	if (difficulty < 0.6)
	{
		prob = (float)data.successes / ((float)data.failures * 10.0 + (float)data.successes);
	}
	else
	{
		prob = (float)data.failures  / ((float)data.successes + (float)data.failures);
	}
	//prob = (float)(data.successes - (data.failures * (0.6 - difficulty))) / data.successes;
	if (prob < 0) prob = 0;
	std::cout << prob << "\n ";
	
	return prob;
}
float BlockManager::GetRandomFloat(float a, float b)
{
	float r2 = static_cast <float> (rand()) / (static_cast <float> (RAND_MAX / (a + b)));
	//std::cout << "goal: " << a << ", check: " << r2 << "\n";
	return r2;
}

void BlockManager::UpdateDifficulty()
{

}

// Use difficulty and performance to determine the block
Block BlockManager::SpawnBlock(AgentPerformance performance)
{
	Block block;
	//std::cout << "probabilities: \n";
	float p_far = GetProbability(performance.GetFarData());
	float p_high = GetProbability(performance.GetHighData());
	float p_below = GetProbability(performance.GetBelowData());
	float p_narrow = GetProbability(performance.GetNarrowData());
	float p_enemy = GetProbability(performance.GetEnemyData());

	//std::cout << "\n";


	if (GetRandomFloat(p_far, p_high) < p_far)
	{
		block.SetFar(true);
		block.SetHigh(false);
	}
	else
	{
		block.SetHigh(true);
		block.SetFar(false);
	}

	if (GetRandomFloat(p_below, 1.0 - p_below) < p_below)
	{
		block.SetBelow(true);
		block.SetHigh(false);
	}
	else
	{
		block.SetBelow(false);
	}

	if (GetRandomFloat(p_narrow, 1.0 - p_narrow) < p_narrow)
	{
		block.SetNarrow(true);
	}
	else
	{
		block.SetNarrow(false);
	}

	if (GetRandomFloat(p_enemy, 1.0 - p_enemy) < p_enemy)
	{
		block.SetEnemy(true);
	}
	else
	{
		block.SetEnemy(false);
	}

	//difficulty += 0.01;
	if (difficulty > 1) difficulty = 1;

	return block;
}
