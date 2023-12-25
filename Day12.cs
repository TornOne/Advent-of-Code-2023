using System;
using System.IO;
using System.Threading.Tasks;

static class Day12 {
	public static long Solution1() {
		long count = 0;
		foreach (string line in File.ReadAllLines("input12.txt")) {
			string[] parts = line.Split(' ');
			count += SolveLine(parts[0], Array.ConvertAll(parts[1].Split(','), int.Parse));
		}
		return count;
	}

	public static long Solution2() {
		long totalCount = 0;
		//int lineCounter = 0;
		string[] allLines = File.ReadAllLines("input12.txt");
		Parallel.ForEach(allLines, line => {
			string[] parts = line.Split(' ');
			long count = SolveLine(string.Join('?', [ parts[0], parts[0], parts[0], parts[0], parts[0] ]), Array.ConvertAll(string.Join(',', [ parts[1], parts[1], parts[1], parts[1], parts[1] ]).Split(','), int.Parse));
			lock (allLines) {
				totalCount += count;
				//Console.WriteLine($"{++lineCounter} / {allLines.Length} - {line} - {count}");
			}
		});
		return totalCount;
	}

	static long SolveLine(string springs, int[] pattern) {
		int patternSum = 0;
		foreach (int length in pattern) {
			patternSum += length + 1;
		}
		int[] patternSums = new int[pattern.Length];
		patternSums[0] = patternSum - 1;
		for (int i = 1; i < patternSums.Length; i++) {
			patternSums[i] = patternSums[i - 1] - pattern[i - 1] - 1;
		}
		return CountPatterns('.' + springs, pattern, patternSums, 0, springs.Length - springs.LastIndexOf('#'));
	}

	static long CountPatterns(ReadOnlySpan<char> springs, int[] pattern, int[] patternSums, int patternIndex, int lastSpringFromEnd) {
		if (patternIndex == pattern.Length) { //We've reached the end of the pattern
			return springs.Length < lastSpringFromEnd ? 1 : 0; //If the last spring is further from the end then there are characters left, the solution is valid
		}

		long count = 0;
		int maxPadding = springs.Length - patternSums[patternIndex]; //Padding - empty spaces - has to leave room for the rest of the pattern to fit
		for (int i = 0; i < maxPadding; i++) {
			if (springs[i] != '#') {
				int springsEnd = i + 1 + pattern[patternIndex];
				bool valid = true;
				for (int j = i + 1; j < springsEnd; j++) {
					if (springs[j] == '.') {
						valid = false;
						break;
					}
				}
				if (valid) {
					count += CountPatterns(springs[springsEnd..], pattern, patternSums, patternIndex + 1, lastSpringFromEnd);
				}
			} else {
				return count;
			}
		}

		return count;
	}
}
