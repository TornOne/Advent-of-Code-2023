using System;
using System.Collections.Generic;
using System.IO;

static class Day8 {
	public static int Solution1() {
		Dictionary<string, (string left, string right)> map = MakeMap(out string instructions);
		string current = "AAA";
		int steps = 0;
		while (current != "ZZZ") {
			current = instructions[steps % instructions.Length] == 'L' ? map[current].left : map[current].right;
			steps++;
		}
		return steps;
	}

	public static long Solution2() {
		Dictionary<string, (string left, string right)> map = MakeMap(out string instructions);
		List<string> current = [];
		foreach (string pos in map.Keys) {
			if (pos[2] == 'A') {
				current.Add(pos);
			}
		}
		int[] steps = new int[current.Count];
		for (int i = 0; i < current.Count; i++) {
			int step = 0;
			string pos = current[i];
			while (pos[2] != 'Z') {
				pos = instructions[step % instructions.Length] == 'L' ? map[pos].left : map[pos].right;
				step++;
			}
			steps[i] = step;
		}

		Dictionary<int, int> allPrimeFactors = [];
		foreach (int step in steps) {
			int value = step;
			Dictionary<int, int> primeFactors = [];
			for (int i = 2; value > 1; i++) {
				if (value % i == 0) {
					value /= i;
					primeFactors[i] = primeFactors.TryGetValue(i, out int count) ? count + 1 : 1;
					i--; //Retry the same number
				}
			}
			foreach (KeyValuePair<int, int> primeFactor in primeFactors) {
				allPrimeFactors[primeFactor.Key] = allPrimeFactors.TryGetValue(primeFactor.Key, out int count) ? Math.Max(count, primeFactor.Value) : primeFactor.Value;
			}
		}
		long product = 1;
		foreach (KeyValuePair<int, int> primeFactor in allPrimeFactors) {
			for (int i = 0; i < primeFactor.Value; i++) {
				product *= primeFactor.Key;
			}
		}

		return product;
	}

	static Dictionary<string, (string left, string right)> MakeMap(out string instructions) {
		string[] lines = File.ReadAllLines("input8.txt");
		instructions = lines[0];
		Dictionary<string, (string left, string right)> map = new(lines.Length - 2);
		foreach (string line in lines.AsSpan(2)) {
			map[line[..3]] = (line[7..10], line[12..15]);
		}
		return map;
	}
}
