namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;

    public static class SingleGroupCommand
    {
        public static async Task Execute(CommandContext context)
        {
            var groupName = context.Parameters.Length == 2 ? context.Parameters[1] : "";
            var group = context.GroupsRepository.Group(groupName);
            await context.SendMarkdownMessage(group.AsMarkdown());
        }
    }
}