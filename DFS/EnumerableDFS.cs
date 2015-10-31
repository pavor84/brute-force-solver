using System;
using System.Collections;
using System.Collections.Generic;

namespace DFS
{
    /// <summary>
    /// Implementation of iterative Depth-first search with IEnumerable interface.
    /// </summary>
    public class EnumerableDFS<T> : IEnumerable<List<T>> where T : class {
        private IterativeDFS<T> dfs = new IterativeDFS<T>();

        /// <summary>
        /// Init enumerable depth-first search.
        /// </summary>
        /// <param name="context">Custom data to be passed to delegates.</param>
        /// <param name="getChild">Delegate to obtain a child with given index for current branch.</param>
        /// <param name="maxDepth">Depth limit (negative is treated as no limit).</param>
        public void Init(object context, GetChildDelegate<T> getChild, int maxDepth = -1) {
            dfs.Init(context, getChild, (ctx, branch) => {}, maxDepth);
        }

        /// <summary>
        /// Reset depth-first search to initial state.
        /// </summary>
        public void Reset() {
            dfs.Reset();
        }

        class DFSEnumerator : IEnumerator<List<T>> {
            private IterativeDFS<T> dfs;
            public DFSEnumerator(IterativeDFS<T> dfs) {
                this.dfs = dfs;
                dfs.Reset();
            }

            #region IEnumerator implementation
            public bool MoveNext() {
                return dfs.Next();
            }
            public void Reset() {
                dfs.Reset();
            }
            object IEnumerator.Current {
                get {
                    return dfs.State;
                }
            }
            #endregion
            #region IDisposable implementation
            public void Dispose() {
            }
            #endregion
            #region IEnumerator implementation
            public List<T> Current {
                get {
                    return dfs.State;
                }
            }
            #endregion
        }

        #region IEnumerable implementation

        public IEnumerator<List<T>> GetEnumerator() {
            return new DFSEnumerator(dfs);
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator() {
            return new DFSEnumerator(dfs);
        }

        #endregion
    }
}

