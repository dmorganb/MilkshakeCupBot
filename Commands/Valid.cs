namespace MilkshakeCup.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    public class Valid<TValue>
    {
        public TValue Value { get; private set; }

        public string[] Errors => _errors?.ToArray() ?? new string[] { };

        public bool HasErrors => Errors.Any();

        private readonly List<string> _errors;

        public Valid() 
        {
            _errors = new List<string>();
        }

        public Valid(TValue value)
            : this()
        {
            Value = value;
        }

        public void AddError(string error) => _errors.Add(error);
    }
}