using System;

namespace CommandLineArgsToDict
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Init(args);


            Console.WriteLine("Hello World!");

            Console.WriteLine($"The key1{Config.Key1} key2:{Config.Key1}!");
        }
    }
}
