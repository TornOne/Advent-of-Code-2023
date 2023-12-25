using System.Collections.Generic;
using System.IO;

static class Day15 {
	public static int Solution1() {
		int sum = 0;
		foreach (string step in File.ReadAllText("input15.txt")[..^1].Split(',')) {
			sum += Hash(step);
		}
		return sum;
	}

	public static int Solution2() {
		List<(string label, int focalLength)>[] boxes = new List<(string, int)>[256];
		for (int i = 0; i < boxes.Length; i++) {
			boxes[i] = [];
		}

		foreach (string step in File.ReadAllText("input15.txt")[..^1].Split(',')) {
			string[] parts = step.Split('=', '-');
			string label = parts[0];
			int focalLength = parts[1] == string.Empty ? 0 : int.Parse(parts[1]);
			List<(string label, int focalLength)> box = boxes[Hash(label)];
			int lensIndex = box.FindIndex(lens => lens.label == label);
			if (focalLength == 0) {
				if (lensIndex >= 0) {
					box.RemoveAt(lensIndex);
				}
			} else {
				if (lensIndex >= 0) {
					box[lensIndex] = (label, focalLength);
				} else {
					box.Add((label, focalLength));
				}
			}
		}

		int sum = 0;
		for (int i = 0; i < boxes.Length; i++) {
			List<(string label, int focalLength)> box = boxes[i];
			for (int j = 0; j < box.Count; j++) {
				sum += (i + 1) * (j + 1) * box[j].focalLength;
			}
		}
		return sum;
	}

	static int Hash(string value) {
		int hash = 0;
		foreach (char c in value) {
			hash += c;
			hash *= 17;
			hash %= 256;
		}
		return hash;
	}
}
