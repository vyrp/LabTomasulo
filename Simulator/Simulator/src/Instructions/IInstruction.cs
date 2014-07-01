
namespace LabTomasulo
{
    interface IInstruction
    {
        IInstruction Clone();
        bool TryEmit();
        bool TryExecute();
        bool TryWrite();
    }
}
