using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DFS.Benchmark
{
	class MainClass {
		static Node Root = new Node ("0");


		public static void Main(string[] args) {
			CreateBalancedTree(Root, 3, 10);

			// warming up
			for (int i = 0; i < 10; i++) {
				TestGeneralDFS ();
				TestIterativeDFS ();
				TestEnumerableDFS ();
				TestYieldDFS ();
			}

			MeasureTime ("GeneralDFS", () => {
				TestGeneralDFS ();
			});

			MeasureTime ("IterativeDFS", () => {
				TestIterativeDFS ();
			});

			MeasureTime ("EnumerableDFS", () => {
				TestEnumerableDFS ();
			});

			MeasureTime ("YieldDFS", () => {
				TestYieldDFS ();
			});
		}

		static long MeasureTime(string label, Action action) {
			var sw = new Stopwatch();
			sw.Start();
			action();
			sw.Stop();

			long time = sw.ElapsedMilliseconds;
			Console.WriteLine(label + " took " + time + " ms");
			return time;
		}

		static void CreateBalancedTree(Node root, int children, int depth) {
			if (depth <= 1)
				return;
			root.Children = new List<Node>();
			for (int i = 0; i < children; i++) {
				var child = new Node(root.Name + (i + 1).ToString());
				root.Children.Add(child);
				CreateBalancedTree(child, children, depth - 1);
			}
		}

		static IEnumerable TestGeneralDFS () {
			var result = new List<string>();
			var dfs = new GeneralDFS<Node>();
			dfs.Init(Root, Node.GetChildren, (context, branch) => result.Add(Node.BranchToString(branch)));
			dfs.Run();
			return result;
		}

		static IEnumerable TestIterativeDFS () {
			var result = new List<string>();
			var dfs = new IterativeDFS<Node>();
			dfs.Init(Root, Node.GetChild, (context, branch) => result.Add(Node.BranchToString(branch)));
			while (dfs.Next());
			return result;
		}

		static IEnumerable TestEnumerableDFS () {
			var dfs = new EnumerableDFS<Node>();
			dfs.Init(Root, Node.GetChild);
			var result = dfs.Select(branch => Node.BranchToString(branch)).ToList();
			return result;
		}

		static IEnumerable TestYieldDFS () {
			var dfs = new YieldDFS<Node>();
			dfs.Init(Root, Node.GetChildren);
			var result = dfs.Get().Select(branch => Node.BranchToString(branch)).ToList();
			return result;
		}
	}
}
