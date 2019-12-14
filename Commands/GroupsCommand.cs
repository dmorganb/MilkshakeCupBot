namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;
    using MilkshakeCup.Models;

    public static class GroupsCommand
    {
        public static async Task Execute(CommandContext context)
        {
            foreach (var group in context.GroupsRepository.Groups())
            {
                await context.SendMarkdownMessage(group.AsMarkdown());
            }
        }

        public static string AsMarkdown(this Group group)
        {
            if (group == null)
            {
                return "No encontré ese grupo";
            }

            var text = "Group " + group.Name + "\n";
            text += "```\n";
            text += "Equ|Pt|PJ| G| E| P|GF|Dif\n";
            text += "=========================\n";

            foreach (var player in group.Players)
            {
                text += (player.Name.Length > 3 ? player.Name.Substring(0, 3)
                            : player.Name.PadRight(3)) + "|";
                text += $"{player.Points}".PadLeft(2) + "|";
                text += $"{player.TotalGames}".PadLeft(2) + "|";
                text += $"{player.Won}".PadLeft(2) + "|";
                text += $"{player.Draw}".PadLeft(2) + "|";
                text += $"{player.Lost}".PadLeft(2) + "|";
                text += $"{player.GoalsInFavor}".PadLeft(2) + "|";
                text += $"{player.GoalDifference}".PadLeft(3) + "\n";
            }

            text += "```";

            return text;
        }
    }
}