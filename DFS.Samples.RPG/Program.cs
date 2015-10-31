using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DFS.Samples.RPG
{
	class AttributeVariant {
		public int AttributeIndex { get; set; }
		public int ValueIndex { get; set; }

		public AttributeVariant() {
		}
		public AttributeVariant(int a, int v) {
			AttributeIndex = a;
			ValueIndex = v;
		}
	}

	class MainClass {
		const string DictionaryFileName = "US.dic";

		public static string[] LoadDictionary(string filename) {
			var list = File.ReadAllLines(filename);
			return list;
		}

		public static void Main(string[] args) {
			var wordDict = LoadDictionary(DictionaryFileName);

			var allowedWords = new SortedSet<string> ();
			foreach (var w in wordDict) {
				if (w.Length == MaxAttributes)
					allowedWords.Add(w);
			}

			int total = 0;
			var found = new SortedSet<string> ();

			var dfs = new GeneralDFS<AttributeVariant> ();
			dfs.Init(null, GetNextAttributeVariants, (context, result) => {
				total++;
				var word = "";
				var sb = new StringBuilder ();
				foreach (var node in result) {
					word += AttributeValues [node.AttributeIndex] [node.ValueIndex] [0];
					if (sb.Length > 0)
						sb.Append ("-");
					sb.Append (AttributeValues [node.AttributeIndex] [node.ValueIndex].Substring (0, 3));
				}

				word = word.ToLower ();
				if (allowedWords.Contains (word) && !found.Contains (word)) {
					found.Add (word);
					Console.WriteLine (word + " " + sb.ToString ());
				}
			}, MaxAttributes);
			dfs.Run();

			Console.WriteLine("Total: " + total);
			Console.WriteLine("Found: " + found.Count);
		}

		const int MaxAttributes = 7;

		static readonly string[][] AttributeValues = new string[][] {
			new string[] { "Strength", "Body", "Might", "Brawn" },
			new string[] { "Perception", "Alertness", "Awareness", "Cautiousness" },
			new string[] { "Constitution", "Stamina", "Endurance", "Vitality", "Health", "Defense", "Resistance", "Fortitude", "Resilience" },
			new string[] { "Charisma", "Presence", "Charm", "Social", "Niceness" },
			new string[] { "Intelligence", "Intellect", "Mind", "Knowledge" },
			new string[] { "Dexterity", "Agility", "Reflexes", "Quickness", "Speed" },
			//new string[] { "Defense", "Resistance", "Fortitude", "Resilience" },
			//new string[] { "Willpower", "Sanity", "Personality", "Ego" },
			new string[] { "Luck", "Fate", "Chance" }
		};

		public static IEnumerable<AttributeVariant> GetNextAttributeVariants(object context, List<AttributeVariant> attributes) {
			var variants = new List<AttributeVariant> ();
			for (int i = 0; i < AttributeValues.Length; i++) {
				if (attributes.FindIndex(a => a.AttributeIndex == i) != -1)
					continue;
				for (int j = 0; j < AttributeValues[i].Length; j++) {
					if (attributes.FindIndex(a => AttributeValues[a.AttributeIndex][a.ValueIndex][0] == AttributeValues[i][j][0]) != -1)
						continue;
					variants.Add(new AttributeVariant(i, j));
				}
			}
			return variants;
		}
	}
}
