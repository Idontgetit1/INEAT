using System;


// execute and test the program
namespace INEAT
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Counter nodeInnovation = new Counter();
            Counter connectionInnovation = new Counter();
            
            Genome genome = new Genome();
            int n1 = nodeInnovation.getInnovation();
            int n2 = nodeInnovation.getInnovation();
            int n3 = nodeInnovation.getInnovation();
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, n1));
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.INPUT, n2));
            genome.addNodeGene(new NodeGene(NodeGene.TYPE.OUTPUT, n3));

            int c1 = connectionInnovation.getInnovation();
            int c2 = connectionInnovation.getInnovation();
            genome.addConnectionGene(new ConnectionGene(n1,n3,0.5f,true,c1));
            genome.addConnectionGene(new ConnectionGene(n2,n3,0.5f,true,c2));

            Evaluator eval = new Eval(100, genome, nodeInnovation, connectionInnovation);

            for (int i = 0; i < 500; i++)
            {
                eval.evaluate();
                Console.WriteLine();
                Console.Write("Generation: " + i);
                Console.Write("\tHighest Fitness: " + eval.highestScore);
                Console.Write("\tAmount of species: " + eval.species.Count);
                Console.Write("\tHighest Connections: " + eval.fittestGenome.connections.Count);
            }
        }
    }

    class Eval : Evaluator {

        public Eval(int maxGenerations, Genome genome, Counter nodeInnovation, Counter connectionInnovation) : base(maxGenerations, genome, nodeInnovation, connectionInnovation) {
        }
        public override float evaluateGenome(Genome genome) {
            // difference to 100
            return 1000f/Math.Abs(genome.connections.Count - 100);
        }
    }
}
