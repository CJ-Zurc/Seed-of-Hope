using System;
using System.Collections.Generic;


    public abstract class Vehicle
    {
        public string Brand {get; set;}
        public string Model {get; set;}
        public int Year {get; set;}
        
        public Vehicle (string brand, string model, int year){
            Brand = brand;
            Model = model;
            Year = year;
        }
        

        public void ShowDetails()
        {
            Console.WriteLine($"Vehicle Info: {Brand} {Model}, Year: {Year}");
        }
        
        public abstract void Drive();


    }

    public class Car : Vehicle
    {
        public Car (string brand, string model, int year) : base (brand, model, year){}
        public override void Drive()
        {
            Console.WriteLine("Driving a car.");
        }
    }

    public class Motorcycle : Vehicle
    {
        public Motorcycle (string brand, string model, int year) : base (brand, model, year){}
        public override void Drive()
        {
            Console.WriteLine("Riding a motorcycle.");
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            List<Vehicle> vehicles = new List<Vehicle>{
                new Car("Toyota", "Corolla", 2020),
                new Motorcycle("Yamaha", "R1", 2019)
            };
            
            foreach (var vehicle in vehicles){
                vehicle.ShowDetails();
                vehicle.Drive();
                Console.WriteLine();
            }
        }
    }

