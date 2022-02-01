using System;
using System.Collections.Generic;


public class Genome {
    public Dictionary<int, ConnectionGene> connections;
    public Dictionary<int, NodeGene> nodes;
    private Random random;
    private int innovation = 0;
    private List<int> innovations = new List<int>();

    public Genome() {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
        random = new Random();
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
        innovation++;
        ConnectionGene newConnection = new ConnectionGene(inNode, outNode, (float)random.NextDouble() * 2 - 1, true, innovation);
        connections.Add(innovation, newConnection);
            
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
            innovation++;
            ConnectionGene inToNew = new ConnectionGene(inNode, id, 1, true, innovation);
            innovation++;
            ConnectionGene newToOut = new ConnectionGene(id, outNode, connections[connection].weight, true, innovation);

            // put node in dict
            nodes.Add(id, newNode);

            // put conn in dict
            connections.Add(innovation, inToNew);
            connections.Add(innovation, newToOut);
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