public class InterfaceComponent
{
    public abstract class Encounter
    {
        public Node Source { get; }
        public Node Destination { get; }
        public abstract void IsNear();
        public abstract void CloseEncounter();

        public Encounter(Node source, Node destination)
        {
            Source = source;
            Destination = destination;
        }
    }

    public interface INodeState
    {
        void SendBundle(Encounter encounter, Package b);
        void RcvBundle(Package b);
    }

    public interface INode
    {
        Encounter EstablishConnection(Node listener);
        void ShowInfo();
    }

}
