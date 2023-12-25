using System;
using System.IO;

static partial class Day22 {
	public static int Solution1() {
		Initialize(out bool[][][] tower, out Brick[] bricks, out _, out _, out _);

		//Try removing each brick, and see if that would cause any other brick to fall
		int count = 0;
		foreach (Brick brick in bricks) {
			brick.Set(tower, false);
			count += 1 - Settle(tower, bricks, true);
			brick.Set(tower, true);
		}
		return count;
	}

	public static int Solution2() {
		Initialize(out _, out Brick[] bricks, out int maxX, out int maxY, out int maxZ);

		//Recalculate the height of the tower, for optimization
		foreach (Brick brick in bricks) {
			maxZ = Math.Max(maxZ, brick.toZ);
		}

		//Clone all the bricks and the entire tower, then see how many bricks would fall by removing each brick
		int count = 0;
		for (int i = 0; i < bricks.Length; i++) {
			Brick[] clonedBricks = new Brick[bricks.Length - 1];
			for (int from = 0, to = 0; from < bricks.Length; from++, to++) {
				if (from == i) {
					to--;
					continue;
				}
				Brick brick = bricks[from];
				clonedBricks[to] = new Brick(brick.fromX, brick.fromY, brick.fromZ, brick.toX, brick.toY, brick.toZ);
			}
			bool[][][] clonedTower = CreateTower(maxX, maxY, maxZ, clonedBricks);
			count += Settle(clonedTower, clonedBricks);
		}
		return count;
	}

	class Brick(int fromX, int fromY, int fromZ, int toX, int toY, int toZ) {
		public readonly int fromX = fromX, fromY = fromY, toX = toX, toY = toY;
		public int fromZ = fromZ, toZ = toZ;
		public int Height => toZ - fromZ;

		public void Set(bool[][][] tower, bool value) {
			for (int x = fromX; x <= toX; x++) {
				for (int y = fromY; y <= toY; y++) {
					for (int z = fromZ; z <= toZ; z++) {
						tower[x][y][z] = value;
					}
				}
			}
		}
	}

	static void Initialize(out bool[][][] tower, out Brick[] bricks, out int maxX, out int maxY, out int maxZ) {
		//Read in the bricks
		bricks = Array.ConvertAll(File.ReadAllLines("input22.txt"), line => {
			int[] coords = Array.ConvertAll(line.Split(',', '~'), int.Parse);
			return new Brick(coords[0], coords[1], coords[2] - 1, coords[3], coords[4], coords[5] - 1); //Shift Z down by 1 so the ground brick would be at 0
		});

		//Initialize the tower
		maxX = 0;
		maxY = 0;
		maxZ = 0;
		foreach (Brick brick in bricks) {
			maxX = Math.Max(maxX, brick.toX);
			maxY = Math.Max(maxY, brick.toY);
			maxZ = Math.Max(maxZ, brick.toZ);
		}
		tower = CreateTower(maxX, maxY, maxZ, bricks);

		//Sort and settle the bricks down
		Array.Sort(bricks, (a, b) => a.fromZ - b.fromZ);
		Settle(tower, bricks);
	}

	static int Settle(bool[][][] tower, Brick[] bricks, bool simulateOnly = false) {
		int bricksSettled = 0;
		foreach (Brick brick in bricks) {
			//Find the lower position it can fall to
			int highestZ = 0;
			for (int x = brick.fromX; x <= brick.toX; x++) {
				for (int y = brick.fromY; y <= brick.toY; y++) {
					for (int z = brick.fromZ - 1; z >= highestZ; z--) {
						if (tower[x][y][z]) {
							highestZ = Math.Max(highestZ, z + 1);
							break;
						}
					}
				}
			}

			//Reset the brick's position
			if (highestZ != brick.fromZ) {
				if (simulateOnly) {
					return 1;
				}
				brick.Set(tower, false);
				int height = brick.Height;
				brick.fromZ = highestZ;
				brick.toZ = highestZ + height;
				brick.Set(tower, true);
				bricksSettled++;
			}
		}
		return bricksSettled;
	}

	static bool[][][] CreateTower(int maxX, int maxY, int maxZ, Brick[] bricks) {
		bool[][][] tower = new bool[maxX + 1][][];
		for (int x = 0; x <= maxX; x++) {
			tower[x] = new bool[maxY + 1][];
			for (int y = 0; y <= maxY; y++) {
				tower[x][y] = new bool[maxZ + 1];
			}
		}
		foreach (Brick brick in bricks) {
			brick.Set(tower, true);
		}
		return tower;
	}
}
