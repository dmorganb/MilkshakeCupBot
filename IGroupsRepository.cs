namespace MilkshakeCup
{
    using System.Collections.Generic;

    public interface IGroupsRepository 
    {
        IEnumerable<Group> Groups();

        Group Group(string groupName);

        void Save(Group group);
    }
}