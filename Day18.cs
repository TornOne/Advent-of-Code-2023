using System;
using System.Collections.Generic;
using System.IO;

static class Day18 {
	public static int Solution1() {
		HashSet<(int x, int y)> map = [ (0, 0) ];
		(int x, int y) loc = (0, 0);
		foreach (string line in File.ReadAllLines("input18.txt")) {
			string[] parts = line.Split(' ');
			char dir = parts[0][0];
			int len = int.Parse(parts[1]);

			for (int i = 0; i < len; i++) {
				loc = dir switch {
					'U' => (loc.x, loc.y - 1),
					'R' => (loc.x + 1, loc.y),
					'D' => (loc.x, loc.y + 1),
					_ => (loc.x - 1, loc.y)
				};
				map.Add(loc);
			}
		}

		//Flood fill
		Stack<(int x, int y)> queue = [];
		queue.Push((1, 1));
		map.Add((1, 1));
		void TryAdd((int, int) pos) {
			if (map.Add(pos)) {
				queue.Push(pos);
			}
		}
		while (queue.Count > 0) {
			loc = queue.Pop();
			TryAdd((loc.x, loc.y - 1));
			TryAdd((loc.x + 1, loc.y));
			TryAdd((loc.x, loc.y + 1));
			TryAdd((loc.x - 1, loc.y));
		}

		return map.Count;
	}

	public static long Solution2() {
		//Track all the lines
		List<(int fromX, int fromY, int toX, int toY)> lines = [];
		(int x, int y) loc = (0, 0);
		foreach (string line in File.ReadAllLines("input18.txt")) {
			string[] parts = line.Split(' ');
			char dir = parts[2][7];
			int len = Convert.ToInt32(parts[2][2..7], 16) * 2; //Extend twofold to get rid of decimal points later on
			(int x, int y) newLoc = dir switch {
				'3' => (loc.x, loc.y - len),
				'0' => (loc.x + len, loc.y),
				'1' => (loc.x, loc.y + len),
				_ => (loc.x - len, loc.y)
			};
			lines.Add((loc.x, loc.y, newLoc.x, newLoc.y));
			loc = newLoc;
		}

		//Shift the (horizontal) lines into place
		for (int i = 0; i < lines.Count; i++) {
			(int fromX, int fromY, int toX, int toY) = lines[i];
			if (fromX == toX) { //Vertical - skip
				continue;
			}

			if (toX > fromX) { //Right
				fromY--;
				toY--;
			} else { //Left
				fromY++;
				toY++;
			}

			(int, int fromY, int, int toY) next = lines[i + 1];
			if (next.toY > next.fromY) { //Down
				toX++;
			} else { //Up
				toX--;
			}

			(int, int fromY, int, int toY) last = lines[(i == 0 ? lines.Count : i) - 1];
			if (last.toY > last.fromY) { //Down
				fromX++;
			} else { //Up
				fromX--;
			}

			lines[i] = (fromX, fromY, toX, toY);
		}

		//Find the minimal Y value
		int minY = int.MaxValue;
		foreach ((int fromX, int fromY, int toX, int toY) in lines) {
			if (fromX != toX) {
				minY = Math.Min(minY, Math.Min(fromY, toY));
			}
		}

		//Sum the area
		long sum = 0;
		foreach ((int fromX, int fromY, int toX, int toY) in lines) {
			if (fromX != toX) { //Horizontal
				sum += (long)(fromX - toX) * (fromY - minY);
			}
		}

		return sum / 4; //Undo the extension
	}
}
