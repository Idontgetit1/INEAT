using System;
using System.Collections.Generic;


public class Genome {
    List<ConnectionGene> connections;
    List<NodeGene> nodes;
    private Random random = new Random();

    public void addConnectionMutation(int inNode, int outNode) {
        // if not already expressed
        if (connections.Find(x => x.inNode == inNode && x.outNode == outNode) == null) {
            connections.Add(new ConnectionGene(inNode, outNode, random.NextDouble(), true, 1));
        }
    }

    public void addNodeMutation(int connection) {
        // if not already expressed
        if (connections[connection].expressed == true) {
            int inNode = connections[connection].inNode;
            int outNode = connections[connection].outNode;

            // add new node
            int newNode = nodes.Count;
            nodes.Add(new NodeGene(newNode));

            // add new connection
            connections.Add(new ConnectionGene(inNode, newNode, 1, true, 1));
            connections.Add(new ConnectionGene(newNode, outNode, connections[connection].weight, true, 1));

            // remove old connection
            connections[connection].expressed = false;
        }

    }
}