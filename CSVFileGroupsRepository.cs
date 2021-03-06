﻿namespace MilkshakeCup
{
    using System;
    using System.IO;
    using System.Linq;
    using MilkshakeCup.Models;

    public class CSVFileGroupsRepository : IGroupsRepository
    {
        private const char comma = ',';

        private const string csvExtension = ".csv";

        private readonly string _folderPath;

        public CSVFileGroupsRepository(string folderPath)
        {
            _folderPath = folderPath;
        }

        public Group[] Groups() => GroupNames().Select(Group).ToArray();

        public Group Group(string groupName)
        {
            Group group = null;

            if (File.Exists(FilePath(groupName)))
            {
                group = new Group(groupName);
                Array.ForEach(File.ReadAllLines(FilePath(groupName)), csv => group.Add(Player(csv)));
            }

            return group;
        }

        public void Save(Group group) =>
            File.WriteAllLines(FilePath(group.Name), group.Players.Select(CSV).ToArray());

        /// <summary>
        /// Returns the file names of all .csv files in the _folderPath.
        /// They will be used as group names.
        /// E.g. for this structure: (_folderPath = "Groups")
        /// - Groups
        ///     - a.csv
        ///     - b.csv
        /// will return "a" and "b" ordered alphabetically.
        /// </summary>
        private IOrderedEnumerable<string> GroupNames() =>
            Directory
                .GetFiles(_folderPath, "*" + csvExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(groupName => groupName);

        /// <summary>
        /// Returns the FilePath for a group based on its name
        /// E.g. "a" will return "Groups/a.csv" (with _folderPath = "Groups")
        /// </summary>
        private string FilePath(string groupName) =>
            Path.Combine(_folderPath, groupName + csvExtension);

        private static Player Player(string csv) => Player(csv.Split(comma));

        private static Player Player(string[] columns) =>
            new Player(
                columns[0],
                columns[1],
                ushort.Parse(columns[2]),
                ushort.Parse(columns[3]),
                ushort.Parse(columns[4]),
                ushort.Parse(columns[5]),
                ushort.Parse(columns[6]));

        private static string CSV(Player player) =>
            string.Join(
                comma,
                player.Name,
                player.Team,
                player.Won,
                player.Draw,
                player.Lost,
                player.GoalsInFavor,
                player.GoalsAgainst);
    }
}