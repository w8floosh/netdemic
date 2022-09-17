using static BundleComponent;
using static NodeComponent;
public class InterfaceComponent
{
    public interface IEncounter
    {
        INode Source { get; }
        INode Destination { get; }
        int TimeElapsed { get; }

        void CheckDistance();
        void CloseEncounter();
    }

    public interface INodeState
    {
        void SendBundle(IEncounter encounter);
        void RcvBundle(IEncounter e, Bundle b);
    }

    public interface INode
    {
        IEncounter EstablishConnection();
        void showInfo();

        byte[] GeneratePhysicalAddress();
    }

}
