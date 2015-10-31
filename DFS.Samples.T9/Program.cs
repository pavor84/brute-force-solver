using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace DFS.Samples.T9
{
    class T9Key {
        public char Digit {
            get;
            private set;
        }
        public char[] Letters {
            get;
            private set;
        }

        public T9Key(char digit, params char[] letters) {
            Digit = digit;
            Letters = letters;
        }
    }

    class T9Keyboard {
        private Dictionary<char, T9Key> keyMapping = new Dictionary<char, T9Key>();

        public T9Keyboard(IEnumerable<T9Key> keys) {
            foreach (var key in keys) {
                keyMapping.Add(key.Digit, key);
            }
        }

        public T9Key GetKey(char digit) {
            T9Key result;
            if (!keyMapping.TryGetValue(digit, out result))
                return null;
            return result;
        }

        public List<T9Key> Translate(string digits) {
            var result = new List<T9Key>();
            for (int i = 0; i < digits.Length; i++) {
                var key = GetKey(digits[i]);
                if (key != null)
                    result.Add(key);
            }
            return result;
        }
    }

    class T9Matcher {
        private string[] words;

        public T9Matcher(string dictionaryFilename) {
            words = File.ReadAllLines(dictionaryFilename);
            Array.Sort(words);
        }

        /*
        private List<string> FindWordsWithPrefix(string prefix) {
            var result = new List<string>();
            for (int i = 0; i < words.Length; i++) {
                if (words[i].StartsWith(prefix)) {
                    result = new List<string>();
                    result.Add(words[i]);
                }
            }
            return result;
        }
        */

        private List<string> FindWordsWithPrefix(string prefix) {
            List<string> result = null; // use lazy alloc
            int index = Array.BinarySearch(words, prefix);
            if (index > 0) {
                // perfect match
                result = new List<string>();
                result.Add(words[index++]);
            } else {
                // close match
                index = ~index;
            }
            for (int i = index; i < words.Length; i++) {
                if (words[i].StartsWith(prefix)) {
                    if (result == null)
                        result = new List<string>();
                    result.Add(words[i]);
                } else {
                    break;
                }
            }
            return result;
        }

        class T9Letter {
            public T9Key Key {
                get;
                set;
            }
            public int LetterIndex {
                get;
                set;
            }
            public char Letter {
                get {
                    return Key.Letters[LetterIndex];
                }
            }
        }

        /*
        public IEnumerable<string> GetWords(List<T9Key> pressed) {
            var result = new List<string>();
            var dfs = new GeneralDFS<T9Letter>();
            dfs.Init(this, (context, branch) => {
                if (branch.Count >= pressed.Count)
                    return null;
                var key = pressed[branch.Count];
                return key.Letters.Select((l, index) => new T9Letter() { Key = key, LetterIndex = index });
            }, (context, branch) => {
                var word = branch.Aggregate("", (w, v) => w + v.Letter);
                var matches = FindWordsWithPrefix(word);
                if (matches != null) {
                    foreach (var match in matches) {
                        result.Add(match);
                    }
                }
            });
            dfs.Run();
            return result;
        }
        */

        public IEnumerable<string> GetWords(List<T9Key> pressed) {
            var dfs = new EnumerableDFS<T9Letter>();
            dfs.Init(this, (context, branch, index) => {
                if (branch.Count >= pressed.Count)
                    return null;
                if (index >= pressed[branch.Count].Letters.Length)
                    return null;
                return new T9Letter() {
                    Key = pressed[branch.Count],
                    LetterIndex = index
                };
            });
            foreach (List<T9Letter> result in dfs) {
                var word = result.Aggregate("", (w, v) => w + v.Letter);
                var matches = FindWordsWithPrefix(word);
                if (matches != null) {
                    foreach (var match in matches) {
                        yield return match;
                    }
                }
            }
        }

        public Task StartGettingWords(List<T9Key> pressed, Action<string> processWord, CancellationToken ct) {
            var dfs = new IterativeDFS<T9Letter>();
            dfs.Init(this, (context, branch, index) => {
                if (branch.Count >= pressed.Count)
                    return null;
                if (index >= pressed[branch.Count].Letters.Length)
                    return null;
                return new T9Letter() {
                    Key = pressed[branch.Count],
                    LetterIndex = index
                };
            }, (context, branch) => {
                var word = branch.Aggregate("", (w, v) => w + v.Letter);
                var matches = FindWordsWithPrefix(word);
                if (matches != null) {
                    foreach (var match in matches) {
                        processWord(match);
                    }
                }
            });

            var task = Task.Factory.StartNew(() => {
                ct.ThrowIfCancellationRequested();

                while (dfs.Next()) {
                    if (ct.IsCancellationRequested) {
                        ct.ThrowIfCancellationRequested();
                    }
                }
            });
            return task;
        }
    }

    class MainClass {
        static T9Keyboard Keyboard = new T9Keyboard(new T9Key[] {
            new T9Key('2', 'a', 'b', 'c'),
            new T9Key('3', 'd', 'e', 'f'),
            new T9Key('4', 'g', 'h', 'i'),
            new T9Key('5', 'j', 'k', 'l'),
            new T9Key('6', 'm', 'n', 'o'),
            new T9Key('7', 'p', 'q', 'r', 's'),
            new T9Key('8', 't', 'u', 'v'),
            new T9Key('9', 'w', 'x', 'y', 'z'),
        });

        const string DictionaryFileName = "US.dic";
        static T9Matcher Matcher = new T9Matcher (DictionaryFileName);

        static void PrintMatches(string digits) {
            var keys = Keyboard.Translate(digits);
//            foreach (var match in Matcher.GetWords(keys)) {
//                Console.WriteLine(match);
//            }
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;
            var task = Matcher.StartGettingWords(keys, (w) => Console.WriteLine(w), ct);
            //tokenSource.Cancel();
            try {
                task.Wait();
            } catch (AggregateException ex) {
                Console.WriteLine("Task cancelled");
            } finally {
                tokenSource.Dispose();
            }

        }

        public static void Main (string[] args) {
            PrintMatches ("773242"); // SPECIA...
        }
    }
}
