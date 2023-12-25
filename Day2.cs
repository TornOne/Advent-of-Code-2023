using System;
using System.IO;

static class Day2 {
	static void GetMinCounts(string[] parts, out int minRed, out int minGreen, out int minBlue) {
		minRed = 0;
		minGreen = 0;
		minBlue = 0;
		foreach (string draw in parts.AsSpan(1)) {
			foreach (string cube in draw.Split(", ")) {
				string[] cubeParts = cube.Split(' ');
				int count = int.Parse(cubeParts[0]);
				_ = cubeParts[1] switch {
					"red" => minRed = Math.Max(minRed, count),
					"green" => minGreen = Math.Max(minGreen, count),
					"blue" => minBlue = Math.Max(minBlue, count),
					_ => 0
				};
			}
		}
	}

	public static int Solution1() {
		int sum = 0;
		foreach (string line in File.ReadAllLines("input2.txt")) {
			string[] parts = line.Split(new[] { ": ", "; " }, StringSplitOptions.None);
			GetMinCounts(parts, out int minRed, out int minGreen, out int minBlue);
			if (minRed <= 12 && minGreen <= 13 && minBlue <= 14) {
				sum += int.Parse(parts[0].Split(' ')[1]);
			}
		}
		return sum;
	}

	public static int Solution2() {
		int sum = 0;
		foreach (string line in File.ReadAllLines("input2.txt")) {
			string[] parts = line.Split(new[] { ": ", "; " }, StringSplitOptions.None);
			GetMinCounts(parts, out int minRed, out int minGreen, out int minBlue);
			sum += minRed * minGreen * minBlue;
		}
		return sum;
	}
}
