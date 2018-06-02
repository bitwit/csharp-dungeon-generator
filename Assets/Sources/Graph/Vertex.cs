using System;

namespace DungeonGenerator {
  public struct Vertex<T> {

    public T data;
    public int index;

    public Vertex (T data, int index) {
      this.data = data;
      this.index = index;
    }

    public override int GetHashCode () {
      return ("" + data + index).GetHashCode ();
    }

  }
}

// public func == < T > (lhs: Vertex<T>, rhs: Vertex<T>) - > Bool {
//   guard lhs.index == rhs.index
//   else {
//     return false
//   }

//   guard lhs.data == rhs.data
//   else {
//     return false
//   }

//   return true
// }