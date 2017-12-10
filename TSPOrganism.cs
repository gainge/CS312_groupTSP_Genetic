using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class TSPOrganism
    {
        /* Constants */
        private const double MAX_CROSSOVER_PERCENTAGE = 0.4;
        private const double MIN_CROSSOVER_PERCENTAGE = 0.1;

        /* "Constants" */
        public static int NUM_CITIES;
        public static City[] Cities;                    // Used for referencing the cities
        public static bool isOnlyValidRoutes = true;    // whether or not we care if we generate infinity routes
        private static Random rnd = new Random();

        /* Data Members */
        private ArrayList route;       // Private member for the route itself.  List of cities
        private double fitness;

        /* Constructors */

        public TSPOrganism()
        {
            route = new ArrayList();
            fitness = Double.PositiveInfinity;
        }

        public TSPOrganism(ArrayList route)
        {
            this.route = route;
            fitness = computeCost(this.route);
        }


        /* Accessor Methods */
        public ArrayList getRoute()
        {
            return route;
        }

        public void setRoute(ArrayList route)
        {
            this.route = route;
        }

        public static void setCities(City[] cities)
        {
            Cities = cities;
        }

        public static bool getIsOnlyValidRoutes()
        {
            return isOnlyValidRoutes;
        }

        public static void setIsOnlyValidRoutes(bool onlyValidRoutes)
        {
            isOnlyValidRoutes = onlyValidRoutes;
        }

        public static void setSeed(int seed)
        {
            rnd = new Random(seed);
        }
        

        /* Member Fucntions */
        public static TSPOrganism newOrganism()
        {
            int i, swap, temp, count = 0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            ArrayList newRoute = new ArrayList();

            // We only really need this loop if we care about infinite routes...
            // It should be easy enough to 
            do
            {
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = rnd.Next(0, Cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                newRoute.Clear();
                for (i = 0; i < Cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    newRoute.Add(Cities[perm[i]]);
                }
                //bssf = new TSPSolution(retRoute);
                count++;
            } while (isOnlyValidRoutes && computeCost(newRoute) == double.PositiveInfinity);                // until a valid route is found

            return new TSPOrganism(newRoute);
        }


        public TSPOrganism crossover(TSPOrganism parent)
        {
            int start = 0;
            int end = 0;
            int diff = 0;

            // Define a range
            while (true)
            {
                // Obtain two indices to swap
                start = rnd.Next(0, NUM_CITIES);
                end = rnd.Next(0, NUM_CITIES);

                if (start == end) continue;

                // How much did we chop off?
                diff = Math.Abs(start - end);
                double percentage =  (double) diff / route.Count;

                if (percentage <= MAX_CROSSOVER_PERCENTAGE) break;
            }

            // Force the start to be the lower number, and end to be the higher
            // this is mainly just for sanity's sake
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            // Now for the fun bit!
            ArrayList childRoute = new ArrayList();
            childRoute.AddRange(route.GetRange(start, diff));    // Add the subsequence to the child's list

            // Now, loop through the second parent, starting after 'end'
            // If the element is not contained in the child route, we add it (to the end)
            for (int i = end + 1; i < NUM_CITIES; i++)
            {
                City currentCity = parent.getRoute()[i] as City;

                if (childRoute.Contains(currentCity)) continue;
                childRoute.Add(currentCity);
            }
            // Then we have to add the cities before 'end' that we may have missed
            ArrayList prefix = new ArrayList();
            for (int i = 0; i < end + 1; i++)
            {
                City currentCity = parent.getRoute()[i] as City;

                if (childRoute.Contains(currentCity)) continue;
                //childRoute.Insert(0, currentCity);
                prefix.Add(currentCity);
            }

            // Add the pre-pended portion
            childRoute.InsertRange(0, prefix);

            return new TSPOrganism(childRoute);
        }

        public TSPOrganism mutate()
        {
            // Create a new instance of our array list to serve as the mutated array list
            ArrayList mutatedRoute = new ArrayList();
            mutatedRoute.AddRange(this.route);

            // Obtain two indices to swap
            int index1 = rnd.Next(0, NUM_CITIES);   // Don't swap the start/end
            int index2 = rnd.Next(0, NUM_CITIES);   // Num cities is always route.Count - 1
            while (index1 == index2) index2 = rnd.Next(0, NUM_CITIES);

            // Now we just swap!
            City temp = mutatedRoute[index1] as City;
            mutatedRoute[index1] = mutatedRoute[index2];
            mutatedRoute[index2] = temp;

            return new TSPOrganism(mutatedRoute);
        }

        public double getFitness()
        {
            return fitness;
        }

        /* Helper Fucntions */
        private static double computeCost(ArrayList route)
        {
            // go through each edge in the route and add up the cost. 
            int x;
            City here;
            double cost = 0D;

            for (x = 0; x < route.Count - 1; x++)
            {
                here = route[x] as City;
                cost += here.costToGetTo(route[x + 1] as City);
            }

            // go from the last city to the first. 
            here = route[route.Count - 1] as City;
            cost += here.costToGetTo(route[0] as City);

            return cost;
        }

    }
}
