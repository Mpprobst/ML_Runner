#pragma once
#include "Block.h"
class Agent
{

public:
	Agent();
	~Agent();

	// Setters
	void SetFarProb(float prob);
	void SetHighProb(float prob);
	void SetBelowProb(float prob);
	void SetNarrowProb(float prob);
	void SetEnemyProb(float prob);

	// Getters
	float GetFarProb();
	float GetHighProb();
	float GetBelowProb();
	float GetNarrowProb();
	float GetEnemyProb();
	float GetRandomFloat();
	bool IsAlive();

	// Add probabilities and determine if the jump is made
	bool AttemptJump(Block block);

private:
	// Feature probabilities
	float p_far;
	float p_high;
	float p_below;
	float p_narrow;
	float p_enemy;

	bool alive;

};

