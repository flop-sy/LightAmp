#region

using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Sanford.Collections.Generic;

public partial class UndoableList<T>
{
    #region SetCommand

    private sealed class SetCommand : ICommand
    {
        private readonly int index;

        private readonly T newItem;
        private readonly IList<T> theList;

        private T oldItem;

        private bool undone = true;

        public SetCommand(IList<T> theList, int index, T item)
        {
            this.theList = theList;
            this.index = index;
            newItem = item;
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            Debug.Assert(index >= 0 && index < theList.Count);

            oldItem = theList[index];
            theList[index] = newItem;
            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            Debug.Assert(index >= 0 && index < theList.Count);
            Debug.Assert(theList[index].Equals(newItem));

            theList[index] = oldItem;
            undone = true;
        }

        #endregion
    }

    #endregion

    #region InsertCommand

    private sealed class InsertCommand : ICommand
    {
        private readonly int index;

        private readonly T item;
        private readonly IList<T> theList;
        private int count;

        private bool undone = true;

        public InsertCommand(IList<T> theList, int index, T item)
        {
            this.theList = theList;
            this.index = index;
            this.item = item;
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            Debug.Assert(index >= 0 && index <= theList.Count);

            count = theList.Count;
            theList.Insert(index, item);
            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            Debug.Assert(index >= 0 && index <= theList.Count);
            Debug.Assert(theList[index].Equals(item));

            theList.RemoveAt(index);
            undone = true;

            Debug.Assert(theList.Count == count);
        }

        #endregion
    }

    #endregion

    #region InsertRangeCommand

    private sealed class InsertRangeCommand : ICommand
    {
        private readonly int index;

        private readonly List<T> insertList;
        private readonly List<T> theList;

        private bool undone = true;

        public InsertRangeCommand(List<T> theList, int index, IEnumerable<T> collection)
        {
            this.theList = theList;
            this.index = index;

            insertList = new List<T>(collection);
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            Debug.Assert(index >= 0 && index <= theList.Count);

            theList.InsertRange(index, insertList);

            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            Debug.Assert(index >= 0 && index <= theList.Count);

            theList.RemoveRange(index, insertList.Count);

            undone = true;
        }

        #endregion
    }

    #endregion

    #region RemoveAtCommand

    private sealed class RemoveAtCommand : ICommand
    {
        private readonly int index;
        private readonly IList<T> theList;
        private int count;

        private T item;

        private bool undone = true;

        public RemoveAtCommand(IList<T> theList, int index)
        {
            this.theList = theList;
            this.index = index;
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            Debug.Assert(index >= 0 && index < theList.Count);

            item = theList[index];
            count = theList.Count;
            theList.RemoveAt(index);
            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            Debug.Assert(index >= 0 && index < theList.Count);

            theList.Insert(index, item);
            undone = true;

            Debug.Assert(theList.Count == count);
        }

        #endregion
    }

    #endregion

    #region RemoveRangeCommand

    private sealed class RemoveRangeCommand : ICommand
    {
        private readonly int count;

        private readonly int index;
        private readonly List<T> theList;

        private List<T> rangeList = new();

        private bool undone = true;

        public RemoveRangeCommand(List<T> theList, int index, int count)
        {
            this.theList = theList;
            this.index = index;
            this.count = count;
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            Debug.Assert(index >= 0 && index < theList.Count);
            Debug.Assert(index + count <= theList.Count);

            rangeList = new List<T>(theList.GetRange(index, count));

            theList.RemoveRange(index, count);

            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            theList.InsertRange(index, rangeList);

            undone = true;
        }

        #endregion
    }

    #endregion

    #region ClearCommand

    private sealed class ClearCommand : ICommand
    {
        private readonly IList<T> theList;

        private IList<T> undoList;

        private bool undone = true;

        public ClearCommand(IList<T> theList)
        {
            this.theList = theList;
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            undoList = new List<T>(theList);

            theList.Clear();

            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            Debug.Assert(theList.Count == 0);

            foreach (var item in undoList) theList.Add(item);

            undoList.Clear();

            undone = true;
        }

        #endregion
    }

    #endregion

    #region ReverseCommand

    private sealed class ReverseCommand : ICommand
    {
        private readonly int count;

        private readonly int index;

        private readonly bool reverseRange;
        private readonly List<T> theList;

        private bool undone = true;

        public ReverseCommand(List<T> theList)
        {
            this.theList = theList;
            reverseRange = false;
        }

        public ReverseCommand(List<T> theList, int index, int count)
        {
            this.theList = theList;
            this.index = index;
            this.count = count;
            reverseRange = true;
        }

        #region ICommand Members

        public void Execute()
        {
            #region Guard

            if (!undone) return;

            #endregion

            if (reverseRange)
                theList.Reverse(index, count);
            else
                theList.Reverse();

            undone = false;
        }

        public void Undo()
        {
            #region Guard

            if (undone) return;

            #endregion

            if (reverseRange)
                theList.Reverse(index, count);
            else
                theList.Reverse();

            undone = true;
        }

        #endregion
    }

    #endregion
}