namespace ppc.Commands
{
    public interface ICancelable : ICommand
    {
        void Undo();
    }
}
