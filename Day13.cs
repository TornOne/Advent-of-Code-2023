using System;
using System.IO;

static class Day13 {
	public static int Solution1() {
		int sum = 0;
		foreach (string[] pattern in Array.ConvertAll(File.ReadAllText("input13.txt").Split("\n\n"), pattern => pattern.Split('\n', StringSplitOptions.RemoveEmptyEntries))) {
			//Check horizontal
			for (int y = 1; y < pattern.Length; y++) {
				if (pattern[y - 1] == pattern[y]) { //One line matches, also check all the others
					bool valid = true;
					for (int offset = 1; offset < Math.Min(y, pattern.Length - y); offset++) {
						if (pattern[y - 1 - offset] != pattern[y + offset]) {
							valid = false;
							break;
						}
					}
					if (valid) {
						sum += 100 * y;
						break;
					}
				}
			}

			//Check vertical
			int width = pattern[0].Length;
			bool ColsEqual(int x1, int x2) => Array.TrueForAll(pattern, row => row[x1] == row[x2]);
			for (int x = 1; x < width; x++) {
				if (ColsEqual(x - 1, x)) { //One line matches, also check all the others
					bool valid = true;
					for (int offset = 1; offset < Math.Min(x, width - x); offset++) {
						if (!ColsEqual(x - 1 - offset, x + offset)) {
							valid = false;
							break;
						}
					}
					if (valid) {
						sum += x;
						break;
					}
				}
			}
		}
		return sum;
	}

	public static int Solution2() {
		int sum = 0;
		foreach (string[] pattern in Array.ConvertAll(File.ReadAllText("input13.txt").Split("\n\n"), pattern => pattern.Split('\n', StringSplitOptions.RemoveEmptyEntries))) {
			//Check horizontal
			static int GetRowDelta(string row1, string row2) {
				int delta = 0;
				for (int i = 0; i < row1.Length; i++) {
					delta += row1[i] == row2[i] ? 0 : 1;
					if (delta > 1) {
						return delta;
					}
				}
				return delta;
			}
			for (int y = 1; y < pattern.Length; y++) {
				int error = GetRowDelta(pattern[y - 1], pattern[y]);
				if (error <= 1) { //One line matches, also check all the others
					for (int offset = 1; offset < Math.Min(y, pattern.Length - y); offset++) {
						error += GetRowDelta(pattern[y - 1 - offset], pattern[y + offset]);
						if (error > 1) {
							break;
						}
					}
					if (error == 1) {
						sum += 100 * y;
						break;
					}
				}
			}

			//Check vertical
			int width = pattern[0].Length;
			int GetColDelta(int x1, int x2) {
				int delta = 0;
				foreach (string row in pattern) {
					delta += row[x1] == row[x2] ? 0 : 1;
					if (delta > 1) {
						return delta;
					}
				}
				return delta;
			}
			for (int x = 1; x < width; x++) {
				int error = GetColDelta(x - 1, x);
				if (error <= 1) { //One line matches, also check all the others
					for (int offset = 1; offset < Math.Min(x, width - x); offset++) {
						error += GetColDelta(x - 1 - offset, x + offset);
						if (error > 1) {
							break;
						}
					}
					if (error == 1) {
						sum += x;
						break;
					}
				}
			}
		}
		return sum;
	}
}
