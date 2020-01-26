namespace MilkshakeCup.Models
{
    public interface IGroupsRepository
    {
        Group[] Groups();

        Group Group(string groupName);

        void Save(Group group);
    }
}