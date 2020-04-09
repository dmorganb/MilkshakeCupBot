namespace MilkshakeCup.Commands
{
    using System.Linq;
    using System.Threading.Tasks;

    public static class RevertCommand
    {
        public static async Task Execute(CommandContext context)
        {
            var validMatch = context.CreateMatch();

            if (validMatch.HasErrors)
            {
                await context.SendMessage(validMatch.Errors.First());
                return;
            }

            // Everything is correct at this point, go ahead and update the group.
            var match = validMatch.Value;
            var matchGroup = match.Unregister();
            context.GroupsRepository.Save(matchGroup);

            // confirmation message
            await context.SendMarkdownMessage("Anotado!\n\n" + matchGroup.AsMarkdown());

        }
    }
}