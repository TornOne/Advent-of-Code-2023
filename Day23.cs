using System;
using System.Collections.Generic;
using System.IO;

static partial class Day23 {
	public static int Solution1() {
		int[][] map = GetMap(out int width, out int height, out Dictionary<int, int> nodeLengths);

		//Find the edges connecting the paths
		List<(int from, int to)> edges = [];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int tile = map[y][x];
				if (tile == Up) {
					edges.Add((map[y + 1][x], map[y - 1][x]));
				} else if (tile == Right) {
					edges.Add((map[y][x - 1], map[y][x + 1]));
				} else if (tile == Down) {
					edges.Add((map[y - 1][x], map[y + 1][x]));
				} else if (tile == Left) {
					edges.Add((map[y][x + 1], map[y][x - 1]));
				}
			}
		}

		//Find the longest path
		int[] distances = new int[nodeLengths.Count];
		for (int repeat = 0; repeat < nodeLengths.Count - 1; repeat++) {
			foreach ((int from, int to) in edges) {
				int newDistance = distances[from] + nodeLengths[from] + 1;
				if (newDistance > distances[to]) {
					distances[to] = newDistance;
				}
			}
		}

		int exitNode = map[height - 1][width - 2];
		return distances[exitNode] + nodeLengths[exitNode] - 1;
	}

	public static int Solution2() {
		int[][] map = GetMap(out int width, out int height, out Dictionary<int, int> nodeLengths);

		//Find all the crossroads
		Dictionary<int, List<int>> crossroads = [];
		foreach (KeyValuePair<int, int> node in nodeLengths) {
			if (node.Value == 1) {
				crossroads.Add(node.Key, []);
			}
		}

		//Find the edges connecting the paths
		Dictionary<int, List<int>> paths = [];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int a, b;
				int tile = map[y][x];
				if (tile == Up || tile == Down) {
					a = map[y - 1][x];
					b = map[y + 1][x];
				} else if (tile == Right || tile == Left) {
					a = map[y][x - 1];
					b = map[y][x + 1];
				} else {
					continue;
				}

				if (crossroads.ContainsKey(a)) {
					(a, b) = (b, a); //a shall be the path, and b the crossroad
				}

				crossroads[b].Add(a);
				nodeLengths[a]++;
				if (paths.TryGetValue(a, out List<int>? bs)) {
					bs.Add(b);
				} else {
					paths[a] = [ b ];
				}
			}
		}

		//Find the longest path
		int exitNode = map[height - 1][width - 2];
		int Traverse(int currentNode) {
			int crossroad = paths[currentNode][0];
			if (!crossroads.TryGetValue(crossroad, out List<int>? possibleExits)) {
				return int.MinValue; //Dead end
			}

			crossroads.Remove(crossroad);
			int maxDistance = int.MinValue;
			foreach (int possibleExit in possibleExits) {
				if (possibleExit == currentNode) {
					continue; //Can't go back
				} else if (possibleExit == exitNode) {
					maxDistance = nodeLengths[exitNode]; //Must go to the exit if possible
					break;
				}

				paths[possibleExit].Remove(crossroad);
				maxDistance = Math.Max(maxDistance, Traverse(possibleExit));
				paths[possibleExit].Add(crossroad);
			}
			crossroads[crossroad] = possibleExits;
			return nodeLengths[currentNode] + 1 + maxDistance;
		}
		return Traverse(0) - 1;
	}

	const int Up = int.MaxValue;
	const int Right = int.MaxValue - 1;
	const int Down = int.MaxValue - 2;
	const int Left = int.MaxValue - 3;
	const int Wall = int.MaxValue - 4;
	const int Floor = -1;

	static int[][] GetMap(out int width, out int height, out Dictionary<int, int> nodeLengths) {
		//Convert the map to a better representation
		int[][] map = Array.ConvertAll(File.ReadAllLines("input23.txt"), line => Array.ConvertAll(line.ToCharArray(), c => c switch {
			'.' => Floor,
			'#' => Wall,
			'^' => Up,
			'>' => Right,
			'v' => Down,
			'<' => Left,
			_ => throw new Exception()
		}));
		height = map.Length;
		width = map[0].Length;

		//Flood fill all the continuous paths
		nodeLengths = [];
		int nextNode = 0;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (map[y][x] == Floor) {
					nodeLengths[nextNode] = FloodFill(map, y, x, nextNode);
					nextNode++;
				}
			}
		}

		return map;
	}

	static int FloodFill(int[][] map, int yStart, int xStart, int id) {
		int size = 0;
		map[yStart][xStart] = id;
		Stack<(int y, int x)> queue = [];
		queue.Push((yStart, xStart));
		while (queue.Count > 0) {
			size++;
			(int y, int x) = queue.Pop();
			if (y > 0 && map[y - 1][x] == Floor) { //Up
				map[y - 1][x] = id;
				queue.Push((y - 1, x));
			}
			if (map[y][x + 1] == Floor) { //Right
				map[y][x + 1] = id;
				queue.Push((y, x + 1));
			}
			if (y + 1 < map.Length && map[y + 1][x] == Floor) { //Down
				map[y + 1][x] = id;
				queue.Push((y + 1, x));
			}
			if (map[y][x - 1] == Floor) { //Left
				map[y][x - 1] = id;
				queue.Push((y, x - 1));
			}
		}
		return size;
	}
}
