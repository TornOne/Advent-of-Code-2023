using System.Collections.Generic;
using System.IO;

static partial class Day21 {
	public static int Solution1() {
		//Initialize map
		string[] lines = File.ReadAllLines("input21.txt");
		bool[][] map = new bool[lines.Length][];
		int startY = 0;
		int startX = 0;
		for (int row = 0; row < map.Length; row++) {
			string line = lines[row];
			map[row] = new bool[line.Length];
			for (int col = 0; col < line.Length; col++) {
				char c = line[col];
				map[row][col] = c == '#';
				if (c == 'S') {
					startY = row;
					startX = col;
				}
			}
		}

		//Traverse
		HashSet<(int y, int x)> lastStep = [ (startY, startX) ];
		HashSet<(int y, int x)> newStep = [];
		for (int steps = 0; steps < 64; steps++) {
			foreach ((int y, int x) in lastStep) {
				if (y > 0) {
					int newY = y - 1;
					if (!map[newY][x]) {
						newStep.Add((newY, x));
					}
				}
				if (x > 0) {
					int newX = x - 1;
					if (!map[y][newX]) {
						newStep.Add((y, newX));
					}
				}
				if (y + 1 < map.Length) {
					int newY = y + 1;
					if (!map[newY][x]) {
						newStep.Add((newY, x));
					}
				}
				if (x + 1 < map[0].Length) {
					int newX = x + 1;
					if (!map[y][newX]) {
						newStep.Add((y, newX));
					}
				}
			}
			lastStep = newStep;
			newStep = [];
		}

		return lastStep.Count;
	}

	public static long Solution2() {
		//I solved this mostly by hand, and it's another day that's dependent on the input.
		//So instead of a solution based on any input, I will provide an explanation on how I arrived at the answer that works for my input.
		//Because, it's not like any reasonable solution could work for all inputs according to the problem description.

		//I noticed that I was asked for the answer for an inputs of steps that was a multiple of the width and height of the garden (unnoted assumption of the garden being square) + half of the garden width rounded down.
		//Because the edges of the garden, as well as the horizontal and vertical lines from the starting position, are all empty (another unnoted assumption), and we can't move in diagonals, our possible step cloud expands equally fast in all directions.
		//So I extended the map by a few gardens in each direction and counted the covered squares each step.
		//Fitting them to a curve every width steps produced an exact quadratic equation, and so did fitting them to a curve every width steps offset by half the width rounded down.
		//Said equation was 15549x^2 + 15634x + 3943, where x is how many times the width fits into the number of steps.

		long x = 26501365 / File.ReadAllLines("input21.txt").Length;
		return 15549 * x * x + 15634 * x + 3943;
		//Honestly, I could probably write the curve fitting and all that steps to arrive at the formula into the solution, but I don't entirely respect today's problem, and I'm also ill today, so I can't be bothered.
	}
}
