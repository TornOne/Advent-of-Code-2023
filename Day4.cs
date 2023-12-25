using System;
using System.Collections.Generic;
using System.IO;

static class Day4 {
	public static int Solution1() {
		int sum = 0;

		foreach (string line in File.ReadAllLines("input4.txt")) {
			int winCount = CountWins(line);
			sum += winCount > 2 ? 1 << winCount - 1 : winCount;
		}

		return sum;
	}

	public static int Solution2() {
		int sum = 0;
		string[] lines = File.ReadAllLines("input4.txt");
		int[] cardCopies = new int[lines.Length];
		Array.Fill(cardCopies, 1);

		for (int i = 0; i < lines.Length; i++) {
			int winCount = CountWins(lines[i]);
			for (int iCopy = i + 1; iCopy <= i + winCount; iCopy++) {
				cardCopies[iCopy] += cardCopies[i];
			}
			sum += cardCopies[i];
		}

		return sum;
	}

	static int CountWins(string line) {
		int winCount = 0;
		string[] parts = line.Split([ ": ", " | " ], StringSplitOptions.None);
		HashSet<int> winningNumbers = new(Array.ConvertAll(parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries), int.Parse));
		foreach (int number in Array.ConvertAll(parts[2].Split(' ', StringSplitOptions.RemoveEmptyEntries), int.Parse)) {
			if (winningNumbers.Contains(number)) {
				winCount++;
			}
		}
		return winCount;
	}
}
