using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks; 

namespace learningC_
{
    class Program
    {
        static void Main(string[] args)
        {
            //ReadFrom returns an iterable object, a type of object that can be used in foreach, contains code to loop to the next entry. 
            //Synchronous Code:
            /*var lines = ReadFrom("test.txt"); 
            foreach(var line in lines)
            {
                Console.WriteLine(line);
                if (!string.IsNullOrWhiteSpace(line)) Task.Delay(200).Wait();
            }*/

            //Async code, Wait blocks execution until task returns. Can't use await in Main. 
            //ShowTeleprompter().Wait();
            
            //New Async Code using RunTelemprompter
            RunTeleprompter().Wait();
        }

        //Uses TelePrompter config class to manage execution
        private static async Task RunTeleprompter()
        {
            var config = new TelePrompterConfig();
            var displayTask = ShowTeleprompter(config);

            var speedTask = GetInput(config);
            //Waits until either thread completes execution
            await Task.WhenAny(displayTask, speedTask);
        }

        static IEnumerable<string> ReadFrom(string file) //IEnumerable is a generic iterator method, a method that iterates over a sequence returned. 
        {
            string line;
            //using manages resource allocation, automatically disposes the variable once the end of the funcion is reached, 
            using (var reader = File.OpenText(file)) //File is just a built in file reader class in System.IO
            {
                while((line = reader.ReadLine())!= null)
                {
                    var words = line.Split(); 

                    foreach(var word in words)
                    {
                        yield return word + " ";
                    }
                    yield return Environment.NewLine; 
                }
            }

        }
        //Asynchronously display the teleprompter on this thread
        //Async functions must be labelled with Task and ShowTeleprompter
        private static async Task ShowTeleprompter(TelePrompterConfig config)
        {
            var words = ReadFrom("test.txt"); 
            foreach (var word in words)
            {
                Console.WriteLine(word); 
                if (!string.IsNullOrWhiteSpace(word)) await Task.Delay(config.DelayInMilleseconds);
            }
            //Sets done when finished with execution
            config.SetDone();
        }

        private static async Task GetInput(TelePrompterConfig  config)
        {
            var delay = 200; 
            //Creates an action lambada function that is executed until the loop exits, lambada function is also a delegate
            Action work = () => 
            {
                while(!config.Done)
                {   
                    var key = Console.ReadKey(true); 

                    if (key.KeyChar == '>')
                    {
                        config.UpdateDelay(-10);
                    }
                    else if (key.KeyChar == '<')
                    {
                        config.UpdateDelay(10);
                    }
                    else if (key.KeyChar == 'x')
                    {
                        config.SetDone();
                    }
                }
            };
            //Runs the provided delegate until function returns
            await Task.Run(work);
        }
    }
    //Class the allows interface between input and teleprompter
    internal class TelePrompterConfig 
    {
        public int DelayInMilleseconds{get; private set;} = 200; 

        public bool Done {get; private set;}

        public void UpdateDelay(int increment)
        {
            DelayInMilleseconds += increment;
            
        }

        public void SetDone()
        {
            Done = true;
        }
    }
}
