#include "pch.h"
#include "Block.h"
#include <iostream>
#include <stdio.h>
Block::Block()
{
}


Block::~Block()
{
	far = false;
	high = false;
	below = false;
	narrow = false;
	enemy = false;
}

//----------------------------------------------------------------------------------------
//----------------------------------------SETTERS-----------------------------------------
//----------------------------------------------------------------------------------------

void Block::SetFar(bool value)
{
	far = value;
}
void Block::SetHigh(bool value)
{
	high = value;
}
void Block::SetBelow(bool value)
{
	below = value;
}
void Block::SetNarrow(bool value)
{
	narrow = value;
}
void Block::SetEnemy(bool value)
{
	enemy = value;
}

//----------------------------------------------------------------------------------------
//----------------------------------------GETTERS-----------------------------------------
//----------------------------------------------------------------------------------------

bool Block::GetFar()
{
	return far;
}
bool Block::GetHigh()
{
	return high;
}
bool Block::GetBelow()
{
	return below;
}
bool Block::GetNarrow()
{
	return narrow;
}
bool Block::GetEnemy()
{
	return enemy;
}
char * Block::GetName()
{
	char name[6];
	
	if (far) name[0] = 'F';
	else name[0] = 'S';

	if (high) name[1] = 'H';
	else name[1] = 'L';

	if (below) name[2] = 'B';
	else name[2] = 'A';

	if (narrow) name[3] = 'N';
	else name[3] = 'W';

	if (enemy) name[4] = 'E';
	else name[4] = 'X';

	name[5] = '\0';

	std::cout << "block: ";
	for (int i = 0; i < 5; i++)
	{
		std::cout << name[i];
	}
	std::cout << "\n ";

	return name;
}
