using System;


// execute and test the program
namespace INEAT
{
    class Program
    {
        static void Main(string[] args)
        {
            Genome parent1 = new Genome();
            
            // add 5 node genes
            parent1.addNodeGene(new NodeGene(1));
            parent1.addNodeGene(new NodeGene(2));
            parent1.addNodeGene(new NodeGene(3));
            parent1.addNodeGene(new NodeGene(4));
            parent1.addNodeGene(new NodeGene(5));

            parent1.addConnectionGene(new ConnectionGene(1, 4, 1, true, 1));
            parent1.addConnectionGene(new ConnectionGene(2, 4, 1, false, 2));
            parent1.addConnectionGene(new ConnectionGene(3, 4, 1, true, 3));
            parent1.addConnectionGene(new ConnectionGene(2, 5, 1, true, 4));
            parent1.addConnectionGene(new ConnectionGene(5, 4, 1, true, 5));
            parent1.addConnectionGene(new ConnectionGene(1, 5, 1, true, 8));


            Genome parent2 = new Genome();

            // add 6 node genes
            parent2.addNodeGene(new NodeGene(1));
            parent2.addNodeGene(new NodeGene(2));
            parent2.addNodeGene(new NodeGene(3));
            parent2.addNodeGene(new NodeGene(4));
            parent2.addNodeGene(new NodeGene(5));
            parent2.addNodeGene(new NodeGene(6));

            parent2.addConnectionGene(new ConnectionGene(1, 4, 1, true, 1));
            parent2.addConnectionGene(new ConnectionGene(2, 4, 1, false, 2));
            parent2.addConnectionGene(new ConnectionGene(3, 4, 1, true, 3));
            parent2.addConnectionGene(new ConnectionGene(2, 5, 1, true, 4));
            parent2.addConnectionGene(new ConnectionGene(5, 4, 1, false, 5));
            parent2.addConnectionGene(new ConnectionGene(5, 6, 1, true, 6));
            parent2.addConnectionGene(new ConnectionGene(6, 4, 1, true, 7));
            parent2.addConnectionGene(new ConnectionGene(3, 5, 1, true, 9));
            parent2.addConnectionGene(new ConnectionGene(1, 6, 1, true, 10));


            // print parent1 connections
            Console.WriteLine("Parent 1 connections:");
            foreach (ConnectionGene connection in parent1.connections.Values)
            {
                Console.WriteLine("inNode: " + connection.inNode + " outNode: " + connection.outNode + " weight: " + connection.weight + " expressed: " + connection.expressed + " innovation: " + connection.innovation);
            }

            // print parent2 connections
            Console.WriteLine("Parent 2 connections:");
            foreach (ConnectionGene connection in parent2.connections.Values)
            {
                Console.WriteLine("inNode: " + connection.inNode + " outNode: " + connection.outNode + " weight: " + connection.weight + " expressed: " + connection.expressed + " innovation: " + connection.innovation);
            }

            // crossover
            Genome child = Genome.crossover(parent2, parent1, new Random());

            // print child connections
            Console.WriteLine("Child connections:");
            foreach (ConnectionGene connection in child.connections.Values)
            {
                Console.WriteLine("inNode: " + connection.inNode + " outNode: " + connection.outNode + " weight: " + connection.weight + " expressed: " + connection.expressed + " innovation: " + connection.innovation);
            }
        }
    }
}
