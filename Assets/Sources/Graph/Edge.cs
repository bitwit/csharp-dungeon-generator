using System;

public struct Edge<T, D> {

  public Vertex<T> from;
  public Vertex<T> to;
  public D data;
  public double weight;

  public Edge(Vertex<T> from, Vertex<T> to, D data, double weight) {
    this.from = from;
    this.to = to;
    this.data = data;
    this.weight = weight;
  }

  public override int GetHashCode() {
    var str = "" + from + to + weight;
    return str.GetHashCode();
  }

}

// public func == <T, D>(lhs: Edge<T, D>, rhs: Edge<T, D>) -> Bool {
//   guard lhs.from == rhs.from else {
//     return false
//   }

//   guard lhs.to == rhs.to else {
//     return false
//   }

//   guard lhs.weight == rhs.weight else {
//     return false
//   }

//   return true
// }
