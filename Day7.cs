using System;
using System.Collections.Generic;
using System.IO;

static class Day7 {
	enum Type { HighCard, Pair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind };

	static int Solution(Dictionary<char, int> cardMap, Action<List<int>, int[]> Count) {
		List<int> cardCounts = new(5);
		(int[] hand, int bid, Type type)[] hands = Array.ConvertAll(File.ReadAllLines("input7.txt"), line => {
			int[] hand = Array.ConvertAll(line[..5].ToCharArray(), c => cardMap[c]);
			cardCounts.Clear();
			Count(cardCounts, hand);

			return (hand, bid: int.Parse(line[6..]), type: cardCounts[0] == 5 ? Type.FiveOfAKind :
				cardCounts[0] == 4 ? Type.FourOfAKind :
				cardCounts[0] == 3 ? cardCounts[1] == 2 ? Type.FullHouse : Type.ThreeOfAKind :
				cardCounts[0] == 2 ? cardCounts[1] == 2 ? Type.TwoPair : Type.Pair :
				Type.HighCard);
		});

		Array.Sort(hands, (a, b) => {
			if (a.type != b.type) {
				return b.type - a.type;
			} else {
				for (int i = 0; i < 5; i++) {
					if (a.hand[i] != b.hand[i]) {
						return b.hand[i] - a.hand[i];
					}
				}
				return 0;
			}
		});

		int sum = 0;
		for (int i = 0; i < hands.Length; i++) {
			sum += (hands.Length - i) * hands[i].bid;
		}
		return sum;
	}

	public static int Solution1() => Solution(new Dictionary<char, int>() { { '2', 0 }, { '3', 1 }, { '4', 2 }, { '5', 3 }, { '6', 4 }, { '7', 5 }, { '8', 6 }, { '9', 7 }, { 'T', 8 }, { 'J', 9 }, { 'Q', 10 }, { 'K', 11 }, { 'A', 12 } }, (cardCounts, hand) => {
		for (int i = 0; i < 5; i++) {
			int card = hand[i];
			int count = 1;
			for (int j = 0; j < i; j++) { //Check if we've processed this card already
				int otherCard = hand[j];
				if (card == otherCard) {
					goto End;
				}
			}
			for (int j = i + 1; j < 5; j++) {
				int otherCard = hand[j];
				if (card == otherCard) {
					count++;
				}
			}
			cardCounts.Add(count);
		End:
			continue;
		}
		cardCounts.Sort((a, b) => b - a); //Descending order
	});

	public static int Solution2() => Solution(new() { { 'J', 0 }, { '2', 1 }, { '3', 2 }, { '4', 3 }, { '5', 4 }, { '6', 5 }, { '7', 6 }, { '8', 7 }, { '9', 8 }, { 'T', 9 }, { 'Q', 10 }, { 'K', 11 }, { 'A', 12 } }, (cardCounts, hand) => {
		int jokers = 0;
		foreach (int card in hand) {
			if (card == 0) {
				jokers++;
			}
		}
		for (int i = 0; i < 5; i++) {
			int card = hand[i];
			if (card == 0) {
				continue;
			}
			int count = 1;
			for (int j = 0; j < i; j++) { //Check if we've processed this card already
				int otherCard = hand[j];
				if (card == otherCard) {
					goto End;
				}
			}
			for (int j = i + 1; j < 5; j++) {
				int otherCard = hand[j];
				if (card == otherCard) {
					count++;
				}
			}
			cardCounts.Add(count);
		End:
			continue;
		}
		cardCounts.Sort((a, b) => b - a); //Descending order
		if (jokers == 5) {
			cardCounts.Add(5);
		} else {
			cardCounts[0] += jokers;
		}
	});
}
