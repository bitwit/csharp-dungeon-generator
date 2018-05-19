using System.Collections.Generic;
using System.Linq;

public class AdjacencyListGraph<T, D> {
    
    internal List<EdgeList<T, D>> adjacencyList = new List<EdgeList<T, D>>();
    
    public AdjacencyListGraph() {}
    
    public AdjacencyListGraph(AdjacencyListGraph<T, D> graph) {
        foreach(var edge in graph.edges) {
            var from = createVertex(edge.from.data);
            var to = createVertex(edge.to.data);
            
            addEdge(from, to, edge.data, edge.weight);
        }
    }
    
    public List<Vertex<T>> vertices {
        get {
            var vertices = new List<Vertex<T>>();
            foreach(var edgeList in adjacencyList) {
                vertices.Add(edgeList.vertex);
            }
            return vertices;
        }
    }
    
    public List<Edge<T, D>> edges {
        get {
            //TODO: csharp set
            var allEdges = new HashSet<Edge<T, D>>();
            foreach(var edgeList in adjacencyList) {
                foreach(var edge in edges) {
                    allEdges.Add(edge);
                }
            }
            return allEdges.ToList();
        }
    }
    
    public Vertex<T> createVertex(T data) {
        // check if the vertex already exists
        //TODO: proper equatability
        var matchingVertices = vertices.Where(v => v.data.Equals(data)).ToList();
        
        if(matchingVertices.Count > 0) {
            return matchingVertices[0];
        }
        
        // if the vertex doesn't exist, create a new one
        var vertex = new Vertex<T>(data, adjacencyList.Count);
        adjacencyList.Add(new EdgeList<T, D>(vertex));
        return vertex;
    }
    
    public void addEdge(Vertex<T> from, Vertex<T> to, D data, double weight) {
        var edge = new Edge<T, D>(from, to, data, weight);
        var edgeList = adjacencyList[from.index];
        edgeList.addEdge(edge);
    }
    
    public void removeEdge(Edge<T, D> edge) {
        var list = adjacencyList[edge.from.index];
        var index = edges.IndexOf(edge);
        if (index != -1) {
            adjacencyList[edge.from.index].edges.RemoveAt(index);
        }
    }
    
    public void removeAllEdges() {
        for(var i = 0; i < adjacencyList.Count; i++) {
            adjacencyList[i].edges.Clear();
        }
    }
    
    public double weightFrom(Vertex<T> sourceVertex, Vertex<T> destinationVertex) {
        var edges = adjacencyList[sourceVertex.index].edges;
        
        foreach(var edge in edges) {
            //TODO: equatability
            if (edge.to.Equals(destinationVertex)) {
                return edge.weight;
            }
        }
        
        return -1;
    }
    
    public List<Edge<T, D>> edgesFrom(Vertex<T> sourceVertex) {
        return adjacencyList[sourceVertex.index].edges;
    }
    
    // open var description: String {
    //     var rows = [String]()
    //     for edgeList in adjacencyList {
            
    //         guard let edges = edgeList.edges else {
    //             continue
    //         }
            
    //         var row = [String]()
    //         for edge in edges {
    //             let value = "\(edge.to.data): \(edge.weight))"
    //             row.append(value)
    //         }
            
    //         rows.append("\(edgeList.vertex.data) -> [\(row.joined(separator: ", "))]")
    //     }
        
    //     return rows.joined(separator: "\n")
    // }
}
