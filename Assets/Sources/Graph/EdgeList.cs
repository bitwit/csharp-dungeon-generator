using System.Collections.Generic;

public class EdgeList<T, D> {
    
    public Vertex<T> vertex;
    public List<Edge<T, D>> edges = new List<Edge<T, D>>();
    
    public EdgeList(Vertex<T> vertex) {
        this.vertex = vertex;
    }
    
    public void addEdge(Edge<T, D> edge) {
        this.edges.Add(edge);
    }
}
