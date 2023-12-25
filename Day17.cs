using System;
using System.Collections.Generic;
using System.IO;

static class Day17 {
	public static int Solution1() {
		int[][] map = Array.ConvertAll(File.ReadAllLines("input17.txt"), line => Array.ConvertAll(line.ToCharArray(), c => c - '0'));
		int width = map[0].Length;
		int height = map.Length;

		//Initialize the map
		int[][][][] shortestDistanceMap = new int[4][][][]; //Direction, Distance, Y, X
		for (Dir dir = Dir.N; dir <= Dir.W; dir++) {
			shortestDistanceMap[(int)dir] = new int[3][][];
			for (int dist = 0; dist < 3; dist++) {
				shortestDistanceMap[(int)dir][dist] = new int[height][];
				for (int y = 0; y < height; y++) {
					shortestDistanceMap[(int)dir][dist][y] = new int[width];
					Array.Fill(shortestDistanceMap[(int)dir][dist][y], int.MaxValue);
				}
			}
		}
		shortestDistanceMap[(int)Dir.E][0][0][1] = map[0][1];
		shortestDistanceMap[(int)Dir.S][0][1][0] = map[1][0];

		//Keep a list of visited and unvisited nodes
		HashSet<(Dir, int, int, int)> visited = [];
		HashSet<(Dir, int, int, int)> unvisited = [];
		unvisited.Add((Dir.E, 0, 0, 1));
		unvisited.Add((Dir.S, 0, 1, 0));

		//Find the minimum distance to every possible state
		while (unvisited.Count > 0) {
			//Get the unvisited node with the smallest distance to it
			(Dir dir, int dist, int y, int x) minNode = default;
			int minValue = int.MaxValue;
			foreach ((Dir dir, int dist, int y, int x) node in unvisited) {
				int value = shortestDistanceMap[(int)node.dir][node.dist][node.y][node.x];
				if (value < minValue) {
					minNode = node;
					minValue = value;
				}
			}
			visited.Add(minNode);
			unvisited.Remove(minNode);

			//Add all potential further directions
			void TryAddNode((Dir dir, int dist, int y, int x) node) {
				if (!visited.Contains(node)) {
					unvisited.Add(node);
					shortestDistanceMap[(int)node.dir][node.dist][node.y][node.x] = Math.Min(shortestDistanceMap[(int)node.dir][node.dist][node.y][node.x], minValue + map[node.y][node.x]);
				}
			}
			if (minNode.y > 0 && minNode.dir != Dir.S && (minNode.dir != Dir.N || minNode.dist < 2)) { //North
				TryAddNode((Dir.N, minNode.dir == Dir.N ? minNode.dist + 1 : 0, minNode.y - 1, minNode.x));
			}
			if (minNode.x + 1 < width && minNode.dir != Dir.W && (minNode.dir != Dir.E || minNode.dist < 2)) { //East
				TryAddNode((Dir.E, minNode.dir == Dir.E ? minNode.dist + 1 : 0, minNode.y, minNode.x + 1));
			}
			if (minNode.y + 1 < height && minNode.dir != Dir.N && (minNode.dir != Dir.S || minNode.dist < 2)) { //South
				TryAddNode((Dir.S, minNode.dir == Dir.S ? minNode.dist + 1 : 0, minNode.y + 1, minNode.x));
			}
			if (minNode.x > 0 && minNode.dir != Dir.E && (minNode.dir != Dir.W || minNode.dist < 2)) { //West
				TryAddNode((Dir.W, minNode.dir == Dir.W ? minNode.dist + 1 : 0, minNode.y, minNode.x - 1));
			}
		}

		//Find the minimum distance to the end square in any possible state
		int minHeat = int.MaxValue;
		for (Dir dir = Dir.N; dir <= Dir.W; dir++) {
			for (int dist = 0; dist < 3; dist++) {
				minHeat = Math.Min(shortestDistanceMap[(int)dir][dist][height - 1][width - 1], minHeat);
			}
		}

		return minHeat;
	}

