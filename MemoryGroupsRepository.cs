using System.Collections.Generic;

namespace MilkshakeCup
{
    public class MemoryGroupsRepository : IGroupsRepository
    {
        private List<Row> _groupA;

        private List<Row> _groupB;

        private List<Row> _groupC;

        public MemoryGroupsRepository()
        {
            _groupA = new List<Row>
            {
                new Row("victor", "toluca", 0, 0, 0, 0, 0),
                new Row("cristopher", "chivas", 0, 0, 0, 0, 0),
                new Row("dennis", "monterrey", 0, 0, 0, 0, 0),
                new Row("luis", "unam", 0, 0, 0, 0, 0),
            };
            _groupB = new List<Row>()
            {
                new Row("steven", "cruzazul", 0, 0, 0, 0, 0),
                new Row("henry", "puebla", 0, 0, 0, 0, 0),
                new Row("manuel", "pachuca", 0, 0, 0, 0, 0),
                new Row("carlos", "tigres", 0, 0, 0, 0, 0),
            };
            _groupC = new List<Row>()
            {
                new Row("eduardo steiner", "santoslaguna", 0, 0, 0, 0, 0),
                new Row("sergio", "america", 0, 0, 0, 0, 0),
                new Row("daniel morgan", "veracruz", 0, 0, 0, 0, 0),
                new Row("kevin", "leon", 0, 0, 0, 0, 0),
                new Row("jasson", "morelia", 0, 0, 0, 0, 0),
            };
        }

        public List<List<Row>> Groups()
        {
            return new List<List<Row>> { _groupA, _groupB, _groupC };
        }

        public List<Row> Group(string groupName)
        {
            switch(groupName)
            {
                case "a": return _groupA;
                case "b": return _groupB;
                case "c": return _groupC;
                default: return null;
            }
        }

        public void Save(List<Row> group)
        {
            // nothing since it's memory
        }
    }
}