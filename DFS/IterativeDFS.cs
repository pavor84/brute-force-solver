using System;
using System.Collections.Generic;

namespace DFS
{
    /// <summary>
    /// Delegate that can be used to obtain a child with given index for current branch.
    /// </summary>
    /// <returns>A reference to a child, or null if no child with provided index exists</returns>
    /// <param name="context">Custom data passed from top-level caller.</param>
    /// <param name="branch">Current branch (i.e. sequence of tree nodes starting from root).</param>
    /// <param name="index">Zero-based child index.</param>
    public delegate T GetChildDelegate<T>(object context, List<T> branch, int index) where T : class;

    /// <summary>
    /// Implementation of iterative Depth-first search that can be performed step by step.
    /// </summary>
    public class IterativeDFS<T> where T : class {
        private object context;
        private GetChildDelegate<T> getChild;
        private ProcessResultDelegate<T> processResult;
        private int maxDepth = -1;
        private List<T> branch = new List<T>();
        /// <summary>
        /// Gets the state of depth-first search.
        /// </summary>
        /// <value>Current branch (i.e. sequence of tree nodes starting from root).</value>
        public List<T> State {
            get { return branch; }
        }

        private List<int> indices = new List<int>();

        /// <summary>
        /// Init iterative depth-first search.
        /// </summary>
        /// <param name="context">Custom data to be passed to delegates.</param>
        /// <param name="getChild">Delegate to obtain a child with given index for current branch.</param>
        /// <param name="processResult">Delegate to process result.</param>
        /// <param name="maxDepth">Depth limit (negative is treated as no limit).</param>
        public void Init(object context, GetChildDelegate<T> getChild, ProcessResultDelegate<T> processResult, int maxDepth = -1) {
            if (getChild == null)
                throw new ArgumentNullException("getChild");
            if (processResult == null)
                throw new ArgumentNullException("processResult");

            this.context = context;
            this.getChild = getChild;
            this.processResult = processResult;
            this.maxDepth = maxDepth;
            branch = new List<T>();
            indices = new List<int>();
        }

        /// <summary>
        /// Reset depth-first search to initial state.
        /// </summary>
        public void Reset() {
            branch.Clear();
            indices.Clear();
        }

        /// <summary>
        /// Go to next iteration of depth-first search, to check current state use <see cref="State"/>.
        /// </summary>
        /// <returns><c>true</c> if there are more results, <c>false</c> otherwise</returns>
        public bool Next() {
            branch.Clear();
            return NextImpl();
        }

        private bool NextImpl() {
            if (maxDepth > 0 && branch.Count >= maxDepth) {
                processResult(context, branch);
                return true;
            }

            int i = branch.Count;

            while (indices.Count <= i) {
                indices.Add(0);
            }

            T child = getChild(context, branch, indices[i]);
            while (child != null) {
                branch.Add(child);
                if (NextImpl()) {
                    return true;
                } else {
                    // backtracking
                    indices[i]++;
                    if (indices.Count > i + 1) {
                        indices.RemoveRange (i + 1, indices.Count - (i + 1));
                    }
                    branch.RemoveAt(i);
                    child = getChild(context, branch, indices[i]);
                }
            }

            if (indices[i] == 0 && child == null && branch.Count > 0) {
                processResult(context, branch);
                indices[i]++;
                return true;
            }

            return false;
        }
    }
}