	public static int Solution2() {
		int[][] map = Array.ConvertAll(File.ReadAllLines("input17.txt"), line => Array.ConvertAll(line.ToCharArray(), c => c - '0'));
		int width = map[0].Length;
		int height = map.Length;

		//Initialize the map
		int[][][][] shortestDistanceMap = new int[4][][][]; //Direction, Distance, Y, X
		for (Dir dir = Dir.N; dir <= Dir.W; dir++) {
			shortestDistanceMap[(int)dir] = new int[7][][];
			for (int dist = 0; dist < 7; dist++) {
				shortestDistanceMap[(int)dir][dist] = new int[height][];
				for (int y = 0; y < height; y++) {
					shortestDistanceMap[(int)dir][dist][y] = new int[width];
					Array.Fill(shortestDistanceMap[(int)dir][dist][y], int.MaxValue);
				}
			}
		}
		for (int i = 4; i < 8; i++) {
			int horizontalSum = 0;
			int verticalSum = 0;
			for (int j = 1; j <= i; j++) {
				horizontalSum += map[0][j];
				verticalSum += map[j][0];
			}
			shortestDistanceMap[(int)Dir.E][i - 4][0][i] = horizontalSum;
			shortestDistanceMap[(int)Dir.S][i - 4][i][0] = verticalSum;
		}

		//Keep a list of visited and unvisited nodes
		HashSet<(Dir, int, int, int)> visited = [];
		HashSet<(Dir, int, int, int)> unvisited = [];
		for (int i = 4; i < 8; i++) {
			unvisited.Add((Dir.E, i - 4, 0, i));
			unvisited.Add((Dir.S, i - 4, i, 0));
		}

		//Find the minimum distance to every possible state
		while (unvisited.Count > 0) {
			//Get the unvisited node with the smallest distance to it
			(Dir dir, int dist, int y, int x) minNode = default;
			int minValue = int.MaxValue;
			foreach ((Dir dir, int dist, int y, int x) node in unvisited) {
				int value = shortestDistanceMap[(int)node.dir][node.dist][node.y][node.x];
				if (value < minValue) {
					minNode = node;
					minValue = value;
				}
			}
			visited.Add(minNode);
			unvisited.Remove(minNode);

			//Add all potential further directions
			void TryAddNode((Dir dir, int dist, int y, int x) node, int from, bool vertical) {
				if (!visited.Contains(node)) {
					unvisited.Add(node);
					int sum = 0;
					int to = vertical ? node.y : node.x;
					if (from > to) {
						(from, to) = (to, from);
					}
					if (vertical) {
						for (int i = from; i <= to; i++) {
							sum += map[i][node.x];
						}
					} else {
						for (int i = from; i <= to; i++) {
							sum += map[node.y][i];
						}
					}
					shortestDistanceMap[(int)node.dir][node.dist][node.y][node.x] = Math.Min(shortestDistanceMap[(int)node.dir][node.dist][node.y][node.x], minValue + sum);
				}
			}
			for (int extraDist = 4; extraDist < 8; extraDist++) {
				if (minNode.y >= extraDist && minNode.dir != Dir.S && (minNode.dir != Dir.N || minNode.dist + extraDist < 7)) { //North
					TryAddNode((Dir.N, minNode.dir == Dir.N ? minNode.dist + extraDist : extraDist - 4, minNode.y - extraDist, minNode.x), minNode.y - 1, true);
				}
				if (minNode.x + extraDist < width && minNode.dir != Dir.W && (minNode.dir != Dir.E || minNode.dist + extraDist < 7)) { //East
					TryAddNode((Dir.E, minNode.dir == Dir.E ? minNode.dist + extraDist : extraDist - 4, minNode.y, minNode.x + extraDist), minNode.x + 1, false);
				}
				if (minNode.y + extraDist < height && minNode.dir != Dir.N && (minNode.dir != Dir.S || minNode.dist + extraDist < 7)) { //South
					TryAddNode((Dir.S, minNode.dir == Dir.S ? minNode.dist + extraDist : extraDist - 4, minNode.y + extraDist, minNode.x), minNode.y + 1, true);
				}
				if (minNode.x >= extraDist && minNode.dir != Dir.E && (minNode.dir != Dir.W || minNode.dist + extraDist < 7)) { //West
					TryAddNode((Dir.W, minNode.dir == Dir.W ? minNode.dist + extraDist : extraDist - 4, minNode.y, minNode.x - extraDist), minNode.x - 1, false);
				}
			}
		}

		//Find the minimum distance to the end square in any possible state
		int minHeat = int.MaxValue;
		for (Dir dir = Dir.N; dir <= Dir.W; dir++) {
			for (int dist = 0; dist < 7; dist++) {
				minHeat = Math.Min(shortestDistanceMap[(int)dir][dist][height - 1][width - 1], minHeat);
			}
		}

		return minHeat;
	}

	enum Dir { N, E, S, W }
}
