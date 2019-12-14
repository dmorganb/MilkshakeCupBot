namespace MilkshakeCup.Models
{
    using System.Collections.Generic;

    public interface IGroupsRepository
    {
        Group[] Groups();

        Group Group(string groupName);

        void Save(Group group);
    }
}