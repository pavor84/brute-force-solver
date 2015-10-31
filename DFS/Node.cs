using System;
using System.Collections.Generic;
using System.Linq;

namespace DFS
{
    /// <summary>
    /// Sample tree node.
    /// </summary>
    public class Node {
        /// <summary>
        /// Initializes a new instance of the <see cref="DFS.Node"/> class.
        /// </summary>
        /// <param name="name">Name of a tree node (could be empty).</param>
        public Node(string name) {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>Name of a tree node.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the children of a tree node.
        /// </summary>
        /// <value>List of children or null if no children exist.</value>
        public List<Node> Children { get; set; }

        /// <summary>
        /// Delegate that can be used to obtain children for current branch.
        /// </summary>
        /// <returns>Collection of child nodes</returns>
        /// <param name="context">Custom data passed from top-level caller, should be the root of the tree.</param>
        /// <param name="branch">Current branch (i.e. sequence of tree nodes starting from root).</param>
        public static IEnumerable<Node> GetChildren(object context, List<Node> branch) {
            if (context == null)
                return null;
            if (branch.Count == 0)
                return new List<Node> { (Node)context };
            return branch.Last().Children;
        }

        /// <summary>
        /// Delegate that can be used to obtain a child with given index for current branch.
        /// </summary>
        /// <returns>A reference to a child, or null if no child with provided index exists</returns>
        /// <param name="context">Custom data passed from top-level caller, should be the root of the tree.</param>
        /// <param name="branch">Current branch (i.e. sequence of tree nodes starting from root).</param>
        /// <param name="index">Zero-based child index.</param>
        public static Node GetChild(object context, List<Node> branch, int index) {
            if (context == null)
                return null;
            if (branch.Count == 0)
                return index == 0 ? (Node)context : null;
            var children = branch.Last().Children;
            if (children != null && index < children.Count)
                return children[index];
            return null;
        }

        /// <summary>
        /// Concatenate node names into a single string.
        /// </summary>
        /// <returns>Concatenated names of all nodes in branch.</returns>
        /// <param name="branch">Tree branch.</param>
        public static string BranchToString(List<Node> branch) {
            return branch.Aggregate("", (s, n) => s + n.Name);
        }
    }
}