using System;
using System.Collections.Generic;
using System.IO;

namespace learningC_
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = ReadFrom("test.txt"); 
            foreach(var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        static IEnumerable<string> ReadFrom(string file) //IEnumerable is a generic iterator method, a method that iterates over a sequence returned. 
        {
            string line;
            //using manages resource allocation, automatically disposes the variable once the end of the funcion is reached, 
            using (var reader = File.OpenText(file)) //File is just a built in file reader class in System.IO
            {
                while((line = reader.ReadLine())!= null)
                {
                yield return line; //yield indicates to the compiler that the function returns an iterator.
                }
            }

        }
    }
}
