namespace MilkshakeCup
{
    using System.Collections.Generic;

    public interface IGroupsRepository 
    {
        List<List<Row>> Groups();

        List<Row> Group(string groupName);

        void Save(List<Row> group);
    }
}