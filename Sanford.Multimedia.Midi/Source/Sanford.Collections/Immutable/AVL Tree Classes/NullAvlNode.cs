#region

using System.ComponentModel;

#endregion

namespace Sanford.Collections.Immutable
{
    /// <summary>
    ///     Represents a null AVL node.
    /// </summary>
    [ImmutableObject(true)]
    internal sealed class NullAvlNode : IAvlNode
    {
        #region IAvlNode Members

        /// <summary>
        ///     Removes the current node from the AVL tree.
        /// </summary>
        /// <returns>
        ///     The node to in the tree to replace the current node.
        /// </returns>
        public IAvlNode Remove()
        {
            return this;
        }

        /// <summary>
        ///     Balances the subtree represented by the node.
        /// </summary>
        /// <returns>
        ///     The root node of the balanced subtree.
        /// </returns>
        public IAvlNode Balance()
        {
            return this;
        }

        /// <summary>
        ///     Indicates whether or not the subtree the node represents is in
        ///     balance.
        /// </summary>
        /// <returns>
        ///     <b>true</b> if the subtree is in balance; otherwise, <b>false</b>.
        /// </returns>
        public bool IsBalanced()
        {
            return true;
        }

        /// <summary>
        ///     Gets the balance factor of the subtree the node represents.
        /// </summary>
        public int BalanceFactor => 0;

        /// <summary>
        ///     Gets the number of nodes in the subtree.
        /// </summary>
        public int Count => 0;

        /// <summary>
        ///     Gets the node's data.
        /// </summary>
        public object Data => null;

        /// <summary>
        ///     Gets the height of the subtree the node represents.
        /// </summary>
        public int Height => 0;

        /// <summary>
        ///     Gets the node's left child.
        /// </summary>
        public IAvlNode LeftChild => this;

        /// <summary>
        ///     Gets the node's right child.
        /// </summary>
        public IAvlNode RightChild => this;

        #endregion
    }
}