// Copyright (c) 2021 Kaplas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Yarhl.Extension.Clone
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Factory for Node cloning.
    /// </summary>
    public static class NodeFactory
    {
        /// <summary>
        /// Creates a Node from another Node, deepcloning it.
        /// </summary>
        /// <param name="node">The node to clone.</param>
        /// <returns>The cloned node.</returns>
        /// <remarks>The node format must implement <see cref="ICloneableFormat"/>.</remarks>
        public static Node FromNode(Node node)
        {
            if (node!.Format != null && node.Format is not ICloneableFormat && node.Format is not IBinary)
            {
                throw new InvalidOperationException("Format does not implement ICloneableFormat interface.");
            }

            var newNode = new Node(node.Name);
            IFormat newFormat = null;
            switch (node.Format)
            {
                case ICloneableFormat cloneableFormat:
                    newFormat = (ICloneableFormat)cloneableFormat!.DeepClone();
                    break;
                case IBinary binaryFormat:
                    {
                        DataStream newStream = DataStreamFactory.FromMemory();
                        binaryFormat.Stream.WriteTo(newStream);
                        newFormat = new BinaryFormat(newStream);
                        break;
                    }
            }

            newNode.ChangeFormat(newFormat);

            foreach ((string key, dynamic value) in node.Tags)
            {
                newNode.Tags[key] = value;
            }

            return newNode;
        }
    }
}
