namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;

    public static class NotFoundCommand
    {
        public static Task Execute(MilkshakeCupCommandContext context) => Task.CompletedTask;
    }
}