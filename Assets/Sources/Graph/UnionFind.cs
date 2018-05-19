using System.Collections.Generic;

/*
 Union-Find Data Structure
 
 Performance:
 adding new set is almost O(1)
 finding set of element is almost O(1)
 union sets is almost O(1)
 */

public class UnionFind<T> {
    private Dictionary<T, int> index = new Dictionary<T, int>();
    private List<int> parent;
    private List<int> size;

    public UnionFind () {
        parent = new List<int> ();
        size = new List<int> ();
    }

    public void addSetWith (T element) {
        index[element] = parent.Count;
        parent.Add (parent.Count);
        size.Add (1);
    }

    private int setByIndex (int index) {
        if (parent[index] == index) {
            return index;
        } else {
            parent[index] = setByIndex (parent[index]);
            return parent[index];
        }
    }

    public int? setOf (T element) {
        int? indexOfElement = index[element];
        if (indexOfElement.HasValue) {
            return setByIndex (indexOfElement.Value);
        } else {
            return null;
        }
    }

    public void unionSetsContaining (T firstElement, T secondElement) {
        var firstSet = setOf (firstElement);
        var secondSet = setOf (secondElement);

        if (false == firstSet.HasValue || false == secondSet.HasValue) {
            return;
        }

        if (firstSet == secondSet) {
            return;
        }

        if (size[firstSet.Value] < size[secondSet.Value]) {
            parent[firstSet.Value] = secondSet.Value;
            size[secondSet.Value] += size[firstSet.Value];
        } else {
            parent[secondSet.Value] = firstSet.Value;
            size[firstSet.Value] += size[secondSet.Value];
        }
    }

    public bool inSameSet (T firstElement, T secondElement) {
        var firstSet = setOf (firstElement);
        var secondSet = setOf (secondElement);

        if (firstSet.HasValue && secondSet.HasValue) {
            return firstSet.Value == secondSet.Value;
        } else {
            return false;
        }
    }
}