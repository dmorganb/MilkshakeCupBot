using System.Collections.Generic;
using System.Linq;

namespace MilkshakeCup
{
    public class MemoryGroupsRepository : IGroupsRepository
    {
        private Group _groupA;

        private Group _groupB;

        private Group _groupC;

        public MemoryGroupsRepository()
        {
            _groupA = new Group("a");
            _groupA.AddRow(new Row("victor", "toluca", 0, 0, 0, 0, 0));
            _groupA.AddRow(new Row("cristopher", "chivas", 0, 0, 0, 0, 0));
            _groupA.AddRow(new Row("dennis", "monterrey", 0, 0, 0, 0, 0));
            _groupA.AddRow(new Row("luis", "unam", 0, 0, 0, 0, 0));
            
            _groupB = new Group("b");
            _groupB.AddRow(new Row("steven", "cruzazul", 0, 0, 0, 0, 0));
            _groupB.AddRow(new Row("henry", "puebla", 0, 0, 0, 0, 0));
            _groupB.AddRow(new Row("manuel", "pachuca", 0, 0, 0, 0, 0));
            _groupB.AddRow(new Row("carlos", "tigres", 0, 0, 0, 0, 0));

            _groupC = new Group("c");
            _groupC.AddRow(new Row("eduardo steiner", "santoslaguna", 0, 0, 0, 0, 0));
            _groupC.AddRow(new Row("sergio", "america", 0, 0, 0, 0, 0));
            _groupC.AddRow(new Row("daniel morgan", "veracruz", 0, 0, 0, 0, 0));
            _groupC.AddRow(new Row("kevin", "leon", 0, 0, 0, 0, 0));
            _groupC.AddRow(new Row("jasson", "morelia", 0, 0, 0, 0, 0));
        }

        public IEnumerable<Group> Groups()
        {
            return new[] { _groupA, _groupB, _groupC };
        }

        public Group Group(string groupName)
        {
            return Groups().FirstOrDefault(group => group.Name == groupName);
        }

        public void Save(Group group)
        {
            // nothing since it's memory
        }
    }
}