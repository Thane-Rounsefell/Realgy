// only define 1 example at a time
//
//#define Example_1
//#define Example_2
#define Example_3
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace RobotCommand
{
    class Program
    {
        // Grid declares the dimentions of the grid in rows and cols and lists all the obstacles
        // The top left corner is 1,1
        // code does not check for stacked obstacles and only processes the first obstacle created at a given grid location
        public class Grid
        {
            public Grid(int rows, int cols)
            {
                this.rows = rows;   // vertical size
                this.cols = cols;   // horisontal size         
                obstacles = new List<Obstacle>();
            }
            // return first obstacle at specified point in grid
            public Obstacle FindObstacle(Point point)
            {
                foreach (Obstacle obs in obstacles)
                    if (point == obs.point) { return obs; }
                return null;
            }
            public int rows;
            public int cols;
            public List<Obstacle> obstacles;
        }
        // Declare Obstacles
        // Obstacle base class
        public abstract class Obstacle
        {
            public string name;
            public Point point;  // X = Row, Y = Col
        }
        public class Obstacle_Rock : Obstacle
        {
            public Obstacle_Rock(Point point)
            {
                base.name = "Rock";
                base.point = point;
            }
        }
        public class Obstacle_Spinner : Obstacle
        {
            public Obstacle_Spinner(Point point, int increment)
            {
                base.name = "Spinner";
                base.point = point;
                this.increment = increment;
            }
            public int increment;
        }
        public class Obstacle_Hole : Obstacle
        {
            public Obstacle_Hole(Point point, int newRow, int newCol)
            {
                base.name = "Spinner";
                base.point = point;
                this.newRow = newRow;
                this.newCol = newCol;
            }
            public int newRow;
            public int newCol;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start Robot Command");

#if Example_1
            // EXAMPLE 1 - Walk around a 3X3 square using a Spinner in the corners
            Console.WriteLine("Start Example '3X3 Square walk-around using Spinners in corners'");
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Setup of Grid, Obstacles, and Robot
            // This could be read from a text file
            //
            Grid grid = new Grid(3, 3);
            Point robotPosition = new Point(1, 1);  // Robot starting position
            char facing = 'N';                      // Robot starting facing direction
            string commands = "RRRRRRRR";           // All move right commands
            //
            //Create_Rock(grid, 2, 2);        // add a Rock to the center
            //Create_Hole(grid, 3, 3, 2, 2);  // add a Hole to the grid
            Create_Spinner(grid, 1, 1, 1);    // add a Spinner to the grid
            Create_Spinner(grid, 1, 3, 1);    // add a Spinner to the grid
            Create_Spinner(grid, 3, 3, 1);    // add a Spinner to the grid
            Create_Spinner(grid, 3, 1, 1);    // add a Spinner to the grid
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif
#if Example_2
            // EXAMPLE 2 - 
            Console.WriteLine("Start Example '3X3 Square walk-around using holes in center of the sides'");
            // Note that I am using the B (Back) Command that I added for symmetry
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Setup of Grid, Obstacles, and Robot
            // This could be read from a text file
            //
            Grid grid = new Grid(3, 3);
            Point robotPosition = new Point(1, 1);   // Robot starting position
            char facing = 'N';                       // Robot starting facing direction
            string commands = "RBLF";                // Commands to move the robot
            //2
            //Create_Rock(grid, 2, 2);          // add a Rock to the center
            Create_Hole(grid, 1, 2, 1, 3);      // add a Hole to the grid
            Create_Hole(grid, 2, 3, 3, 3);      // add a Hole to the grid
            Create_Hole(grid, 3, 2, 3, 1);      // add a Hole to the grid
            Create_Hole(grid, 2, 1, 1, 1);      // add a Hole to the grid
            //Create_Spinner(grid, 1, 1, 1);    // add a Spinner to the grid
            //Create_Spinner(grid, 1, 3, 1);    // add a Spinner to the grid
            //Create_Spinner(grid, 3, 3, 1);    // add a Spinner to the grid
            //Create_Spinner(grid, 3, 1, 1);    // add a Spinner to the grid
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif
#if Example_3
            // EXAMPLE 3 - 
            Console.WriteLine("Start Example '5X10 Square with rocks - does a zig-zag to bottom corner");
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Setup of Grid, Obstacles, and Robot
            // This could be read from a text file
            //
            Grid grid = new Grid(6, 10);
            Point robotPosition = new Point(1, 1);              // Robot starting position
            char facing = 'N';                                  // Robot starting facing direction
            string commands = "RRRRBRRRRBRRRRBRRRRBRRRRBRRRR";  // Commands to move the robot
                                                                //2
            Create_Rock(grid, 1, 2);
            Create_Rock(grid, 2, 3);
            Create_Rock(grid, 3, 5);
            Create_Rock(grid, 4, 7);
            Create_Rock(grid, 5, 9);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif
            // At this point the following needs to have been created
            // a Grid object
            // a robotPosition Point 
            // a robot facing char N, E, S, or W 
            // a commands string
            //
            Console.WriteLine("COMMANDS: " + commands);
            Console.WriteLine("CMD: " + commands[0] + " Position: " + robotPosition.X + " " + robotPosition.Y + "   STARING POSITION");
            //
            Point newPosition = new Point(robotPosition.X, robotPosition.Y);
            Point holePosition = new Point(robotPosition.X, robotPosition.Y);
            bool executeHole = false;
            // execute each command
            for (int iCmd = 0; iCmd < commands.Length; ++iCmd)
            {
                newPosition = Move(robotPosition, facing, grid, commands[iCmd]);
                if ((robotPosition.X == newPosition.X) && (robotPosition.Y == newPosition.Y)) continue; // if no change then go around
                //                                                                                        //
                Obstacle obstacle = grid.FindObstacle(newPosition);
                if (obstacle != null)
                    switch (obstacle.name)
                    {
                        case "Rock":
                            Console.WriteLine("CMD: " + commands[iCmd] + " Position: " + robotPosition.X + " " + robotPosition.Y + "   HIT A ROCK");
                            continue;   // don't update robotPosition
                        case "Hole":
                            executeHole = true;
                            holePosition.X = ((Obstacle_Hole)obstacle).newRow;
                            holePosition.Y = ((Obstacle_Hole)obstacle).newCol;
                            break;
                        case "Spinner":
                            int i = "NESW".IndexOf(facing);
                            i += ((Obstacle_Spinner)obstacle).increment;
                            facing = "NESW"[i % 4];
                            break;
                        default: continue;   // unknown obstacle kills any move
                    }
                robotPosition = newPosition;
                Console.WriteLine("CMD: " + commands[iCmd] + " Position: " + robotPosition.X + " " + robotPosition.Y);
                if (executeHole)
                {
                    executeHole = false;
                    robotPosition = holePosition;
                    Console.WriteLine("CMD: " + commands[iCmd] + " Position: " + robotPosition.X + " " + robotPosition.Y + "   AFTER HOLE MOVE");
                }
            }
        }
        //
        // Obstacle creation
        //
        public static void Create_Rock(Grid grid, int row, int col)
        {
            Obstacle_Rock obstacle = new Obstacle_Rock(new Point(row, col));
            grid.obstacles.Add(obstacle);
        }
        public static void Create_Hole(Grid grid, int row, int col, int newRow, int newCol)
        {
            Obstacle_Hole obstacle = new Obstacle_Hole(new Point(row, col), newRow, newCol);
            grid.obstacles.Add(obstacle);
        }
        public static void Create_Spinner(Grid grid, int row, int col, int increment)
        {
            Obstacle_Spinner obstacle = new Obstacle_Spinner(new Point(row, col), increment);
            grid.obstacles.Add(obstacle);
        }
        //
        // Move returns current position if move would go off the grid
        public static Point Move(Point currentPosition, char facing, Grid grid, char moveDirection)
        {
            Point newPosition = currentPosition;
            int x = "NESW".IndexOf(facing);         // 0 1 2 3
            int y = "FRBL".IndexOf(moveDirection);  // 0 1 2 3
            int i = (x + y) % 4;
            switch (i)
            {
                case 0: if (currentPosition.X > 1) --newPosition.X; break;
                case 1: if (currentPosition.Y < grid.cols) ++newPosition.Y; break;
                case 2: if (currentPosition.X < grid.rows) ++newPosition.X; break;
                case 3: if (currentPosition.Y > 1) --newPosition.Y; break;
            }
            return newPosition;
        }
    }
}

