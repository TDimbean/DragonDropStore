using DragonDrop.Infrastructure.DTOs;


namespace DragonDrop.DAL.Implementation.Resources
{
    public interface IResourceRepo
    {
        void Reset();
        void Nuke();
        void Add(string name);
        void Remove(int id);
        void Update(DefaultValueDto item, bool isSwap = false);
    }
}
