using DefaultNamespace;

namespace Items
{
    public interface IItem
    {
        ItemType Type { get; }

        bool IsActive { get; }

        bool IsEnable { get; }

        void Active();

        void DeActive();

        void Enable();

        void Disable();

        void Execute();
    }
}