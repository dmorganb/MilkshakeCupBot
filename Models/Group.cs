namespace MilkshakeCup.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Group
    {
        public string Name { get; }

        // Returns the rows ordered by the rules
        public IOrderedEnumerable<Row> Rows => _rows
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsInFavor);

        private List<Row> _rows;

        public Group(string name)
        {
            Name = name;
            _rows = new List<Row>();
        }

        public void AddRow(Row row)
        {
            _rows.Add(row);
        }

        public Row Row(string rowHint)
        {
            return _rows.FirstOrDefault(x => 
                x.Team.StartsWith(rowHint) || 
                x.Player.Contains(rowHint));
        }

        public bool Has(Row row)
        {
            return _rows.Contains(row);
        }
    }
}