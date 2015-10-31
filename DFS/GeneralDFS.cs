using System;
using System.Collections.Generic;

namespace DFS
{
    /// <summary>
    /// Delegate that can be used to obtain children for current branch.
    /// </summary>
    /// <returns>Collection of child nodes</returns>
    /// <param name="context">Custom data passed from top-level caller.</param>
    /// <param name="branch">Current branch (i.e. sequence of tree nodes starting from root).</param>
    public delegate IEnumerable<T> GetChildrenDelegate<T>(object context, List<T> branch);

    /// <summary>
    /// Process branch found with depth-first search (goes from root to the leaf).
    /// </summary>
    /// <param name="context">Custom data passed from top-level caller.</param>
    /// <param name="branch">Current branch (i.e. sequence of tree nodes starting from root).</param>
    public delegate void ProcessResultDelegate<T>(object context, List<T> branch);

    /// <summary>
    /// General implementation of Depth-first search.
    /// </summary>
    public class GeneralDFS<T> {
        private object context;
        private GetChildrenDelegate<T> getChildren;
        private ProcessResultDelegate<T> processResult;
        private int maxDepth = -1;
        private List<T> branch = new List<T>();

        /// <summary>
        /// Init depth-first search.
        /// </summary>
        /// <param name="context">Custom data to be passed to delegates.</param>
        /// <param name="getChildren">Delegate to obtain children for current branch.</param>
        /// <param name="processResult">Delegate to process result.</param>
        /// <param name="maxDepth">Depth limit (negative is treated as no limit).</param>
        public void Init(object context, GetChildrenDelegate<T> getChildren, ProcessResultDelegate<T> processResult, int maxDepth = -1) {
            if (getChildren == null)
                throw new ArgumentNullException("getChildren");
            if (processResult == null)
                throw new ArgumentNullException("processResult");

            this.context = context;
            this.getChildren = getChildren;
            this.processResult = processResult;
            this.maxDepth = maxDepth;
            branch = new List<T>();
        }

        /// <summary>
        /// Reset depth-first search to initial state.
        /// </summary>
        public void Reset() {
            branch.Clear();
        }

        /// <summary>
        /// Run depth-first search (delegates passed to <see cref="Init"/> will be called during execution of this method).
        /// </summary>
        public void Run() {
            if (maxDepth > 0 && branch.Count >= maxDepth) {
                processResult(context, branch);
                return;
            }

            var connected = getChildren(context, branch);
            int numChilds = 0;
            if (connected != null) {
                foreach (var node in connected) {
                    numChilds++;
                    branch.Add(node);
                    Run();
                    // current branch will contain our node as the last one
                    branch.RemoveAt(branch.Count - 1);
                }
            }

            if (numChilds == 0 && branch.Count > 0) {
                processResult(context, branch);
            }
        }
    }
}

