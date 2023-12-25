using System;
using System.Collections.Generic;
using System.IO;

static class Day3 {
	static void Solution(Func<char, int, (int, int), bool> Check) {
		string[] lines = File.ReadAllLines("input3.txt");
		for (int row = 0; row < lines.Length; row++) {
			string line = lines[row];
			for (int col = 0; col < line.Length; col++) {
				char c = line[col];
				if (IsNotDigit(c)) {
					continue;
				}

				//Otherwise found a number. Parse it, then check the surroundings
				int start = col;
				while (++col < line.Length) {
					c = line[col];
					if (IsNotDigit(c)) {
						break;
					}
				}
				int end = col;

				int number = int.Parse(line.AsSpan(start, end - start));
				CheckIsPart();
				void CheckIsPart() {
					for (int otherRow = Math.Max(row - 1, 0); otherRow < Math.Min(row + 2, lines.Length); otherRow++) {
						for (int otherCol = Math.Max(start - 1, 0); otherCol < Math.Min(end + 1, line.Length); otherCol++) {
							if (Check(lines[otherRow][otherCol], number, (otherRow, otherCol))) {
								return;
							}
						}
					}
				}
			}
		}
	}

	public static int Solution1() {
		int sum = 0;
		Solution((c, number, _) => {
			if (c != '.' && IsNotDigit(c)) {
				sum += number;
				return true;
			}
			return false;
		});
		return sum;
	}

	public static int Solution2() {
		Dictionary<(int, int), List<int>> gearNumbers = [];
		Solution((c, number, coord) => {
			if (c == '*') {
				if (gearNumbers.TryGetValue(coord, out List<int>? numbers)) {
					numbers.Add(number);
				} else {
					gearNumbers[coord] = [ number ];
				}
				return true;
			}
			return false;
		});

		int sum = 0;
		foreach (List<int> numbers in gearNumbers.Values) {
			if (numbers.Count == 2) {
				sum += numbers[0] * numbers[1];
			}
		}
		return sum;
	}

	static bool IsNotDigit(char c) => c < '0' || c > '9';
}
