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

namespace MyTests
{
    using System;
    using System.Composition;
    using NUnit.Framework;
    using Yarhl.Extension.Clone;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class CloneTests
    {
        [Test]
        public void NotCloneableFormatThrowsException()
        {
            var node = new Node("test", new IntFormatTest(3));
            Assert.Throws<InvalidOperationException>(() => _ = Yarhl.Extension.Clone.NodeFactory.FromNode(node));

            node.Dispose();
        }

        [Test]
        public void NullFormatIsCloneable()
        {
            var node = new Node("test", null);
            Node clone = Yarhl.Extension.Clone.NodeFactory.FromNode(node);

            Assert.AreNotSame(node, clone);
            Assert.AreEqual(node.Name, clone.Name);
            Assert.IsNull(clone.Format);

            node.Dispose();
            clone.Dispose();
        }

        [Test]
        public void BinaryFormatIsCloneable()
        {
            byte[] data = { 0, 1, 2, 3 };
            DataStream stream = DataStreamFactory.FromArray(data, 0, data.Length);
            BinaryFormat format = new (stream);

            var node = new Node("test", format);
            Node clone = Yarhl.Extension.Clone.NodeFactory.FromNode(node);

            Assert.AreNotSame(node, clone);
            Assert.AreEqual(node.Name, clone.Name);

            Assert.AreNotSame(node.Format, clone.Format);
            Assert.AreNotSame(node.Stream, clone.Stream);
            Assert.IsTrue(node.Stream.Compare(clone.Stream));

            node.Dispose();
            clone.Dispose();
        }

        [Test]
        public void CloneFormat()
        {
            var node = new Node("test", new CloneableIntFormatTest(3));
            Node clone = Yarhl.Extension.Clone.NodeFactory.FromNode(node);

            Assert.AreNotSame(node, clone);
            Assert.AreEqual(node.Name, clone.Name);
            Assert.AreNotSame(node.Format, clone.Format);

            CloneableIntFormatTest oldFormat = node.GetFormatAs<CloneableIntFormatTest>();
            CloneableIntFormatTest newFormat = clone.GetFormatAs<CloneableIntFormatTest>();

            Assert.AreEqual(oldFormat.Value, newFormat.Value);

            newFormat.Value = 4;

            Assert.AreNotEqual(oldFormat.Value, newFormat.Value);

            node.Dispose();
            clone.Dispose();
        }

        [Test]
        public void CloneTags()
        {
            var node = new Node("test", null);
            node.Tags["Tag1"] = "Value 1";
            node.Tags["Tag2"] = "Value 2";

            Node clone = Yarhl.Extension.Clone.NodeFactory.FromNode(node);

            Assert.AreNotSame(node, clone);
            Assert.AreEqual(node.Name, clone.Name);
            Assert.AreEqual(2, clone.Tags.Count);
            Assert.AreEqual("Value 1", clone.Tags["Tag1"]);
            Assert.AreEqual("Value 2", clone.Tags["Tag2"]);

            clone.Tags["Tag1"] = "New Value 1";
            Assert.AreNotEqual(node.Tags["Tag1"], clone.Tags["Tag1"]);

            node.Dispose();
            clone.Dispose();
        }

        [PartNotDiscoverable]
        private sealed class IntFormatTest : IFormat, IDisposable
        {
            public IntFormatTest()
            {
            }

            public IntFormatTest(int val)
            {
                Value = val;
            }

            public int Value { get; set; }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [PartNotDiscoverable]
        private sealed class CloneableIntFormatTest : ICloneableFormat, IDisposable
        {
            public CloneableIntFormatTest()
            {
            }

            public CloneableIntFormatTest(int val)
            {
                Value = val;
            }

            public int Value { get; set; }

            public bool Disposed { get; private set; }

            public object DeepClone()
            {
                return new CloneableIntFormatTest(Value);
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
