using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClassLibrary31
{
    public class DirectoryTree
    {
        private readonly Dictionary<string, object> _nodes = new Dictionary<string, object>();

        public DirectoryTree()
        {
            ReadFile = fileInfo => File.ReadAllText(fileInfo.FullName);
        }

        public DirectoryTree(DirectoryTree parent)
        {
            ReadFile = parent.ReadFile;
        }

        public Func<FileInfo, object> ReadFile;

        public IEnumerable<KeyValuePair<string, object>> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        public object this[params string[] keys]
        {
            set
            {
                foreach (var key in keys)
                {
                    _nodes[key] = value;
                }
            }
        }

        public void Load(string path)
        {
            var directory = new DirectoryInfo(path);

            Read(this, directory);
        }

        public void Save(string path)
        {
            Write(path, this);
        }

        private void Write(string path, object value)
        {
            var tree = value as DirectoryTree;
            if (tree != null)
            {
                Directory.CreateDirectory(path);

                foreach (var node in tree.Nodes)
                {
                    Write(Path.Combine(path, node.Key), node.Value);
                }
            }
            else
            {
                File.WriteAllText(path, value?.ToString() ?? string.Empty);
            }
        }

        private static void Read(DirectoryTree parent, DirectoryInfo directoryInfo)
        {
            var directory = new DirectoryTree(parent);

            foreach (var subDirectoryInfo in directoryInfo.EnumerateDirectories())
            {
                Read(directory, subDirectoryInfo);
            }

            foreach (var file in directoryInfo.EnumerateFiles())
            {
                directory[file.Name] = directory.ReadFile(file);
            }

            parent[directoryInfo.Name] = directory;
        }

        // TODO: DiffResult
        public DirectoryTree Diff(DirectoryTree other)
        {
            var nodes1 = Flatten().OrderBy(n => n.Key).ToList();
            var nodes2 = other.Flatten().OrderBy(n => n.Key).ToList();

            nodes1.Zip(nodes2, (p1, p2) =>
            {
                var keysEqual = object.Equals(p1.Key, p2.Key);
                var valuesEqual = object.Equals(p1.Value, p2.Value);

                return keysEqual && valuesEqual;
            })
            .All(a => a);

            return null;
        }

        public IEnumerable<KeyValuePair<string, object>> Flatten()
        {
            var allNodes = new Dictionary<string, object>();
            var stack = new Stack<Tuple<string, DirectoryTree>>();
            stack.Push(Tuple.Create("", this));

            while (stack.Count > 0)
            {
                var top = stack.Pop();
                var basePath = top.Item1;
                var tree = top.Item2;

                foreach (var node in tree.Nodes)
                {
                    var subTree = node.Value as DirectoryTree;
                    if (subTree == null)
                    {
                        allNodes[node.Key] = node.Value;
                    }
                    else
                    {
                        var subPath = string.IsNullOrEmpty(basePath) ? node.Key : $"{basePath}/{node.Key}";
                        stack.Push(Tuple.Create(subPath, subTree));
                    }
                }
            }

            return allNodes;
        }
    }
}
