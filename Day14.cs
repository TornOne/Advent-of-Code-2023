using System;
using System.Collections.Generic;
using System.IO;

static class Day14 {
	public static int Solution1() {
		int load = 0;
		char[][] platform = Array.ConvertAll(File.ReadAllLines("input14.txt"), row => row.ToCharArray());
		int rows = platform.Length;
		int cols = platform[0].Length;
		for (int col = 0; col < cols; col++) {
			int openRow = 0;
			for (int row = 0; row < rows; row++) {
				if (platform[row][col] == '#') {
					openRow = row + 1;
				} else if (platform[row][col] == 'O') {
					load += rows - openRow;
					openRow++;
				}
			}
		}
		return load;
	}

	public static int Solution2() {
		char[][] platform = Array.ConvertAll(File.ReadAllLines("input14.txt"), row => row.ToCharArray());
		const uint allCycles = 4000000000;
		uint cycle = 0;
		Dictionary<State, uint> history = [];
		Action<char[][]>[] tilts = [ TiltNorth, TiltWest, TiltSouth, TiltEast ];
		while (history.TryAdd(new State(platform, cycle), cycle)) {
			tilts[cycle % 4](platform);
			cycle++;
		}

		uint period = cycle - history[new State(platform, cycle)];
		cycle += (allCycles - cycle) / period * period;
		while (cycle < allCycles) {
			tilts[cycle % 4](platform);
			cycle++;
		}
		
		int load = 0;
		int rows = platform.Length;
		int cols = platform[0].Length;
		for (int row = 0; row < rows; row++) {
			for (int col = 0; col < cols; col++) {
				if (platform[row][col] == 'O') {
					load += rows - row;
				}
			}
		}
		return load;
	}

	class State {
		public enum NextTilt { North, West, South, East }

		public readonly NextTilt nextTilt;
		public readonly int rows, cols;
		public readonly char[][] platform;

		public State(char[][] platform, uint cycle) {
			nextTilt = (NextTilt)(cycle % 4);
			rows = platform.Length;
			cols = platform[0].Length;
			this.platform = new char[rows][];
			for (int row = 0; row < rows; row++) {
				this.platform[row] = platform[row][..];
			}
		}

		public override bool Equals(object? obj) {
			if (obj is not State otherState) {
				return false;
			}

			if (nextTilt != otherState.nextTilt) {
				return false;
			}
			
			for (int row = 0; row < rows; row++) {
				for (int col = 0; col < cols; col++) {
					if (platform[row][col] != otherState.platform[row][col]) {
						return false;
					}
				}
			}

			return true;
		}

		public override int GetHashCode() {
			int hashCode = (int)nextTilt;
			for (int row = 0; row < rows; row++) {
				for (int col = 0; col < cols; col++) {
					if (platform[row][col] == 'O') {
						hashCode ^= 1 << ((row * cols + col) % 32);
					}
				}
			}
			return hashCode;
		}
	}

	static void TiltNorth(char[][] platform) {
		int rows = platform.Length;
		int cols = platform[0].Length;
		for (int col = 0; col < cols; col++) {
			int openRow = 0;
			for (int row = 0; row < rows; row++) {
				if (platform[row][col] == '#') {
					openRow = row + 1;
				} else if (platform[row][col] == 'O') {
					platform[row][col] = '.';
					platform[openRow][col] = 'O';
					openRow++;
				}
			}
		}
	}

	static void TiltSouth(char[][] platform) {
		int rows = platform.Length;
		int cols = platform[0].Length;
		for (int col = 0; col < cols; col++) {
			int openRow = rows - 1;
			for (int row = rows - 1; row >= 0; row--) {
				if (platform[row][col] == '#') {
					openRow = row - 1;
				} else if (platform[row][col] == 'O') {
					platform[row][col] = '.';
					platform[openRow][col] = 'O';
					openRow--;
				}
			}
		}
	}

	static void TiltWest(char[][] platform) {
		int rows = platform.Length;
		int cols = platform[0].Length;
		for (int row = 0; row < rows; row++) {
			int openCol = 0;
			for (int col = 0; col < cols; col++) {
				if (platform[row][col] == '#') {
					openCol = col + 1;
				} else if (platform[row][col] == 'O') {
					platform[row][col] = '.';
					platform[row][openCol] = 'O';
					openCol++;
				}
			}
		}
	}

	static void TiltEast(char[][] platform) {
		int rows = platform.Length;
		int cols = platform[0].Length;
		for (int row = 0; row < rows; row++) {
			int openCol = cols - 1;
			for (int col = cols - 1; col >= 0; col--) {
				if (platform[row][col] == '#') {
					openCol = col - 1;
				} else if (platform[row][col] == 'O') {
					platform[row][col] = '.';
					platform[row][openCol] = 'O';
					openCol--;
				}
			}
		}
	}
}
