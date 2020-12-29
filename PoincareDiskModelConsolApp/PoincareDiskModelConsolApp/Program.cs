using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using PoincareDiskModelConsolApp.Helpers;

namespace PoincareDiskModelConsolApp
{
    class Program
    {
        int p;
        int q;
        int iteration;
        static void Main(string[] args)
        {

            try
            {
                string val;

                Console.WriteLine(
                    "In order to generate poencare disk tesalation:\n " +
                    "(1) Number of sides of polygon: p\n" +
                    " (2) Number of polygons that meet at every vertex: q\n" +
                    " (3) Number of iteration: i\n"
                    );

                Console.WriteLine("Enter p:");
                val = Console.ReadLine();
                int p = Convert.ToInt32(val);

                Console.WriteLine("Enter q: ");
                val = Console.ReadLine();
                int q = Convert.ToInt32(val);

                Console.WriteLine("Enter i: ");
                val = Console.ReadLine();
                int iteration = Convert.ToInt32(val);

                using (OpenTKHelper openTKHelper = new OpenTKHelper(800, 800, "LearnOpenTK", p, q, iteration))
                {
                    //Run takes a double, which is how many frames per second it should strive to reach.
                    //You can leave that out and it'll just update as fast as the hardware will allow it.
                    openTKHelper.Run(60.0);

                }

            }
            catch(Exception exception)
                {
                    Console.WriteLine(exception.Message); 
                }

            

        }
    }
}
