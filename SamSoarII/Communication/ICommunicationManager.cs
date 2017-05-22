using SamSoarII.Communication.Command;

namespace SamSoarII.Communication
{
    public interface ICommunicationManager
    {
        int Start();
        int Abort();
        int Read(ICommunicationCommand cmd);
        int Write(ICommunicationCommand cmd);
    }
}
