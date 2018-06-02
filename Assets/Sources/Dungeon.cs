using System;
using System.Collections.Generic;

namespace DungeonGenerator {
    public class Dungeon<RoomType, HallwayType> : AdjacencyListGraph<RoomType, HallwayType>
        where RoomType : DungeonRoom
    where HallwayType : DungeonHallway {

        public List<RoomType> rooms {
            get {
                var rooms = new List<RoomType> ();
                foreach (var edgeList in this.adjacencyList) {
                    rooms.Add (edgeList.vertex.data);
                }
                return rooms;
            }
        }

        public List<HallwayType> hallways {
            get {
                var hallways = new List<HallwayType> ();
                foreach (var edge in edges) {
                    hallways.Add (edge.data);
                }
                return hallways;
            }
        }

        public Dungeon () : base () {

        }

        public Dungeon (AdjacencyListGraph<RoomType, HallwayType> graph) : base (graph) {

        }

    }
}