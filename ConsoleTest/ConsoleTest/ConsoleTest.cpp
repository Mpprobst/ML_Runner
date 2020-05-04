// ConsoleTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include "Agent.h"
#include "Block.h"
#include "BlockManager.h"
#include "AgentPerformance.h"
#include <fstream>

using namespace std;

int main()
{
	ofstream resultsFile;
	resultsFile.open("results.csv");
	resultsFile << "far, high, below, narrow, enemy\n";
	int seed = 0;
	int numTests = 5000;

	Agent agent;
	agent.SetFarProb(.5);
	agent.SetHighProb(.5);
	agent.SetBelowProb(.5);
	agent.SetNarrowProb(.5);
	agent.SetEnemyProb(.5);

	resultsFile << "Agent Skill\n";
	resultsFile << agent.GetFarProb() << "," << agent.GetHighProb() << "," << agent.GetBelowProb() << "," << agent.GetNarrowProb() << "," << agent.GetEnemyProb() << "\n";
	resultsFile << "Blocks Feature Counts Over Time\n";

	AgentPerformance performance;
	int lastFarCount = 2;
	int lastHighCount = 2;
	int lastBelowCount = 2;
	int lastNarrowCount = 2;
	int lastEnemyCount = 2;

	BlockManager blockManager;

	while (seed < numTests)
	{
		bool agentAlive = true;
		blockManager.SetDifficulty(0.15);
		blockManager.SetSeed(seed);
		int blockCount = 0;
		while (agentAlive && blockCount < 100)
		{
			Block spawnedBlock = blockManager.SpawnBlock(performance);
			spawnedBlock.GetName();

			agentAlive = agent.AttemptJump(spawnedBlock);
			performance.UpdateBlockData(spawnedBlock, agentAlive);
			blockCount++;
			cout << blockCount << "\n";
		}
		cout << "DIE after " << blockCount << " blocks\n\n";
		int farCount = performance.GetFarData().successes + performance.GetFarData().failures;
		int highCount = performance.GetHighData().successes + performance.GetHighData().failures;
		int belowCount = performance.GetBelowData().successes + performance.GetBelowData().failures;
		int narrowCount = performance.GetNarrowData().successes + performance.GetNarrowData().failures;
		int enemyCount = performance.GetEnemyData().successes + performance.GetEnemyData().failures;

		resultsFile << farCount - lastFarCount << ",";
		resultsFile << highCount - lastHighCount << ",";
		resultsFile << belowCount - lastBelowCount << ",";
		resultsFile << narrowCount - lastNarrowCount << ",";
		resultsFile << enemyCount - lastEnemyCount << ",";
		resultsFile << blockCount << "\n";

		lastFarCount = farCount;
		lastHighCount = highCount;
		lastBelowCount = belowCount;
		lastNarrowCount = narrowCount;
		lastEnemyCount = enemyCount;

		seed++;
	}
	resultsFile << "Final Probabilities\n";
	resultsFile << performance.GetFarP() << "," << performance.GetHighP() << ", " << performance.GetBelowP() << ", " << performance.GetNarrowP() << ", " << performance.GetEnemyP() << "\n";
	resultsFile.close();
	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
