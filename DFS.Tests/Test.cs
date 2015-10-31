using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DFS.Tests
{
	[TestFixture ()]
	public class Test {
		//      A
		//    / | \
		//   B  C D
		//  / \    \
		// E  F    G
		//          \
		//          H
		static Node Root = new Node("A") { Children = new List<Node>() {
				new Node("B") { Children = new List<Node>() { new Node("E"), new Node("F") } },
				new Node("C"),
				new Node("D") { Children = new List<Node>() { new Node("G") { Children = new List<Node>() { new Node("H") } } } }
			}
		};

		static List<string> Paths = new List<string> {
			"ABE", "ABF", "AC", "ADGH"
		};

		static List<string> Paths2 = new List<string> {
			"AB", "AC", "AD"
		};

		[Test ()]
		public void TestGeneralDFS () {
			var result = new List<string>();
			var dfs = new GeneralDFS<Node>();
			dfs.Init(null, Node.GetChildren, (context, branch) => result.Add(Node.BranchToString(branch)));
			dfs.Run();
			Assert.AreEqual(0, result.Count);

			dfs.Init(Root, Node.GetChildren, (context, branch) => result.Add(Node.BranchToString(branch)));
			dfs.Run();
			Assert.AreEqual(Paths.Count, result.Count);
			for (int i = 0; i < result.Count; i++) {
				Assert.AreEqual(Paths[i], result[i]);
			}
		}

		[Test ()]
		public void TestGeneralDFSDepth () {
			var result = new List<string>();
			var dfs = new GeneralDFS<Node>();
			dfs.Init(null, Node.GetChildren, (context, branch) => result.Add(Node.BranchToString(branch)), 2);
			dfs.Run();
			Assert.AreEqual(0, result.Count);

			dfs.Init(Root, Node.GetChildren, (context, branch) => result.Add(Node.BranchToString(branch)), 2);
			dfs.Run();
			Assert.AreEqual(Paths2.Count, result.Count);
			for (int i = 0; i < result.Count; i++) {
				Assert.AreEqual(Paths2[i], result[i]);
			}
		}

		[Test ()]
		public void TestIterativeDFS () {
			var result = new List<string>();
			var dfs = new IterativeDFS<Node>();
			dfs.Init(null, Node.GetChild, (context, branch) => result.Add(Node.BranchToString(branch)));
			while (dfs.Next());
			Assert.AreEqual(0, result.Count);

			dfs.Init(Root, Node.GetChild, (context, branch) => result.Add(Node.BranchToString(branch)));
			while (dfs.Next());
			Assert.AreEqual(Paths.Count, result.Count);
			for (int i = 0; i < result.Count; i++) {
				Assert.AreEqual(Paths[i], result[i]);
			}
		}

		[Test ()]
		public void TestEnumerableDFS () {
			var dfs = new EnumerableDFS<Node>();
			dfs.Init(null, Node.GetChild);
			Assert.AreEqual(0, dfs.Count());

			dfs.Init(Root, Node.GetChild);
			Assert.AreEqual(Paths.Count, dfs.Count());

			var result = dfs.Select(branch => Node.BranchToString(branch)).ToList();
			for (int i = 0; i < result.Count; i++) {
				Assert.AreEqual(Paths[i], result[i]);
			}
		}

		[Test ()]
		public void TestYieldDFS () {
			var dfs = new YieldDFS<Node>();
			dfs.Init(null, Node.GetChildren);
			Assert.AreEqual(0, dfs.Get().Count());

			dfs.Init(Root, Node.GetChildren);
			Assert.AreEqual(Paths.Count, dfs.Get().Count());

			var result = dfs.Get().Select(branch => Node.BranchToString (branch)).ToList();
			for (int i = 0; i < result.Count; i++) {
				Assert.AreEqual(Paths[i], result[i]);
			}
		}
	}
}

