#pragma once
class Block
{
public:
	Block();
	~Block();

	void SetFar(bool value);
	void SetHigh(bool value);
	void SetBelow(bool value);
	void SetNarrow(bool value);
	void SetEnemy(bool value);

	bool GetFar();
	bool GetHigh();
	bool GetBelow();
	bool GetNarrow();
	bool GetEnemy();
	char * GetName();

private:
	bool far;
	bool high;
	bool below;
	bool narrow;
	bool enemy;

};

