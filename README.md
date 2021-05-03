# Yarhl Node Cloning Extension [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/Kaplas80/Yarhl.Extension.Clone/workflows/Build%20and%20release/badge.svg)

This is a extension for [Yarhl](https://scenegate.github.io/Yarhl/) allowing `Node` and `Format` cloning.

## IMPORTANT

**This project will be deleted if this functionality is merged into Yarhl.**

## Usage

```csharp
Node newNode = NodeFactory.FromNode(oldNode);
```

Note that original node format must implement `ICloneableFormat` or `IBinary`. Otherwise, the clone method will throw an exception.
