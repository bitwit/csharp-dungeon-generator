using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGenerator {
    class GraphHelpers {

        static public AdjacencyListGraph<T, D> minimumSpanningTreeKruskal<T, D> (AdjacencyListGraph<T, D> graph) {

            double cost = 0;
            var tree = new AdjacencyListGraph<T, D> (graph);
            tree.removeAllEdges ();
            var sortedEdgeListByWeight = graph.edges.OrderBy (a => a.weight).ToList ();

            var unionFind = new UnionFind<Vertex<T>> ();
            foreach (var vertex in graph.vertices) {
                unionFind.addSetWith (vertex);
            }

            foreach (var edge in sortedEdgeListByWeight) {
                var v1 = edge.from;
                var v2 = edge.to;
                if (false == unionFind.inSameSet (v1, v2)) {
                    cost += edge.weight;
                    tree.addEdge (v1, v2, edge.data, edge.weight);
                    unionFind.unionSetsContaining (v1, v2);
                }
            }

            return tree;
        }

    }
}