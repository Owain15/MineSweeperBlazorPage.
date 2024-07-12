

using System.Diagnostics;

public enum CellState 
{ 

  Hidden,
  UnHidden,
  Flagged

} 

public struct GameSettings
{
	public int XLength;
	public int YHeight;
	public int NumberOfMines;

}

internal struct Cell
{
	public int X; 
	public int Y;

	public int TestValue;

	public CellState State;

	public bool DoesCellContainsAMine;

	public int AdjacentMineCount;

}


internal class MineField
{
	public Cell[,] cellData;

	public bool IsGameStillRunning;

    private int[,] AdjacentCellValues = new int[8, 2] 
	{ {-1,-1 }, { 0, -1 }, { 1, -1 }, 
      { -1, 0 },            { 1, 0 }, 
	  { -1, 1 }, { 0, 1 }, { 1, 1 } };

	public MineField( GameSettings settings)
	{
		IsGameStillRunning = true;

		cellData = new Cell[settings.XLength, settings.YHeight];
	
		SetCellValues();
		SetMines(settings);
		SetAdjacentMineCount();
	}

	private void SetCellValues()
	{
		var testNo = 1;

		for (int x = 0; x < cellData.GetLength(0); x++)
		{
			for(int y = 0; y < cellData.GetLength(1); y++)
			{
				//var cell = cellData[x, y];

				cellData[x, y].X = x;
				cellData[x, y].Y = y;
				cellData[x, y].State = CellState.Hidden;
				cellData[x, y].DoesCellContainsAMine = false;

				//cellData[x, y] = cell;

				cellData[x, y].TestValue = testNo;
				testNo++;
			}
		}
	
	}
	private void SetMines(GameSettings settings)
	{
		Random random = new Random();

		var MinesSet = 0;

		while (MinesSet != settings.NumberOfMines)
		{
			var x = random.Next(0,cellData.GetLength(0));
			var y = random.Next(0, cellData.GetLength(1));

			if (!cellData[x,y].DoesCellContainsAMine)
			{ 
				cellData[x,y].DoesCellContainsAMine = true; 
			    MinesSet++;
			}
		}
	}
	private void SetAdjacentMineCount()
	{
		for (int x = 0; x < cellData.GetLength(0); x++)
		{
			for (int y = 0; y < cellData.GetLength(1); y++)
			{
				var adjacentMineCount = GetAdjacentMineCount(x,y);

				cellData[x, y].AdjacentMineCount = adjacentMineCount;
			}

		}
	}
	private int GetAdjacentMineCount(int x, int y)
	{
		var adjacentMines = 0;
		
		for(int i = 0; i < 8; i++)
		{
			var adjacentCellX = x + AdjacentCellValues[i,0];
			var adjacentCellY = y + AdjacentCellValues[i,1];

		  if(adjacentCellX >= 0 && adjacentCellX < cellData.GetLength(0) 
				&& adjacentCellY >= 0 && adjacentCellY < cellData.GetLength(1) 
				&& cellData[adjacentCellX,adjacentCellY].DoesCellContainsAMine)
		  { adjacentMines++; }
		
		}
		return adjacentMines;
	}
	private void CascadeUnHiddenCell(int x, int y)
	{
		for (int i = 0; i < 8; i++)
		{
			var adjacentCellX = x + AdjacentCellValues[i, 0];
			var adjacentCellY = y + AdjacentCellValues[i, 1];

			if (adjacentCellX >= 0 && adjacentCellX < cellData.GetLength(0)
				&& adjacentCellY >= 0 && adjacentCellY < cellData.GetLength(1) 
				&&cellData[adjacentCellX, adjacentCellY].State == CellState.Hidden)
			{ 
				cellData[adjacentCellX, adjacentCellY].State = CellState.UnHidden; 
			    if(cellData[adjacentCellX, adjacentCellY].AdjacentMineCount==0)
				{ CascadeUnHiddenCell(adjacentCellX, adjacentCellY); }
			}

		}
	}

	public void HandleLeftClickEvent(int x, int y)
	{
		if (cellData[x,y].State == CellState.Hidden)
		{ cellData[x, y].State = CellState.UnHidden; }

		if (cellData[x,y].DoesCellContainsAMine && cellData[x,y].State == CellState.UnHidden)
		{ IsGameStillRunning = false; }

		if(cellData[x,y].State == CellState.UnHidden && cellData[x,y].AdjacentMineCount == 0 )
		{ CascadeUnHiddenCell(x, y); }
	}
	public void HandleRightClickEvent(int x, int y)
	{
		if (cellData[x,y].State == CellState.Hidden)
		{ cellData[x, y].State = CellState.Flagged; }
		
		else if (cellData[x, y].State == CellState.Flagged)
		{ cellData[x, y].State = CellState.Hidden; }

	}
	public void HandleLeftClickHoldEvent(int x, int y) { }
	public void HandleLeftClickHoldUpEvent(int x, int y) { }

}