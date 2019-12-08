namespace MilkshakeCup
{
    using System.IO;
    using System.Linq;
    using MilkshakeCup.Models;

    public class CSVFileGroupsRepository : IGroupsRepository
    {
        private const char comma = ',';

        private const string csvExtension = ".csv";

        private string _folderPath;

        public CSVFileGroupsRepository(string folderPath)
        {
            _folderPath = folderPath;
        }

        public Group[] Groups()
        {
            return GroupNames().Select(Group).ToArray();
        }

        public Group Group(string groupName)
        {
            Group group = null;

            if (File.Exists(FilePath(groupName)))
            {
                group = new Group(groupName);

                foreach (var csvLine in File.ReadAllLines(FilePath(groupName)))
                {
                    group.AddRow(Row(csvLine));
                }
            }

            return group;
        }

        public void Save(Group group)
        {
            File.WriteAllLines(
                FilePath(group.Name),
                group.Rows.Select(CSV).ToArray());
        }

        /// <summary>
        /// Returns the file names of all .csv files in the _folderPath.
        /// They will be used as group names.
        /// E.g. for this structure: (_folderPath = "Groups")
        /// - Groups
        ///     - a.csv
        ///     - b.csv
        /// will return "a" and "b" ordered alphabetically.
        /// </summary>
        private IOrderedEnumerable<string> GroupNames()
        {
            const string all = "*";
            return Directory.GetFiles(_folderPath, all + csvExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(groupName => groupName);
        }

        /// <summary>
        /// Returns the FilePath for a group based on its name
        /// E.g. "a" will return "Groups/a.csv" (with _folderPath = "Groups")
        /// </summary>
        private string FilePath(string groupName)
        {
            return Path.Combine(_folderPath, groupName + csvExtension);
        }

        private static Row Row(string csvLine)
        {
            var columns = csvLine.Split(comma);

            return new Row(
                columns[0],
                columns[1],
                int.Parse(columns[2]),
                int.Parse(columns[3]),
                int.Parse(columns[4]),
                int.Parse(columns[5]),
                int.Parse(columns[6]));
        }

        private static string CSV(Row row)
        {
            return string.Join(
                comma,
                row.Player,
                row.Team,
                row.Won,
                row.Draw,
                row.Lost,
                row.GoalsInFavor,
                row.GoalsAgainst);
        }
    }
}