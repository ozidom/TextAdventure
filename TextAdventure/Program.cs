using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextAdventure
{
    class Program
    {

        public enum CommandEnum
        {
            Command = 0,
            FirstParameter
        }

        static void Main(string[] args)
        {
            Queue<Decimal> q = new Queue<decimal>();

            Building house = new Building();
            Game game = new Game(house);
            string message = "You have travelled for a long time for your quest, searching for a special cup. You arrive at a house. Is this the place?";

            while (!game.HasKeyBeenFound)
            {
                DisplayScreen(house, message);
                string[] commandParts = GetKeyBoardCommand();

                switch (commandParts[(int)CommandEnum.Command])
                {
                    case "MOVE":
                    case "M":
                        if (!house.Move(commandParts[(int)CommandEnum.FirstParameter]))
                        {
                            message = "Can't move there";
                        }
                        else
                        {
                            message = "Walking...";
                        }
                        break;
                    case "DESCRIBE":
                    case "D":
                        message = String.Join(",", house.CurrentRoom.Items.Select(s => s.Name)); //Using Linq to build a string of text for each item in room
                        break;
                    case "INVENTORY":
                    case "I":
                        message = String.Join(",", game.Inventory.Select(s => s.Name));
                        break;
                    case "PICKUP":
                    case "P":
                        Item pickedItem = house.CurrentRoom.Items.FirstOrDefault(i => i.Name == commandParts[1]);
                        game.Pickup(house.CurrentRoom, pickedItem);
                        message = String.Join(",", game.Inventory.Select(s => s.Name));
                        break;
                    default:
                        message = String.Join(",", house.CurrentRoom.Items.Select(s => s.Name));
                        break;
                }
            }
            Console.WriteLine("You found the cup well done - your quest is over - YOU WON");
            Console.ReadLine();
        }

        private static string[] GetKeyBoardCommand()
        {
            string command = Console.ReadLine();
            string CommandUpper = command.ToUpper();
            string[] commandParts = CommandUpper.Split(' ');
            return commandParts;
        }

        private static void DisplayScreen(Building house, string message)
        {
            Console.Clear();
            Console.WriteLine("You are in " + house.CurrentRoom.RoomName);
            Console.WriteLine("Exits: " + house.CurrentRoom.DisplayExits());
            Console.WriteLine(message);
            Console.Write(">");
        }
    }
    public class Game
    {
        public List<Item> Inventory { get; set; }
        public Item Key { get; set; }
        public bool HasKeyBeenFound { get; set; }

        public Game(Building building)
        {
            Key = building.Key;
            Inventory = new List<Item>();
            HasKeyBeenFound = false;
        }

        public bool Pickup(Room r, Item i)
        {
            //does the room and the item exist
            if (r == null || i == null)
            {
                return false;
            }

            if (!r.Items.Contains(i))
            {
                return false;
            }
            else
            {
                r.Items.Remove(i);
                Inventory.Add(i);
                CheckForKeyBeingPickedUp(i);
                return true;
            }
            //does item exist in room
        }

        private void CheckForKeyBeingPickedUp(Item i)
        {
            if (Key.Equals(i))
            {
                HasKeyBeenFound = true;
            }
        }
    }

    public class Building
    {
        public Room CurrentRoom { get; set; }
        public static int ItemCount;
        public Item Key { get; set; }

        public Building()
        {
            //Create a sample House
            CreateSampleHouse();
        }

        private void CreateSampleHouse()
        {
            Room r1 = new Room("ENTRY");
            Room r2 = new Room("BEDROOM");
            Room r3 = new Room("CORRIDOR");

            //Add the Exits for this building
            r1.AddExit(r3);
            r3.AddExit(r1);
            r3.AddExit(r2);
            r2.AddExit(r3);

            //Add Items
            r2.AddItem(new Item("TABLE"));
            r2.AddItem(new Item("CHAIR"));
            Item cup = new Item("CUP");
            r2.AddItem(cup);

            Key = cup;
            CurrentRoom = r1;
        }

        public bool Move(string roomName)
        {
            bool canMove = false;
            Room room = CurrentRoom.Exits.FirstOrDefault(r => r.RoomName == roomName);
            if (room != null)
            {
                CurrentRoom = room;
                canMove = true;
            }
            return canMove;
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public Item(string name)
        {
            Name = name;
        }
    }

    public class Room
    {
        public List<Room> Exits;// = new List<Room>();
        public List<Item> Items;
        public string RoomName { get; set; }

        public Room(string name)
        {
            RoomName = name;
            Exits = new List<Room>();
            Items = new List<Item>();
        }

        public string DisplayExits()
        {
            if (Exits.Any())
            {
                StringBuilder s = new StringBuilder();
                Exits.ForEach(e => s.Append(e.RoomName + " "));
                return s.ToString();
            }
            else
            {
                return "No Exits";
            }
        }

        public void AddExit(Room r)
        {
            Exits.Add(r);
        }

        public void AddItem(Item i)
        {
            Items.Add(i);
        }
    }
}
