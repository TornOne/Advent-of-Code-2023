using System;
using System.IO;

static class Day6 {
	public static int Solution1() {
		int product = 1;
		string[] lines = File.ReadAllLines("input6.txt");
		int[] times = Array.ConvertAll(lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..], int.Parse);
		int[] records = Array.ConvertAll(lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..], int.Parse);
		for (int i = 0; i < times.Length; i++) {
			product *= FindSum(times[i], records[i]);
		}
		return product;
	}

	public static int Solution2() {
		string[] lines = File.ReadAllLines("input6.txt");
		long totalTime = long.Parse(string.Join("", lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..]));
		long record = long.Parse(string.Join("", lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..]));
		return FindSum(totalTime, record);
	}

	static int FindSum(long totalTime, long record) {
		int sum = 0;
		for (long time = 1; time < totalTime; time++) {
			long distance = time * (totalTime - time);
			if (distance > record) {
				sum++;
			}
		}
		return sum;
	}
}
