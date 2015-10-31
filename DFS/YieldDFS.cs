using System;
using System.Collections.Generic;

namespace DFS
{
    /// <summary>
    /// Implementation of iterative Depth-first search using C# yield operator.
    /// </summary>
    public class YieldDFS<T> where T : class {
        private object context;
        private GetChildrenDelegate<T> getChildren;
        private int maxDepth = -1;
        private List<T> branch = new List<T>();

        /// <summary>
        /// Init iterative depth-first search.
        /// </summary>
        /// <param name="context">Custom data to be passed to delegates.</param>
        /// <param name="getChildren">Delegate to obtain children for current branch.</param>
        /// <param name="maxDepth">Depth limit (negative is treated as no limit).</param>
        public void Init(object context, GetChildrenDelegate<T> getChildren, int maxDepth = -1) {
            if (getChildren == null)
                throw new ArgumentNullException("getChildren");

            this.context = context;
            this.getChildren = getChildren;
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
        /// Get iterable collection of depth-first search results.
        /// </summary>
        public IEnumerable<List<T>> Get() {
            foreach (var result in GetImpl()) {
                yield return result;
                branch.Clear();
            }
        }

        private IEnumerable<List<T>> GetImpl() {
            if (maxDepth > 0 && branch.Count >= maxDepth) {
                yield return branch;
                yield break;
            }

            var connected = getChildren(context, branch);
            int numChilds = 0;
            if (connected != null) {
                foreach (var node in connected) {
                    numChilds++;
                    branch.Add(node);
                    foreach (var result in GetImpl()) {
                        yield return branch;
                        // after the result is returned, current branch is reset and
                        // all recursive calls are executed again in the stack order
                        branch.Add(node);
                    }
                    // current branch will contain our node as the last one
                    branch.RemoveAt(branch.Count - 1);
                }
            }

            if (numChilds == 0 && branch.Count > 0) {
                yield return branch;
            }
        }
    }
}

