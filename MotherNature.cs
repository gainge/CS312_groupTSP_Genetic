using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class MotherNature
    {
        /* DATA MEMBERS */
        private double percentMated;    // Percent of offspring created just by matching parents together
        private double percentMutated;  // Percent of offspring created by mutating children
        private double percentRandom;   // Percent of offspring that are randomly created

        private int populationSize;


        /* CONSTRUCTORS */
        // Default Constructor
        public MotherNature()
        {
            // Default population size
            populationSize = 100;   

            // Default parameters
            percentMated = 0.6;
            percentMutated = 0.3;
            percentRandom = 0.1;

        }

        public MotherNature(int populationSize, double matingPercent, double mutationPercent, double randomPercent)
        {
            this.populationSize = populationSize;

            percentMated = matingPercent;
            percentMutated = mutationPercent;
            percentRandom = randomPercent;
        }

        /* ACCESSOR METHODS */
        // I generatd these thinking I'd use them but I never did lol
        public double PercentMated { get => percentMated; set => percentMated = value; }
        public double PercentMutated { get => percentMutated; set => percentMutated = value; }
        public double PercentRandom { get => percentRandom; set => percentRandom = value; }
        public int PopulationSize { get => populationSize; set => populationSize = value; }



        /* MEMBER METHODS */
        public List<TSPOrganism> generateInitialPopulation()
        {
            List<TSPOrganism> population = new List<TSPOrganism>();

            for (int i = 0; i < populationSize; i++)
            {
                population.Add(TSPOrganism.newOrganism());
            }

            return population;
        }

        public List<TSPOrganism> createOffspring(List<TSPOrganism> parents)
        {
            List<TSPOrganism> totalOffspring = new List<TSPOrganism>();

            totalOffspring.AddRange(mateForOffpring(parents));
            totalOffspring.AddRange(mutateForOffspring(parents));
            totalOffspring.AddRange(randomizeForOffspring());
            // We should never exceed the population size right now

            // Add randoms to cap off if we're still not at the pop size
            while (totalOffspring.Count < populationSize)
            {
                totalOffspring.Add(TSPOrganism.newOrganism());
            }

            return totalOffspring;
        }


        /* HELPER FUNCTIONS */
        private List<TSPOrganism> mateForOffpring(List<TSPOrganism> parents)
        {
            List<TSPOrganism> offspring = createOrganisms(parents, percentMated);
            return offspring;
        }

        private List<TSPOrganism> mutateForOffspring(List<TSPOrganism> parents)
        {
            List<TSPOrganism> offspring = createOrganisms(parents, percentMutated);

            // Now we just need to mutate them
            for (int i = 0; i < offspring.Count; i++)
            {
                offspring[i] = offspring[i].mutate();
            }

            return offspring;
        }

        private List<TSPOrganism> randomizeForOffspring()
        {
            List<TSPOrganism> randomOffspring = new List<TSPOrganism>();

            int targetVolume = getTargetVolume(percentRandom);

            for (int i = 0; i < targetVolume; i++)
            {
                randomOffspring.Add(TSPOrganism.newOrganism());
            }

            return randomOffspring;
        }


        private List<TSPOrganism> createOrganisms(List<TSPOrganism> parents, double percentOfPopulation)
        {
            List<TSPOrganism> offspring = new List<TSPOrganism>();

            int targetVolume = getTargetVolume(percentOfPopulation);

            int i = 0;
            while (true)
            {
                if (i + 1 >= parents.Count) i = 0;  // Handle wraparound

                TSPOrganism parent1 = parents[i];
                TSPOrganism parent2 = parents[i + 1];

                offspring.Add(parent1.crossover(parent2));
                offspring.Add(parent2.crossover(parent1));

                if (offspring.Count >= targetVolume) break;
            }


            return offspring;
        }

        
        private int getTargetVolume(double targetPercentage)
        {
            int targetVolume = (int)(targetPercentage * populationSize);

            if (targetVolume % 2 == 1) targetVolume--;  // Only deal with even numbers

            return targetVolume;
        }

    }
}
