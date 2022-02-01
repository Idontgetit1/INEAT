using System.Collections.Generic;

public class NodeGene{
    public int id;

    public enum TYPE {
        INPUT,
        HIDDEN,
        OUTPUT
    }

    public TYPE type;

    public NodeGene(TYPE type, int id){
        this.id = id;
        this.type = type;
    }

    public NodeGene copy(){
        NodeGene copy = new NodeGene(type, id);
        return copy;
    }
}