using System;
using System.Collections.Generic;


public class Genome {
    public Dictionary<int, ConnectionGene> connections;
    public Dictionary<int, NodeGene> nodes;
    private List<int> innovations = new List<int>();

    const float PROBABILITY_PERTURB = 0.9f;
    const float C1 = 1.0f;
    const float C2 = 1.0f;
    const float C3 = 1.0f;

    public Genome() {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
    }

    public Genome(Genome toBeCopied) {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
        foreach (KeyValuePair<int, ConnectionGene> entry in toBeCopied.connections) {
            connections.Add(entry.Key, entry.Value.copy());
        }
        foreach (KeyValuePair<int, NodeGene> entry in toBeCopied.nodes) {
            nodes.Add(entry.Key, entry.Value.copy());
        }
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

    public void addConnectionMutation(Counter innovation, int maxAttempts, Random r) {
        int tries = 0;
        Boolean success = false;
        while (tries < maxAttempts && success == false) {
            tries++;

            int[] nodeInnovationNumbers = new int[nodes.Keys.Count];
            
            int keyNode1 = nodeInnovationNumbers[r.Next(nodeInnovationNumbers.Length)];
            int keyNode2 = nodeInnovationNumbers[r.Next(nodeInnovationNumbers.Length)];

            NodeGene node1 = nodes[keyNode1];
            NodeGene node2 = nodes[keyNode2];
            float weight = (float)r.NextDouble() *2f - 1f;

            bool reversed = false;
            if (node1.type == NodeGene.TYPE.HIDDEN && node2.type == NodeGene.TYPE.INPUT){
                reversed = true;
            }else if (node1.type == NodeGene.TYPE.OUTPUT && node2.type == NodeGene.TYPE.HIDDEN){
                reversed = true;
            }else if (node1.type == NodeGene.TYPE.OUTPUT && node2.type == NodeGene.TYPE.INPUT){
                reversed = true;
            }

            bool connectionImpossible = false;
            if (node1.type == NodeGene.TYPE.INPUT && node2.type == NodeGene.TYPE.INPUT){
                connectionImpossible = true;
            } else if (node1.type == NodeGene.TYPE.OUTPUT && node2.type == NodeGene.TYPE.OUTPUT) {
                connectionImpossible = true;
            }

            bool connectionExists = false;
            foreach (ConnectionGene con in connections.Values) {
                if (con.inNode == node1.id && con.outNode == node2.id) {
                    connectionExists = true;
                    break;
                } else if (con.inNode == node2.id && con.outNode == node1.id) {
                    connectionExists = true;
                    break;
                }
            }

            if (connectionExists || connectionImpossible) {
                continue;
            }

            ConnectionGene newCon = new ConnectionGene(reversed ? node2.id : node1.id, reversed ? node1.id : node2.id, weight, true, innovation.getInnovation());
            connections.Add(newCon.innovation, newCon);
            success = true;
        }

        if (success == false) {
            Console.WriteLine("addConnectionMutation failed");
        }
            
    }

    public void addNodeMutation(Counter connectionInnovation, Counter nodeInnovation, Random r) {
        ConnectionGene con = connections[innovations[r.Next(innovations.Count)]];

        NodeGene inNode = nodes[con.inNode];
        NodeGene outNode = nodes[con.outNode];

        con.expressed = false;

        NodeGene newNode = new NodeGene(NodeGene.TYPE.HIDDEN, nodeInnovation.getInnovation());
        ConnectionGene inToNew = new ConnectionGene(inNode.id, newNode.id, 1f, true, connectionInnovation.getInnovation());
        ConnectionGene newToOut = new ConnectionGene(newNode.id, outNode.id, con.weight, true, connectionInnovation.getInnovation());

        nodes.Add(newNode.id, newNode);
        connections.Add(inToNew.innovation, inToNew);
        connections.Add(newToOut.innovation, newToOut);
    }

    public void mutation(Random r) {
        foreach (ConnectionGene connection in connections.Values) {
            if (r.NextDouble() < PROBABILITY_PERTURB) {
                connection.weight *= (float)r.NextDouble() * 4 - 2;
            }else{
                connection.weight = (float)r.NextDouble() * 4 - 2;
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