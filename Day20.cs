using System;
using System.Collections.Generic;
using System.IO;

static partial class Day20 {
	public static int Solution1() {
		Broadcaster broadcaster = new();
		InitializeModules(broadcaster);

		//Run all the pulses
		int lowCount = 0;
		int highCount = 0;
		Queue<(Module from, Module to, bool pulse)> queue = [];
		for (int i = 0; i < 1000; i++) {
			queue.Enqueue((null!, broadcaster, false));
			while (queue.Count > 0) {
				(Module from, Module to, bool pulse) = queue.Dequeue();
				if (pulse) {
					highCount++;
				} else {
					lowCount++;
				}
				bool? returnPulse = to.ReceivePulse(from, pulse);
				if (returnPulse is null) {
					continue;
				}
				foreach (Module destination in to.destinations) {
					queue.Enqueue((to, destination, returnPulse.Value));
				}
			}
		}

		return lowCount * highCount;
	}

	public static long Solution2() {
		Broadcaster broadcaster = new();
		Dictionary<string, Module> moduleNameMap = InitializeModules(broadcaster);

		//Run the pulses until a low pulse is sent to rx
		//Note, this puzzle is entirely dependent on the shape of the input.
		//I solved it part by hand, plugging the graph into GraphViz and intuiting the working mechanism.
		//The graph divides into 4 independent sections, with the broadcaster and the output connecting them all.
		//Essentially, each of the 4 sections is a clock which sends a high pulse to the output conjunction every N cycles, but a low pulse on all other cycles.
		//The answer thus is the cycle on which all four clocks send a high pulse, which is the least common multiple of their cycle length, which, because the cycle lengths are all primes, is their product.
		Queue<(Module from, Module to, bool pulse)> queue = [];
		int buttonPresses = 0;
		Dictionary<Module, int> cycleLengths = new() { { moduleNameMap["bc"], 0 }, { moduleNameMap["gj"], 0 }, { moduleNameMap["qq"], 0 }, { moduleNameMap["bx"], 0 } };
		while (true) {
			queue.Enqueue((null!, broadcaster, false));
			buttonPresses++;
			while (queue.Count > 0) {
				(Module from, Module to, bool pulse) = queue.Dequeue();
				bool? returnPulse = to.ReceivePulse(from, pulse);
				if (returnPulse is null) {
					continue;
				}
				foreach (Module destination in to.destinations) {
					queue.Enqueue((to, destination, returnPulse.Value));
				}

				if (cycleLengths.TryGetValue(to, out int cycleLength) && cycleLength == 0) {
					bool allHigh = true;
					foreach (bool value in ((Conjunction)to).inputs.Values) {
						if (!value) {
							allHigh = false;
							break;
						}
					}
					if (allHigh) {
						cycleLengths[to] = buttonPresses;
					}
				}
			}

			bool complete = true;
			foreach (int value in cycleLengths.Values) {
				if (value == 0) {
					complete = false;
					break;
				}
			}
			if (complete) {
				break;
			}
		}

		long product = 1;
		foreach (int value in cycleLengths.Values) {
			product *= value;
		}
		return product;
	}

	static Dictionary<string, Module> InitializeModules(Broadcaster broadcaster) {
		string[][] lines = Array.ConvertAll(File.ReadAllLines("input20.txt"), line => line.Split(" -> "));
		Dictionary<string, Module> moduleNameMap = [];
		NullModule nullModule = new();
		foreach (string[] line in lines) {
			string name = line[0];
			if (name[0] == '%') {
				moduleNameMap[name[1..]] = new FlipFlop();
			} else if (name[0] == '&') {
				moduleNameMap[name[1..]] = new Conjunction();
			}
		}
		foreach (string[] line in lines) {
			string name = line[0][1..];
			Module module = name == "roadcaster" ? broadcaster : moduleNameMap[name];
			foreach (string destination in line[1].Split(", ")) {
				if (!moduleNameMap.TryGetValue(destination, out Module? destModule)) {
					destModule = nullModule;
				}
				module.destinations.Add(destModule);
				if (destModule is Conjunction destConjunction) {
					destConjunction.inputs[module] = false;
				}
			}
		}
		return moduleNameMap;
	}

	abstract class Module {
		public List<Module> destinations = [];

		public abstract bool? ReceivePulse(Module from, bool pulse);
	}

	class NullModule : Module {
		public override bool? ReceivePulse(Module from, bool pulse) => null;
	}

	class Broadcaster : Module {
		public override bool? ReceivePulse(Module from, bool pulse) => pulse;
	}

	class FlipFlop : Module {
		public bool state = false;

		public override bool? ReceivePulse(Module from, bool pulse) => pulse ? null : (state = !state);
	}

	class Conjunction : Module {
		public Dictionary<Module, bool> inputs = [];

		public override bool? ReceivePulse(Module from, bool pulse) {
			inputs[from] = pulse;
			bool hasLow = false;
			foreach (bool value in inputs.Values) {
				if (!value) {
					hasLow = true;
					break;
				}
			}
			return hasLow;
		}
	}
}
