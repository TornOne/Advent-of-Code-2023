using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

static partial class Day19 {
	public static int Solution1() {
		string[] segments = File.ReadAllText("input19.txt").Split("\n\n");

		//Make a list of workflows
		Dictionary<string, List<Func<(int x, int m, int a, int s), (bool pass, string rule)>>> rules = [];
		char[] ruleSplits = [ '{', ',', '}' ];
		foreach (string line in segments[0].Split('\n')) {
			string[] parts = line.Split(ruleSplits, StringSplitOptions.RemoveEmptyEntries);
			List<Func<(int x, int m, int a, int s), (bool pass, string rule)>> ruleList = [];
			foreach (string ruleStr in parts.AsSpan(1)) {
				Match match = ruleRegex.Match(ruleStr);
				if (match.Success) {
					char rating = match.Groups["rating"].Value[0];
					char sign = match.Groups["sign"].Value[0];
					int value = int.Parse(match.Groups["value"].Value);
					string rule = match.Groups["rule"].Value;
					ruleList.Add(item => ((rating == 'x' ? item.x : rating == 'm' ? item.m : rating == 'a' ? item.a : item.s).CompareTo(value) == (sign == '>' ? 1 : -1), rule));
				} else {
					ruleList.Add(item => (true, ruleStr));
				}
			}
			rules[parts[0]] = ruleList;
		}

		//Process all the items
		int sum = 0;
		foreach (string line in segments[1].Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
			string[] parts = line.Split(['=', ',', '}']);
			(int x, int m, int a, int s) item = (int.Parse(parts[1]), int.Parse(parts[3]), int.Parse(parts[5]), int.Parse(parts[7]));
			string rule = "in";
			while (rule != "A" && rule != "R") {
				foreach (Func<(int x, int m, int a, int s), (bool pass, string rule)> RuleFunc in rules[rule]) {
					(bool pass, string nextRule) = RuleFunc(item);
					if (pass) {
						rule = nextRule;
						break;
					}
				}
			}
			if (rule == "A") {
				sum += item.x + item.m + item.a + item.s;
			}
		}

		return sum;
	}

	public static long Solution2() {
		string[] segments = File.ReadAllText("input19.txt").Split("\n\n");

		//Make a list of workflows
		Dictionary<string, List<Func<ItemGroup, (ItemGroup pass, ItemGroup fail, string rule)>>> rules = [];
		char[] ruleSplits = ['{', ',', '}'];
		foreach (string line in segments[0].Split('\n')) {
			string[] parts = line.Split(ruleSplits, StringSplitOptions.RemoveEmptyEntries);
			List<Func<ItemGroup, (ItemGroup pass, ItemGroup fail, string rule)>> ruleList = [];
			foreach (string ruleStr in parts.AsSpan(1)) {
				Match match = ruleRegex.Match(ruleStr);
				if (match.Success) {
					char rating = match.Groups["rating"].Value[0];
					char sign = match.Groups["sign"].Value[0];
					int value = int.Parse(match.Groups["value"].Value);
					string rule = match.Groups["rule"].Value;
					ruleList.Add(item => {
						Range alteringRange = rating == 'x' ? item.x : rating == 'm' ? item.m : rating == 'a' ? item.a : item.s;
						Range passRange, failRange;
						if (sign == '>') {
							passRange = Math.Max(value + 1, alteringRange.Start.Value)..alteringRange.End;
							failRange = alteringRange.Start..Math.Min(value, alteringRange.End.Value);
						} else {
							passRange = alteringRange.Start..Math.Min(value - 1, alteringRange.End.Value);
							failRange = Math.Max(value, alteringRange.Start.Value)..alteringRange.End;
						}
						if (passRange.Start.Value > passRange.End.Value) {
							passRange = 1..0;
						}
						if (failRange.Start.Value > failRange.End.Value) {
							failRange = 1..0;
						}

						return (new ItemGroup(rating == 'x' ? passRange : item.x, rating == 'm' ? passRange : item.m, rating == 'a' ? passRange : item.a, rating == 's' ? passRange : item.s), new ItemGroup(rating == 'x' ? failRange : item.x, rating == 'm' ? failRange : item.m, rating == 'a' ? failRange : item.a, rating == 's' ? failRange : item.s), rule);
					});
				} else {
					ruleList.Add(item => (item, ItemGroup.Empty, ruleStr));
				}
			}
			rules[parts[0]] = ruleList;
		}

		//Split all the items into ranges
		long sum = 0;
		Stack<(ItemGroup itemRange, string ruleName)> itemRanges = [];
		itemRanges.Push((new ItemGroup(1..4000, 1..4000, 1..4000, 1..4000), "in"));
		while (itemRanges.Count > 0) {
			(ItemGroup failRange, string ruleName) = itemRanges.Pop();
			if (ruleName == "A") {
				sum += (long)(failRange.x.End.Value - failRange.x.Start.Value + 1) * (failRange.m.End.Value - failRange.m.Start.Value + 1) * (failRange.a.End.Value - failRange.a.Start.Value + 1) * (failRange.s.End.Value - failRange.s.Start.Value + 1);
				continue;
			}

			foreach (Func<ItemGroup, (ItemGroup pass, ItemGroup fail, string rule)> RuleFunc in rules[ruleName]) {
				(ItemGroup pass, failRange, string rule) = RuleFunc(failRange);
				if (rule != "R" && !pass.IsEmpty) {
					itemRanges.Push((pass, rule));
				}
				if (failRange.IsEmpty) {
					break;
				}
			}
		}

		return sum;
	}

	[GeneratedRegex("(?'rating'[xmas])(?'sign'[<>])(?'value'\\d+):(?'rule'A|R|[a-z]+)")]
	private static partial Regex RuleRegex();
	static Regex ruleRegex = RuleRegex();

	readonly struct ItemGroup(Range x, Range m, Range a, Range s) {
		public readonly Range x = x, m = m, a = a, s = s;

		public bool IsEmpty => x.End.Value == 0 && m.End.Value == 0 && a.End.Value == 0 && s.End.Value == 0;

		public static readonly ItemGroup Empty = new(1..0, 1..0, 1..0, 1..0);
	}
}
