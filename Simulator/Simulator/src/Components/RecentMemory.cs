
namespace LabTomasulo
{
    struct RecentMemory
    {
        /* Properties */

        public readonly int Address;
        public readonly int Value;

        /* Constructor */

        public RecentMemory(int address, int value)
        {
            Address = address;
            Value = value;
        }

        /* Methods */

        public override bool Equals(object obj)
        {
            return obj is RecentMemory && Address == ((RecentMemory)obj).Address;
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }
    }
}
