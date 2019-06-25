namespace MilkshakeCup.Model
{
    public interface IMatchCollectionRepository
    {
        MatchCollection Get(string name);

        void Save(MatchCollection matchCollection);
    }
}