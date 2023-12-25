using System.IO;

static class Day1 {
	public static int Solution1() {
		int sum = 0;
		foreach (string line in File.ReadAllLines("input1.txt")) {
			foreach (char c in line) {
				if (c >= '1' && c <= '9') {
					sum += (c - '0') * 10;
					break;
				}
			}

			for (int i = line.Length - 1; i >= 0; i--) {
				char c = line[i];
				if (c >= '1' && c <= '9') {
					sum += c - '0';
					break;
				}
			}
		}

		return sum;
	}

	public static int Solution2() {
		string[] digits = [ "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" ];
		int sum = 0;
		foreach (string line in File.ReadAllLines("input1.txt")) {
			int first = line.Length - 1;
			int digit = 0;
			for (int i = 0; i < digits.Length; i++) {
				int index = line.IndexOf(digits[i]);
				if (index >= 0 && index < first) {
					first = index;
					digit = i + 1;
				}
			}

			for (int i = 0; i <= first; i++) {
				char c = line[i];
				if (c >= '1' && c <= '9') {
					digit = c - '0';
					break;
				}
			}
			sum += digit * 10;

			first = 0;
			for (int i = 0; i < digits.Length; i++) {
				int index = line.LastIndexOf(digits[i]);
				if (index >= 0 && index > first) {
					first = index;
					digit = i + 1;
				}
			}

			for (int i = line.Length - 1; i >= first; i--) {
				char c = line[i];
				if (c >= '1' && c <= '9') {
					digit = c - '0';
					break;
				}
			}
			sum += digit;
		}

		return sum;
	}
}
