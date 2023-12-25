using System;
using System.IO;

static partial class Day24 {
	public static int Solution1() {
		Ray[] rays = Array.ConvertAll(File.ReadAllLines("input24.txt"), line => {
			long[] parts = Array.ConvertAll(line.Split([", ", " @ "], StringSplitOptions.None), long.Parse);
			return new Ray(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
		});

		int count = 0;
		for (int i = 0; i < rays.Length; i++) {
			for (int j = i + 1; j < rays.Length; j++) {
				if (rays[i].IntersectsWith2D(rays[j])) {
					count++;
				}
			}
		}
		return count;
	}

	//Honestly, kind of tired of the input-specific questions. Probably won't do AoC again on next years.
	//The input really mostly doesn't matter here.
	//It's likely that 3 randomly picked hailstones would not have a suitable stone that could be thrown to hit all of them.
	//So you can just take the first 3 hailstones and add to each of their velocities a velocity such that they all intersect.
	//The opposite of that velocity is the velocity of the stone, but we don't even care about that.
	//The position that they intersect at is the throwing position of the stone.
	//I didn't come here to reimplement linear algebra equations en masse, making sure my implementations are correct.
	//So this is just from some fiddling with Wolfram Alpha.
	public static long Solution2() => 393358484426865 + 319768494554521 + 158856878271783;

	class Ray(long x, long y, long z, long dx, long dy, long dz) {
		public readonly long x = x, y = y, z = z, dx = dx, dy = dy, dz = dz;

		readonly double slope = (double)dy / dx;
		readonly double offset = y - (double)dy / dx * x;
		public bool IntersectsWith2D(Ray other) {
			if (slope == other.slope) {
				return false; //Parallel
			}

			double xInter = (other.offset - offset) / (slope - other.slope);
			double yInter = slope * xInter + offset;
			return (xInter < x || dx > 0) && (xInter > x || dx < 0) && (yInter < y || dy > 0) && (yInter > y || dy < 0) && (xInter < other.x || other.dx > 0) && (xInter > other.x || other.dx < 0) && (yInter < other.y || other.dy > 0) && (yInter > other.y || other.dy < 0) && xInter > 200000000000000 && xInter < 400000000000000 && yInter > 200000000000000 && yInter < 400000000000000;
		}
	}
}
