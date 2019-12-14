namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;

    public static class SingleGroupCommand
    {
        public static async Task Execute(MilkshakeCupCommandContext context)
        {
            var group = context.GroupsRepository.Group(context.Parameters[1]);

            if (group != null)
            {
                await GroupsCommand.SendGroupAsMarkdownTextMessage(context, group);
            }
        }
    }
}