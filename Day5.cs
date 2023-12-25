using System;
using System.Collections.Generic;
using System.IO;

static class Day5 {
	public static uint Solution1() {
		string[] sections = File.ReadAllText("input5.txt").Split("\n\n");
		uint[] seeds = Array.ConvertAll(sections[0].Split(' ')[1..], uint.Parse);

		for (int i = 1; i < sections.Length; i++) {
			(uint from, uint shift, uint length)[] map = ParseMap(sections[i]);
			for (int j = 0; j < seeds.Length; j++) {
				uint seed = seeds[j];
				foreach ((uint from, uint shift, uint length) in map) {
					if (seed >= from && seed < from + length) {
						seeds[j] = seed + shift;
						break;
					}
				}
			}
		}

		uint minSeed = uint.MaxValue;
		foreach (uint seed in seeds) {
			minSeed = Math.Min(seed, minSeed);
		}
		return minSeed;
	}

	public static uint Solution2() {
		string[] sections = File.ReadAllText("input5.txt").Split("\n\n");
		List<(uint from, uint to)> seedRanges = [];
		uint[] seeds = Array.ConvertAll(sections[0].Split(' ')[1..], uint.Parse);
		for (int i = 0; i < seeds.Length / 2; i++) {
			uint from = seeds[i * 2];
			seedRanges.Add((from, from + seeds[i * 2 + 1] - 1));
		}

		for (int i = 1; i < sections.Length; i++) {
			(uint from, uint shift, uint length)[] map = ParseMap(sections[i]);

			int ProcessSeedRangesSection(int seedRangeFrom, int seedRangeTo) {
				for (int j = seedRangeFrom; j < seedRangeTo; j++) {
					(uint seedFrom, uint seedTo) = seedRanges[j];
					foreach ((uint from, uint shift, uint length) in map) {
						if (seedFrom >= from && seedFrom < from + length) { //At least part of the seed range is here
							if (seedTo < from + length) { //The entire range fits
								seedRanges[j] = (seedFrom + shift, seedTo + shift);
							} else { //Part of the range does not fit, and will need to be processed again
								uint rangeEnd = from + length;
								seedRanges[j] = (seedFrom + shift, rangeEnd - 1 + shift);
								seedRanges.Add((rangeEnd, seedTo));
							}
							break;
						}
					}
				}
				return seedRangeTo;
			}

			int processed = 0;
			while (processed < seedRanges.Count) {
				processed = ProcessSeedRangesSection(processed, seedRanges.Count);
			}
		}

		uint minSeed = uint.MaxValue;
		foreach ((uint from, _) in seedRanges) {
			minSeed = Math.Min(from, minSeed);
		}
		return minSeed;
	}

	static (uint, uint, uint)[] ParseMap(string section) {
		Span<string> lines = section.Split('\n', StringSplitOptions.RemoveEmptyEntries).AsSpan(1);
		(uint from, uint shift, uint length)[] map = new (uint, uint, uint)[lines.Length];
		for (int j = 0; j < lines.Length; j++) {
			uint[] parts = Array.ConvertAll(lines[j].Split(' '), uint.Parse);
			map[j] = (parts[1], parts[0] - parts[1], parts[2]);
		}
		return map;
	}
}
