namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;

    public static class SingleGroupCommand
    {
        public static async Task Execute(MilkshakeCupCommandContext context)
        {
            var group = context.GroupsRepository.Group(context.Parameters[1]);
            await GroupsCommand.SendGroupAsMarkdownTextMessage(context, group);
        }
    }
}