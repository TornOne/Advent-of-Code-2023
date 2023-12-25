using System;
using System.Collections.Generic;
using System.IO;

static class Day16 {
	public static int Solution1() {
		string[] map = File.ReadAllLines("input16.txt");
		return CountCoverage(map, map.Length, map[0].Length, (0, 0, 1));
	}

	public static int Solution2() {
		string[] map = File.ReadAllLines("input16.txt");
		int rows = map.Length;
		int cols = map[0].Length;

		int largestCoverage = 0;
		for (int col = 0; col < cols; col++) {
			largestCoverage = Math.Max(CountCoverage(map, rows, cols, (0, col, 2)), largestCoverage);
			largestCoverage = Math.Max(CountCoverage(map, rows, cols, (rows - 1, col, 0)), largestCoverage);
		}
		for (int row = 0; row < rows; row++) {
			largestCoverage = Math.Max(CountCoverage(map, rows, cols, (row, 0, 1)), largestCoverage);
			largestCoverage = Math.Max(CountCoverage(map, rows, cols, (row, cols - 1, 3)), largestCoverage);
		}
		return largestCoverage;
	}

	static int CountCoverage(string[] map, int rows, int cols, (int row, int col, int dir) start) {
		//All splitters are traversed in both directions simultaneously (so we only store them in horizontalTraversed). Mirrors are considered to be horizontally traversed if a beam entered or exited from the left, and vertically if from the right.
		HashSet<(int row, int col)> horizontalTraversed = [];
		HashSet<(int row, int col)> verticalTraversed = [];
		Stack<(int row, int col, int dir)> queue = []; //0=N, 1=E, 2=S, 3=W
		horizontalTraversed.Add((start.row, start.col));
		queue.Push(start);

		while (queue.Count > 0) {
			(int row, int col, int dir) = queue.Pop();
			char tile = map[row][col];

			(int row, int col, int dir) next = tile == '.' ? dir switch {
				0 => (row - 1, col, dir),
				1 => (row, col + 1, dir),
				2 => (row + 1, col, dir),
				_ => (row, col - 1, dir)
			} : next = tile == '/' ? dir switch {
				0 => (row, col + 1, 1),
				1 => (row - 1, col, 0),
				2 => (row, col - 1, 3),
				_ => (row + 1, col, 2)
			} : next = tile == '\\' ? dir switch {
				0 => (row, col - 1, 3),
				1 => (row + 1, col, 2),
				2 => (row, col + 1, 1),
				_ => (row - 1, col, 0)
			} : tile == '|' ? (row + 1, col, 2) : (row, col + 1, 1); //Technically, splitters react the same regardless of whether they split or pass the beam

			if (next.row < 0 || next.row >= rows || next.col < 0 || next.col >= cols) {
				if (tile == '|') { //The other side of a splitter is surely not the off the board
					next = (row - 1, col, 0);
				} else if (tile == '-') {
					next = (row, col - 1, 3);
				} else {
					continue;
				}
			}

		QueueTile:
			char nextTile = map[next.row][next.col];
			if (nextTile == '.') {
				if ((next.dir % 2 == 1 ? horizontalTraversed : verticalTraversed).Add((next.row, next.col))) {
					queue.Push(next);
				}
			} else if (nextTile == '/') {
				if ((next.dir == 1 || next.dir == 2 ? horizontalTraversed : verticalTraversed).Add((next.row, next.col))) {
					queue.Push(next);
				}
			} else if (nextTile == '\\') {
				if ((next.dir == 0 || next.dir == 1 ? horizontalTraversed : verticalTraversed).Add((next.row, next.col))) {
					queue.Push(next);
				}
			} else { //'|' or '-'
				if (horizontalTraversed.Add((next.row, next.col))) {
					queue.Push(next);
				}
			}

			if ((tile == '|' || tile == '-') && (next.dir == 1 || next.dir == 2)) {
				next = tile == '|' ? (row - 1, col, 0) : (row, col - 1, 3);
				if (next.row < 0 || next.row >= rows || next.col < 0 || next.col >= cols) {
					continue;
				}
				goto QueueTile;
			}
		}

		int coverage = 0;
		for (int row = 0; row < rows; row++) {
			for (int col = 0; col < cols; col++) {
				if (horizontalTraversed.Contains((row, col)) || verticalTraversed.Contains((row, col))) {
					coverage++;
				}
			}
		}
		return coverage;
	}
}
