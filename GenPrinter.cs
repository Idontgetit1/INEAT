using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

public class GenPrinter {
    public void printGenome(Genome genome, string path){
        Random r = new Random();
        Dictionary<int, Vector2> nodeGenePositions = new Dictionary<int, Vector2>();
        int nodeSize = 20;
        int connectionSizeBulb = 6;
        int connectionSizeLine = 2;
        int imageSize = 1024;

        int numberInputs = genome.getInputNodes().Count + 1;
        int numberOutputs = genome.getOutputNodes().Count + 1;

        // create white image with imageSize*imageSize
        Bitmap bmp = new Bitmap(imageSize, imageSize);
        Graphics g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        Image image = bmp;

        // draw nodes
        foreach (KeyValuePair<int, NodeGene> entry in genome.nodes) {
            NodeGene node = entry.Value;
            Vector2 position = new Vector2(r.Next(imageSize - 50) + 25, r.Next(imageSize));
            nodeGenePositions.Add(node.id, position);
            if (node.type == NodeGene.TYPE.INPUT) {
                // set position to the left side
                position.X = 10;
                position.Y = node.id * (imageSize / numberInputs) + (imageSize / numberInputs / 2);
                nodeGenePositions[node.id] = position;
                drawNode(image, position, nodeSize, Color.Blue);
            } else if (node.type == NodeGene.TYPE.OUTPUT) {
                // set position to the right side
                position.X = imageSize - 10;
                position.Y = (node.id - numberInputs + 1) * (imageSize / numberOutputs) + (imageSize / numberOutputs / 2);
                nodeGenePositions[node.id] = position;
                drawNode(image, position, nodeSize, Color.Red);
            } else if (node.type == NodeGene.TYPE.HIDDEN) {
                drawNode(image, position, nodeSize, Color.Green);
            }
        }

        // draw connections
        foreach (KeyValuePair<int, ConnectionGene> entry in genome.connections) {
            ConnectionGene connection = entry.Value;
            Vector2 from = nodeGenePositions[connection.inNode];
            Vector2 to = nodeGenePositions[connection.outNode];
            drawConnection(image, from, to, connectionSizeLine, Color.Black, connection.weight);
        }

        image.Save(path);
    }

    public void drawNode(Image image, Vector2 position, int size, Color color){
        Graphics g = Graphics.FromImage(image);
        g.FillEllipse(new SolidBrush(color), (int)position.X - size / 2, (int)position.Y - size / 2, size, size);
    }

    public void drawConnection(Image image, Vector2 from, Vector2 to, int size, Color color, float weight){
        Graphics g = Graphics.FromImage(image);
        g.DrawLine(new Pen(color, size), (int)from.X, (int)from.Y, (int)to.X, (int)to.Y);

        // draw weight
        Vector2 weightPosition = new Vector2((from.X + to.X) / 2, (from.Y + to.Y) / 2);
        g.DrawString(weight.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), (int)weightPosition.X, (int)weightPosition.Y);
    }

    private int countNodesByType(Genome genome, NodeGene.TYPE type){
        int count = 0;
        foreach (KeyValuePair<int, NodeGene> entry in genome.nodes) {
            NodeGene node = entry.Value;
            if (node.type == type) {
                count++;
            }
        }
        return count;
    }
}