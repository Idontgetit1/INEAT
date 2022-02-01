using System;
using System.Collections.Generic;


public class Genome {
    public Dictionary<int, ConnectionGene> connections;
    public Dictionary<int, NodeGene> nodes;
    private Random random;
    private List<int> innovations = new List<int>();
    public InnovationGenerator innovation;

    const float PROBABILITY_PERTURB = 0.1f;
    const float C1 = 1.0f;
    const float C2 = 1.0f;
    const float C3 = 1.0f;

    public Genome() {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
        random = new Random();
        innovation = new InnovationGenerator();
    }

    public static float compatibilityDistance(Genome genome1, Genome genome2){
        // calculate the compatibility distance between two genomes
        // is calculated by (C1*E)/N + (C2*D)/N + C3*W, where E is the number of excess genes, D is the number of disjoint genes, and W is the average weight differences of matching genes
        // N is the number of genes in the larger genome (normalize to 1 if both are smaller that 20 genes) and C1, C2, C3 are constants

        // calculate the number of excess genes
        int excessGenes = 0;
        foreach (KeyValuePair<int, ConnectionGene> connection in genome2.connections) {
            if (!genome1.connections.ContainsKey(connection.Key)) {
                excessGenes++;
            }
        }

        // calculate the number of disjoint genes
        int disjointGenes = 0;
        foreach (KeyValuePair<int, ConnectionGene> connection in genome1.connections) {
            if (!genome2.connections.ContainsKey(connection.Key)) {
                disjointGenes++;
            }
        }

        // calculate the average weight differences of matching genes
        float averageWeightDifference = 0;
        int matchingGenes = 0;
        foreach (KeyValuePair<int, ConnectionGene> connection in genome1.connections) {
            if (genome2.connections.ContainsKey(connection.Key)) {
                averageWeightDifference += Math.Abs(connection.Value.weight - genome2.connections[connection.Key].weight);
                matchingGenes++;
            }
        }
        averageWeightDifference /= matchingGenes;

        // calculate N
        int N = genome1.connections.Count > genome2.connections.Count ? genome1.connections.Count : genome2.connections.Count;
        // normalize to genome size
        if (N < 20) {
            N = 1;
        }

        // calculate the compatibility distance
        float d = (C1 * excessGenes) / N + (C2 * disjointGenes) / N + (C3 * averageWeightDifference);

        Console.WriteLine("excessGenes: " + excessGenes + " disjointGenes: " + disjointGenes + " averageWeightDifference: " + averageWeightDifference + " N: " + N + " d: " + d);

        return d;
    }

    public void addConnectionMutation(int inNode, int outNode) {
        // if not already expressed
        foreach (ConnectionGene connection in connections.Values) {
            if (connection.inNode == inNode && connection.outNode == outNode) {
                if (connection.expressed){
                    return;
                }else{
                    connection.expressed = true;
                    return;
                }
            }
        }

        // create connection
        ConnectionGene newConnection = new ConnectionGene(inNode, outNode, (float)random.NextDouble() * 2 - 1, true, innovation.getInnovation());
        addConnectionGene(newConnection);
            
    }

    public void addNodeMutation(int connection) {
        // if connection is expressed
        if (connections[connection].expressed) {
            int inNode = connections[connection].inNode;
            int outNode = connections[connection].outNode;

            // disable connection
            connections[connection].expressed = false;

            // create new node
            int id = nodes.Count;
            NodeGene newNode = new NodeGene(id);
            
            // create new Connections
            ConnectionGene inToNew = new ConnectionGene(inNode, id, 1, true, innovation.getInnovation());
            ConnectionGene newToOut = new ConnectionGene(id, outNode, connections[connection].weight, true, innovation.getInnovation());

            // put node in dict
            addNodeGene(newNode);

            // put conn in dict
            addConnectionGene(inToNew);
            addConnectionGene(newToOut);
        }
    }

    public void mutation() {
        foreach (ConnectionGene connection in connections.Values) {
            if (random.NextDouble() < PROBABILITY_PERTURB) {
                connection.weight *= (float)random.NextDouble() * 2 - 1;
            }else{
                connection.weight = (float)random.NextDouble() * 2 - 1;
            }
        }
    }

    public static Genome crossover(Genome parent1, Genome parent2, Random r){
        // parent1 is stronger
        
        Genome child = new Genome();

        // copy nodes from parent1
        foreach (NodeGene node in parent1.nodes.Values) {
            child.addNodeGene(node.copy());
        }

        foreach (ConnectionGene connection in parent1.connections.Values) {
            if (parent2.connections.ContainsKey(connection.innovation)){ // gene matching
                ConnectionGene childConnection = r.NextDouble() < 0.5 ? connection.copy() : parent2.connections[connection.innovation].copy();
                child.addConnectionGene(childConnection);
            }else{ // gene disjoint or excess
                // copy from parent1
                child.addConnectionGene(connection.copy());
            }
        }

        return child;
    }

    public void addNodeGene(NodeGene nodeGene){
        nodes.Add(nodeGene.id, nodeGene);
    }

    public void addConnectionGene(ConnectionGene connectionGene){
        connections.Add(connectionGene.innovation, connectionGene);
    }
}