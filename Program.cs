using System;
using System.Collections.Generic;


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

            Eval eval = new Eval(100, genome, nodeInnovation, connectionInnovation);

            for (int i = 0; i < 100; i++)
            {
                foreach (Genome g in eval.genomes)
                {
                    g.output = Program.processNetwork(new float[]{1, 2}, g)[0];
                }
                eval.evaluate();

                
                
                

                Console.WriteLine();
                Console.Write("Generation: " + i);
                Console.Write("\tHighest Fitness: " + eval.highestScore);
                Console.Write("\tAmount of species: " + eval.species.Count);
                Console.Write("\tHighest Nodes: " + eval.fittestGenome.nodes.Count);
                Console.Write("\tHighest Output: " + eval.fittestGenome.output);
            }

            GenPrinter printer = new GenPrinter();

            printer.printGenome(eval.fittestGenome, "fittestGenome.png");
        }

        public static float[] processNetwork(float[] inputs, Genome genome)
        {
            // get genome input nodes
            List<int> inputNodes = genome.getInputNodes();

            // get genome output nodes
            List<int> outputNodes = genome.getOutputNodes();

            float[] outputs = new float[outputNodes.Count];

            Dictionary<int, float> nodeValues = new Dictionary<int, float>();

            List<int> allNodes = genome.getNodes();

            // set all nodes to 0
            foreach (int node in allNodes)
            {
                nodeValues.Add(node, 0);
            }

            // set input nodes to input values
            for (int i = 0; i < inputs.Length; i++)
            {
                nodeValues[inputNodes[i]] = inputs[i];
            }
            
            List<ConnectionGene> connections = new List<ConnectionGene>();
            List<ConnectionGene> lastConnections = new List<ConnectionGene>();
            List<ConnectionGene> lastConnectionsTemp = new List<ConnectionGene>();

            // first input nodes
            // get connections from input nodes to all other nodes
            foreach (int node in inputNodes) {
                foreach (ConnectionGene connection in genome.connections.Values) {
                    if (connection.inNode == node && connection.expressed) {
                        connections.Add(connection);
                        lastConnections.Add(connection);
                    }
                }
            }

            bool hiddenLayersLeft = true;
            while (hiddenLayersLeft) {
                hiddenLayersLeft = false;
                foreach (ConnectionGene connection in lastConnections) {
                    if (genome.getNode(connection.outNode).type == NodeGene.TYPE.HIDDEN) {
                        hiddenLayersLeft = true;
                        // get connections from node to all other nodes
                        foreach (ConnectionGene connection2 in genome.connections.Values) {
                            if (connection2.inNode == connection.outNode && connection2.expressed) {
                                connections.Add(connection2);
                                lastConnectionsTemp.Add(connection2);
                            }
                        }
                    }
                }

                lastConnections = lastConnectionsTemp;
                lastConnectionsTemp = new List<ConnectionGene>();
            }

            // calculate node values
            foreach (ConnectionGene connection in connections) {
                nodeValues[connection.outNode] += connection.weight * nodeValues[connection.inNode];
            }

            // set output nodes to node values
            foreach (int node in outputNodes) {
                outputs[outputNodes.IndexOf(node)] = nodeValues[node];
            }

            return outputs;
        }
    }

    class Eval : Evaluator {

        public Eval(int maxGenerations, Genome genome, Counter nodeInnovation, Counter connectionInnovation) : base(maxGenerations, genome, nodeInnovation, connectionInnovation) {
        }

        public override float evaluateGenome(Genome genome) {
            // difference to 100
            // Console.WriteLine("Size scores: " + scores.Count);

            return 1000f/MathF.Abs(100f - genome.output);
        }
    }

}
