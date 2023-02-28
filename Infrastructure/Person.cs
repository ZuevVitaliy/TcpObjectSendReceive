namespace Infrastructure
{
    public class Person
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public override string ToString()
        {
            return $"{Name} родился {BirthDate:g}";
        }
    }
}