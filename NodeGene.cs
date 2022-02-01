using System.Collections.Generic;

public class NodeGene{
    public int id;
    List<int> inputs;
    List<int> outputs;

    public NodeGene(int id){
        this.id = id;
        inputs = new List<int>();
        outputs = new List<int>();
    }

    public NodeGene copy(){
        NodeGene copy = new NodeGene(id);
        copy.inputs = new List<int>(inputs);
        copy.outputs = new List<int>(outputs);
        return copy;
    }
}