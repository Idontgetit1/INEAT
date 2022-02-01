using System;


// execute and test the program
namespace INEAT
{
    class Program
    {
        
        static void Main(string[] args)
        {

            Genome genome1 = new Genome();

            genome1.addNodeGene(new NodeGene(0));
            genome1.addNodeGene(new NodeGene(1));
            genome1.addNodeGene(new NodeGene(2));
            genome1.addNodeGene(new NodeGene(3));

            genome1.addConnectionGene(new ConnectionGene(0, 1, 2, true, genome1.innovation.getInnovation()));
            genome1.addConnectionGene(new ConnectionGene(1, 2, 2, true, genome1.innovation.getInnovation()));
            genome1.addConnectionGene(new ConnectionGene(0, 3, 1, true, genome1.innovation.getInnovation()));
            genome1.addConnectionGene(new ConnectionGene(3, 2, 1, true, genome1.innovation.getInnovation()));

            

            Genome genome2 = new Genome();

            genome2.addNodeGene(new NodeGene(0));
            genome2.addNodeGene(new NodeGene(1));
            genome2.addNodeGene(new NodeGene(2));
            genome2.addNodeGene(new NodeGene(3));


            genome2.addConnectionGene(new ConnectionGene(0, 3, 3, true, genome2.innovation.getInnovation()));
            genome2.addConnectionGene(new ConnectionGene(3, 2, 3, true, genome2.innovation.getInnovation()));
            


            float comp = Genome.compatibilityDistance(genome1, genome2);

            Console.WriteLine(comp);
        }
    }
}
