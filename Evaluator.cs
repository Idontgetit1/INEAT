using System;
using System.Collections.Generic;

public abstract class Evaluator {

    private Counter nodeInnovation;
    private Counter connectionInnovation;

    private int populationSize;
    private Dictionary<Genome, Species> speciesDict;
    private Dictionary<Genome, float> scoreMap;
    public float highestScore;
    public Genome fittestGenome;
    public List<Genome> genomes;
    public List<Species> species;
    private List<Genome> nextGenGenomes;
    const float COMPATIBILITY_THRESHOLD = 5.0f;
    const float MUTATION_RATE = 0.1f;
    const float ADD_CONNECTION_RATE = 0.05f;
    const float ADD_NODE_RATE = 0.05f;
    public Random random = new Random();

    public Evaluator(int populationSize, Genome startingGenome, Counter nodeInnovation, Counter connectionInnovation) {
        this.populationSize = populationSize;
        this.nodeInnovation = nodeInnovation;
        this.connectionInnovation = connectionInnovation;
        genomes = new List<Genome>();
        for (int i = 0; i < populationSize; i++) {
            genomes.Add(new Genome(startingGenome));
        }
        nextGenGenomes = new List<Genome>();
        speciesDict = new Dictionary<Genome, Species>();
        scoreMap = new Dictionary<Genome, float>();
        species = new List<Species>();
    }

    public void evaluate(){
        // reset everything
        foreach (Species species in species) {
            species.reset(random);
        }

        scoreMap.Clear();
        speciesDict.Clear();
        nextGenGenomes.Clear();
        highestScore = float.MinValue;
        fittestGenome = null;

        // place genomes into species
        foreach (Genome g in genomes) {
            bool found = false;
            foreach (Species s in species) {
                if (Genome.compatibilityDistance(g, s.mascot) < COMPATIBILITY_THRESHOLD) {
                    s.members.Add(g);
                    speciesDict.Add(g, s);
                    found = true;
                    break;
                }
            }
            if (!found) {
                Species s = new Species(g);
                species.Add(s);
                speciesDict.Add(g, s);
            }
        }

        // remove unused species
        for(int i = species.Count - 1; i >= 0; i--) {
            if (species[i].members.Count == 0) {
                species.RemoveAt(i);
            }
        }

        // evaluate genomes and assign fitness
        foreach (Genome g in genomes) {
            Species s = speciesDict[g];

            float score = evaluateGenome(g);
            float adjustedScore = score/s.members.Count;

            s.addAdjustedFitness(adjustedScore);
            s.fitnessPop.Add(new FitnessGenome(g, adjustedScore));
            scoreMap.Add(g, adjustedScore);
            if (score > highestScore){
                highestScore = score;
                fittestGenome = g;
            }
        }

        // put best genomes from each species into next generation
        foreach (Species s in species) {
            s.fitnessPop.Sort(sortFitnessGenomes);
            s.fitnessPop.Reverse();
            FitnessGenome fittestInSpecies = s.fitnessPop[0];
            nextGenGenomes.Add(fittestInSpecies.genome);
        }

        // Breed the rest of the genomes
        while (nextGenGenomes.Count < populationSize) {
            Species s = getRandomSpeciesBiasedAdjustedFitness(random);

            Genome p1 = getRandomGenomeBiasedAdjustedFitness(s, random);
            Genome p2 = getRandomGenomeBiasedAdjustedFitness(s, random);

            Genome child;
            if (scoreMap[p1] >= scoreMap[p2]) {
                child = Genome.crossover(p1, p2, random);
            } else {
                child = Genome.crossover(p2, p1, random);
            }

            if (random.NextDouble() < MUTATION_RATE) {
                child.mutation(random);
            }
            if(random.NextDouble() < ADD_CONNECTION_RATE) {
                child.addConnectionMutation(connectionInnovation, 10, random);
            }
            if(random.NextDouble() < ADD_NODE_RATE) {
                child.addNodeMutation(connectionInnovation, nodeInnovation, random);
            }

            nextGenGenomes.Add(child);
        }

        genomes = nextGenGenomes;
        nextGenGenomes = new List<Genome>();
    }

    private Species getRandomSpeciesBiasedAdjustedFitness(Random random) {
        double completeWeight = 0.0;
        foreach (Species s in species) {
            completeWeight += s.totalAdjustedFitness;
        }

        double r = random.NextDouble() * completeWeight;
        double countWeight = 0.0;
        foreach (Species s in species) {
            countWeight += s.totalAdjustedFitness;
            if (countWeight >= r) {
                return s;
            }
        }

        throw new Exception("Could not find species. Number of total Species is " + species.Count);
    }

    private Genome getRandomGenomeBiasedAdjustedFitness(Species selectFrom, Random random) {
        double completeWeight = 0.0;
        foreach (FitnessGenome fg in selectFrom.fitnessPop) {
            completeWeight += fg.fitness;
        }

        double r = random.NextDouble() * completeWeight;
        double countWeight = 0.0;
        foreach (FitnessGenome fg in selectFrom.fitnessPop) {
            countWeight += fg.fitness;
            if (countWeight >= r) {
                return fg.genome;
            }
        }

        throw new Exception("Could not find genome. Number of Genomes in selected Species is " + selectFrom.fitnessPop.Count);
    }

    private int sortFitnessGenomes(FitnessGenome a, FitnessGenome b) {
        if (a.fitness > b.fitness) {
            return 1;
        } else if (a.fitness < b.fitness) {
            return -1;
        } else {
            return 0;
        }
    }

    public abstract float evaluateGenome(Genome genome);

    public class FitnessGenome {
        public float fitness;
        public Genome genome;

        public FitnessGenome(Genome genome, float fitness){
            this.genome = genome;
            this.fitness = fitness;
        }
    }


    public class FitnessGenomeComparer : Comparer<FitnessGenome> {
        public override int Compare(FitnessGenome a, FitnessGenome b){
            if (a.fitness > b.fitness) {
                return 1;
            } else if (a.fitness < b.fitness) {
                return -1;
            } else {
                return 0;
            }
        }
    }

    public class Species{
        public Genome mascot;
        public List<Genome> members;
        public List<FitnessGenome> fitnessPop;
        public float totalAdjustedFitness = 0f;

        public Species(Genome mascot) {
            this.mascot = mascot;
            members = new List<Genome>();
            members.Add(mascot);
            fitnessPop = new List<FitnessGenome>();
        }

        public void addAdjustedFitness(float adjustedFitness) {
            totalAdjustedFitness += adjustedFitness;
        }

        public void reset (Random random) {
            int newMascotIndex = random.Next(members.Count);
            mascot = members[newMascotIndex];
            members.Clear();
            fitnessPop.Clear();
            totalAdjustedFitness = 0f;
        }
    }
}