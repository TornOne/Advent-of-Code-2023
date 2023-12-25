using System;
using System.Collections.Generic;
using System.IO;

static class Day11 {
	public static long Solution(int expansion) {
		bool[][] universe = Array.ConvertAll(File.ReadAllLines("input11.txt"), line => Array.ConvertAll(line.ToCharArray(), c => c == '#'));
		List<int> horizontalExpansions = [];
		for (int i = 0; i < universe.Length; i++) {
			if (Array.TrueForAll(universe[i], x => !x)) {
				horizontalExpansions.Add(i);
			}
		}
		List<int> verticalExpansions = [];
		for (int i = 0; i < universe[0].Length; i++) {
			if (Array.TrueForAll(universe, row => !row[i])) {
				verticalExpansions.Add(i);
			}
		}

		List<(int x, int y)> galaxies = [];
		for (int y = 0; y < universe.Length; y++) {
			for (int x = 0; x < universe[0].Length; x++) {
				if (universe[y][x]) {
					galaxies.Add((x + ~verticalExpansions.BinarySearch(x) * expansion, y + ~horizontalExpansions.BinarySearch(y) * expansion));
				}
			}
		}

		long sum = 0;
		for (int i = 0; i < galaxies.Count; i++) {
			for (int j = i + 1; j < galaxies.Count; j++) {
				sum += Math.Abs(galaxies[i].x - galaxies[j].x) + Math.Abs(galaxies[i].y - galaxies[j].y);
			}
		}
		return sum;
	}

	public static long Solution1() => Solution(1);

	public static long Solution2() => Solution(999999);
}
