using System;
using System.Collections.Generic;
using System.IO;

static class Day9 {
	static int Solution(Func<int[], int, int> Op) {
		int sum = 0;
		foreach (string line in File.ReadAllLines("input9.txt")) {
			int[] lastValues = Array.ConvertAll(line.Split(' '), int.Parse);
			List<int[]> histories = new() { lastValues };
			while (Array.Exists(lastValues, value => value != 0)) {
				int[] newValues = new int[lastValues.Length - 1];
				for (int i = 0; i < newValues.Length; i++) {
					newValues[i] = lastValues[i + 1] - lastValues[i];
				}
				histories.Add(newValues);
				lastValues = newValues;
			}
			int nextValue = 0;
			for (int i = histories.Count - 2; i >= 0; i--) {
				nextValue = Op(histories[i], nextValue);
			}
			sum += nextValue;
		}
		return sum;
	}

	public static int Solution1() => Solution((history, nextValue) => history[^1] + nextValue);

	public static int Solution2() => Solution((history, nextValue) => history[0] - nextValue);
}
