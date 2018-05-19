using System;

namespace HelloWorld {

    class Hello {

        struct Constants {
            public static Size DungeonSize = new Size (width: 48, height: 48);
        }

        static void Main () {

            Console.WriteLine ("Dungeon Generator Start");
            var generator = new DungeonGenerator<DungeonRoom, DungeonHallway> ();
            generator.dungeonSize = Constants.DungeonSize;
            generator.creationBounds = Constants.DungeonSize;
            generator.minimumRoomWidth = 5;
            generator.minimumRoomHeight = 5;
            generator.maximumRoomWidth = 10;
            generator.maximumRoomHeight = 10;
            generator.initialRoomCreationCount = 30;
            generator.runCompleteGeneration ();

            Console.WriteLine ("Dungeon Generator Complete");
            Console.WriteLine ("Rooms: " + generator.dungeon.rooms.Count);
            Console.WriteLine ("Hallways: " + generator.dungeon.hallways.Count);
        }
    }
}