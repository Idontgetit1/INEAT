using System;


// execute and test the program
namespace INEAT
{
    class Program
    {
        
        static void Main(string[] args)
        {

            Genome genome = new Genome();

            genome.addNodeGene(new NodeGene(0));
            genome.addNodeGene(new NodeGene(1));
            genome.addNodeGene(new NodeGene(2));

            genome.addConnectionGene(new ConnectionGene(0, 2, 1, true, genome.innovation.getInnovation()));
            genome.addConnectionGene(new ConnectionGene(1, 2, 1, true, genome.innovation.getInnovation()));

            // print genome connections
            Console.WriteLine("Genome connections:");
            foreach (ConnectionGene connection in genome.connections.Values)
            {
                Console.WriteLine("inNode: " + connection.inNode + " outNode: " + connection.outNode + " weight: " + connection.weight + " expressed: " + connection.expressed + " innovation: " + connection.innovation);
            }

            genome.addNodeMutation(1);

            // print genome connections
            Console.WriteLine("Genome connections:");
            foreach (ConnectionGene connection in genome.connections.Values)
            {
                Console.WriteLine("inNode: " + connection.inNode + " outNode: " + connection.outNode + " weight: " + connection.weight + " expressed: " + connection.expressed + " innovation: " + connection.innovation);
            }
        }
    }
}
