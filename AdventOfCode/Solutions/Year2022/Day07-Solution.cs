using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2022;
[DayInfo(2022, 07, "No Space Left On Device")]
class Day07 : ASolution
{
    class Node {
        public Node(string name, Node? parent, bool isDirectory) {
            Parent = parent;
            Name = name;
            IsDirectory = isDirectory;
        }
        public Node? Parent { get; }
        public string Name { get; }
        public bool IsDirectory { get; } = false;
        public long? FileSize = null;
        Dictionary<string, Node> Children = new();
        public IEnumerable<Node> Contents => Children.Values;

        long? totalSize = null;
        public long GetTotalSize() {
            if (totalSize is null) {
                if (IsDirectory) {
                    totalSize = Children.Sum(n => n.Value.GetTotalSize());
                }
                else 
                    totalSize = FileSize!;
            }
            return totalSize.Value;
        }
        public Node? GetChild(string name) {
            return Children.GetValueOrDefault(name);
        }
        public Node MakeSubDir(string name) {
            if (!IsDirectory) throw new InvalidOperationException("not a dir");

            Node result;
            if( (result = GetChild(name)) is null ) {
                result = new Node(name, this, true);
                Children.Add(name, result);
            }
            return result;
        }
        public Node MakeFile(string name, long size) {
            if (!IsDirectory) throw new InvalidOperationException("not a dir");

            Node result;
            if ((result = GetChild(name)) is null) {
                result = new Node(name, this, false) {
                    FileSize = size
                };
                Children.Add(name, result);
            }
            return result;
        }
    }
    public Day07() : base(false) {

    }
    Node tree;
    protected override void ParseInput() {
        tree = new("/", null, true);
        Node currentDirectory = tree;
        bool collectingDir = false;
        foreach(string line in Input.SplitByNewline()) {
            var inv = line.Split(' ');
            switch(line[0]) {
                case '$':
                    collectingDir = false;
                    switch (inv[1]) {
                        case "ls":
                            collectingDir = true;
                            continue;
                        case "cd":
                            string subdir = inv[2];
                            if (subdir == "..") {
                                currentDirectory = currentDirectory.Parent;
                                break;
                            } else if( subdir == "/" ) {
                                currentDirectory = tree;
                                break;
                            }
                            // not going up
                            var subNode = currentDirectory.GetChild(subdir) ?? currentDirectory.MakeSubDir(subdir);
                            if (!subNode.IsDirectory) throw new InvalidOperationException("cannot cd into file");
                            currentDirectory = subNode;
                            break;
                        default:
                            throw new Exception("unknown command");
                    }
                    break;
                default:
                    if( !collectingDir) { throw new Exception("unknown input"); }
                    if (inv[0] == "dir") { // Add directory
                        currentDirectory.MakeSubDir(inv[1]);
                    }
                    else { // file
                        currentDirectory.MakeFile(inv[1], long.Parse(inv[0]));
                    }
                    break;
            }
        }
    }
    protected override string SolvePartOne() {
        List<Node> smallDirs = new();
        Queue<Node> walk = new();
        walk.Enqueue(tree);
        while(walk.Count > 0) {
            Node at = walk.Dequeue();
            if( at.GetTotalSize() <= 100000) {
                smallDirs.Add(at);
            }
            foreach(var node in at.Contents) {
                if (node.IsDirectory) {
                    walk.Enqueue(node);
                }
            }
        }
        return smallDirs.Sum(n => n.GetTotalSize()).ToString();
    }

    const long TotalSpace = 70000000;
    const long MinUnused = 30000000;
    protected override string SolvePartTwo() {


        List<Node> dirs = new();
        Queue<Node> walk = new();
        walk.Enqueue(tree);
        while (walk.Count > 0) {
            Node at = walk.Dequeue();
            dirs.Add(at);
            foreach (var node in at.Contents) {
                if (node.IsDirectory) {
                    walk.Enqueue(node);
                }
            }
        }
        long unsedSpace = TotalSpace - tree.GetTotalSize();
        long minToFree = MinUnused - unsedSpace;
        return dirs.Where(n => n.GetTotalSize() > minToFree)
            .OrderBy(n => n.GetTotalSize())
            .First()
            .GetTotalSize()
            .ToString();
    }
}
