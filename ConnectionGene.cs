public class ConnectionGene {
    public int inNode;
    public int outNode;
    public float weight;
    public bool expressed;
    public int innovation;

    public ConnectionGene(int inNode, int outNode, float weight, bool expressed, int innovation) {
        this.inNode = inNode;
        this.outNode = outNode;
        this.weight = weight;
        this.expressed = expressed;
        this.innovation = innovation;
    }

    public ConnectionGene copy() {
        return new ConnectionGene(inNode, outNode, weight, expressed, innovation);
    }
}