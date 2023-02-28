using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Pet
    {
        public string Name { get; set; }
        public PetType PetType { get; set; }

        public override string ToString()
        {
            var petType = PetType switch
            {
                PetType.Dog => "Пёс",
                PetType.Cat => "Кот",
                PetType.Hamster => "Хомяк",
                _ => throw new InvalidOperationException()
            };
            return $"{petType} {Name}";
        }
    }

    public enum PetType
    {
        Cat,
        Dog,
        Hamster
    }
}
