#region

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Sanford.Collections.Generic;

public partial class UndoableList<T>
{
    [Conditional("MIDIDEBUG")]
    public static void Test()
    {
        const int count = 10;
        var comparisonList = new List<int>(count);
        var undoList = new UndoableList<int>(count);

        PopulateLists(comparisonList, undoList, count);
        TestAdd(comparisonList, undoList);
        TestClear(comparisonList, undoList);
        TestInsert(comparisonList, undoList);
        TestInsertRange(comparisonList, undoList);
        TestRemove(comparisonList, undoList);
        TestRemoveAt(comparisonList, undoList);
        TestRemoveRange(comparisonList, undoList);
        TestReverse(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestAdd(IList<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        var redoStack = new Stack<int>();

        while (comparisonList.Count > 0)
        {
            redoStack.Push(comparisonList[comparisonList.Count - 1]);
            comparisonList.RemoveAt(comparisonList.Count - 1);
            Debug.Assert(undoList.Undo());
            TestEquals(comparisonList, undoList);
        }

        while (redoStack.Count > 0)
        {
            comparisonList.Add(redoStack.Pop());
            Debug.Assert(undoList.Redo());
            TestEquals(comparisonList, undoList);
        }
    }

    [Conditional("MIDIDEBUG")]
    private static void TestClear(ICollection<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        undoList.Clear();

        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestInsert(IList<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        var index = comparisonList.Count / 2;

        comparisonList.Insert(index, 999);
        undoList.Insert(index, 999);

        comparisonList.RemoveAt(index);
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);

        comparisonList.Insert(index, 999);
        Debug.Assert(undoList.Redo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestInsertRange(List<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        int[] range = { 1, 2, 3, 4, 5 };
        var index = comparisonList.Count / 2;

        comparisonList.InsertRange(index, range);
        undoList.InsertRange(index, range);

        TestEquals(comparisonList, undoList);

        comparisonList.RemoveRange(index, range.Length);
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);

        comparisonList.InsertRange(index, range);
        Debug.Assert(undoList.Redo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestRemove(IList<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        var index = comparisonList.Count / 2;

        var item = comparisonList[index];

        comparisonList.Remove(item);
        undoList.Remove(item);

        TestEquals(comparisonList, undoList);

        comparisonList.Insert(index, item);
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestRemoveAt(IList<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        var index = comparisonList.Count / 2;

        var item = comparisonList[index];

        comparisonList.RemoveAt(index);
        undoList.RemoveAt(index);

        TestEquals(comparisonList, undoList);

        comparisonList.Insert(index, item);
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestRemoveRange(List<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        var index = comparisonList.Count / 2;
        var count = comparisonList.Count - index;

        var range = comparisonList.GetRange(index, count);

        comparisonList.RemoveRange(index, count);
        undoList.RemoveRange(index, count);

        TestEquals(comparisonList, undoList);

        comparisonList.InsertRange(index, range);
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void TestReverse(List<int> comparisonList, UndoableList<int> undoList)
    {
        TestEquals(comparisonList, undoList);

        comparisonList.Reverse();
        undoList.Reverse();

        TestEquals(comparisonList, undoList);

        comparisonList.Reverse();
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);

        comparisonList.Reverse();
        Debug.Assert(undoList.Redo());

        TestEquals(comparisonList, undoList);

        var count = comparisonList.Count / 2;

        comparisonList.Reverse(0, count);
        undoList.Reverse(0, count);

        TestEquals(comparisonList, undoList);

        comparisonList.Reverse(0, count);
        Debug.Assert(undoList.Undo());

        TestEquals(comparisonList, undoList);

        comparisonList.Reverse(0, count);
        Debug.Assert(undoList.Redo());

        TestEquals(comparisonList, undoList);
    }

    [Conditional("MIDIDEBUG")]
    private static void PopulateLists(ICollection<int> a, ICollection<int> b, int count)
    {
        var r = new Random();

        for (var i = 0; i < count; i++)
        {
            var item = r.Next();
            a.Add(item);
            b.Add(item);
        }
    }

    [Conditional("MIDIDEBUG")]
    private static void TestEquals(ICollection<int> a, ICollection<int> b)
    {
        var equals = a.Count == b.Count;

        var aEnumerator = a.GetEnumerator();
        var bEnumerator = b.GetEnumerator();

        while (equals && aEnumerator.MoveNext() && bEnumerator.MoveNext())
            equals = aEnumerator.Current.Equals(bEnumerator.Current);

        Debug.Assert(equals);
    }
}