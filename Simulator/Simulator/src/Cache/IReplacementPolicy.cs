
namespace LabTomasulo.Arquitetura
{
    interface IReplacementPolicy
    {
        void access(int set, int block);
        void erase(int set, int block);
        void write(int set, int block);
        int getReplacementBlock(int set);
    }
}
