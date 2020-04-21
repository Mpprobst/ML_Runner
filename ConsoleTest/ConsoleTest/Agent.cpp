#include "pch.h"
#include "Agent.h"
#include <stdlib.h>
#include <random>
#include <iostream>

Agent::Agent()
{
}


Agent::~Agent()
{
}

//----------------------------------------------------------------------------------------
//----------------------------------------SETTERS-----------------------------------------
//----------------------------------------------------------------------------------------

void Agent::SetFarProb(float prob)
{
	p_far = prob;
}
void Agent::SetHighProb(float prob)
{
	p_high = prob;
}
void Agent::SetBelowProb(float prob)
{
	p_below = prob;
}
void Agent::SetNarrowProb(float prob)
{
	p_narrow = prob;
}
void Agent::SetEnemyProb(float prob)
{
	p_enemy = prob;
}
//----------------------------------------------------------------------------------------
//----------------------------------------GETTERS-----------------------------------------
//----------------------------------------------------------------------------------------

float Agent::GetFarProb()
{
	return p_far;
}
float Agent::GetHighProb()
{
	return p_high;
}
float Agent::GetBelowProb()
{
	return p_below;
}
float Agent::GetNarrowProb()
{
	return p_narrow;
}
float Agent::GetEnemyProb()
{
	return p_enemy;
}
float Agent::GetRandomFloat()
{
	static std::default_random_engine e;
	static std::uniform_real_distribution<> dis(0, 1); // rage 0 - 1
	return dis(e);
}
bool Agent::IsAlive()
{
	return alive;
}

bool Agent::AttemptJump(Block block)
{
	bool success = true;
	float totalProb = 0;

	if (block.GetFar()) totalProb += p_far;
	else totalProb += 1;

	if (block.GetHigh()) totalProb += p_high;
	else totalProb += 1;

	if (block.GetBelow()) totalProb += p_below;
	else totalProb += 1;

	if (block.GetNarrow()) totalProb += p_narrow;
	else totalProb += 1;

	if (block.GetEnemy()) totalProb += p_enemy;
	else totalProb += 1;

	totalProb = totalProb / 5;
	float random = GetRandomFloat();
	if (random > totalProb)
	{
		success = false;
	}

	std::cout << "totalProb = " << totalProb << " check = " << random << "\n";

	return success;
}