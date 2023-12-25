using System.Collections.Generic;
using System.IO;

static class Day10 {
	public static int Solution1() {
		string[] lines = File.ReadAllLines("input10.txt");
		(int startRow, int startCol) = FindStart(lines);

		int fromRow = startRow;
		int fromCol = startCol;
		int toRow = startRow; //Based on input
		int toCol = startCol - 1; //Based on input
		int pipeLength = 1;
		while (toRow != startRow || toCol != startCol) {
			Move(lines, ref fromRow, ref toRow, ref fromCol, ref toCol);
			pipeLength++;
		}
		return pipeLength / 2;
	}

	public static int Solution2() {
		string[] lines = File.ReadAllLines("input10.txt");
		(int startRow, int startCol) = FindStart(lines);

		int newRows = lines.Length * 2;
		int newCols = lines[0].Length * 2;
		bool[] newMap = new bool[newRows * newCols];
		int GetIndex(int row, int col) => row * 2 * newCols + col * 2;

		int lastIndex = GetIndex(startRow, startCol);
		newMap[lastIndex] = true;
		newMap[lastIndex + newCols] = true; //Based on input
		int fromRow = startRow;
		int fromCol = startCol;
		int toRow = startRow; //Based on input
		int toCol = startCol - 1; //Based on input
		int pipeLength = 1;
		while (toRow != startRow || toCol != startCol) {
			int newIndex = GetIndex(toRow, toCol);
			newMap[newIndex] = true;
			newMap[(lastIndex + newIndex) / 2] = true;
			lastIndex = newIndex;
			Move(lines, ref fromRow, ref toRow, ref fromCol, ref toCol);
			pipeLength++;
		}

		HashSet<(int, int)> enqueued = [];
		Stack<(int, int)> queue = new();
		queue.Push((0, newCols - 1)); //Top right corner, guaranteed to not be inside the pipe
		enqueued.Add((0, newCols - 1));
		void TryEnqueue((int row, int col) coord) {
			if (!newMap[coord.row * newCols + coord.col] && enqueued.Add(coord)) {
				queue.Push(coord);
			}
		}

		int outerTiles = 0;
		while (queue.Count > 0) {
			(int row, int col) = queue.Pop();
			if (row % 2 == 0 && col % 2 == 0) { //Only even tiles are part of the actual grid
				outerTiles++;
			}
			if (row != 0) {
				TryEnqueue((row - 1, col));
			}
			if (col != 0) {
				TryEnqueue((row, col - 1));
			}
			if (row != newRows - 1) {
				TryEnqueue((row + 1, col));
			}
			if (col != newCols - 1) {
				TryEnqueue((row, col + 1));
			}
		}

		return lines.Length * lines[0].Length - pipeLength - outerTiles;
	}

	static readonly Dictionary<char, (int row1, int row2, int col1, int col2)> map = new() {
		{ '|', (-1, 1, 0, 0) },
		{ '-', (0, 0, -1, 1) },
		{ 'L', (-1, 0, 0, 1) },
		{ 'J', (-1, 0, 0, -1) },
		{ '7', (0, 1, -1, 0) },
		{ 'F', (0, 1, 1, 0) }
	};

	static (int, int) FindStart(string[] lines) {
		for (int row = 0; row < lines.Length; row++) {
			int col = lines[row].IndexOf('S');
			if (col >= 0) {
				return (row, col);
			}
		}
		return (-1, -1);
	}

	static void Move(string[] lines, ref int fromRow, ref int toRow, ref int fromCol, ref int toCol) {
		(int row1, int row2, int col1, int col2) = map[lines[toRow][toCol]];
		if (toRow + row1 == fromRow && toCol + col1 == fromCol) {
			fromRow = toRow;
			fromCol = toCol;
			toRow += row2;
			toCol += col2;
		} else {
			fromRow = toRow;
			fromCol = toCol;
			toRow += row1;
			toCol += col1;
		}
	}
}
