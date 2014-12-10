using System;
using System.Collections.Generic;
using System.Linq;

namespace ODataSamples.CustomFormatService
{
    public class CustomFormatContext
    {
        private static readonly CustomFormatContext instance = new CustomFormatContext();
        private List<Person> persons;

        private CustomFormatContext()
        {
            this.persons = new List<Person>();
            this.AddPerson(
                new Person
                {
                    Id = 1,
                    Card = new BusinessCard
                    {
                        N = "L1;F1",
                        FN = "LF1",
                        ORG = "Org1",
                        DynProperties =
                        {
                            {"TITLE", "Worker"},
                        }
                    }
                });

        }

        public static CustomFormatContext Instance { get { return instance; } }
        public IList<Person> People { get { return persons; } }

        public Person AddPerson(Person person)
        {
            lock (this.persons)
            {
                person.Id = this.persons.Count == 0 ? 0 : this.persons.Max(p => p.Id) + 1;
                person.UpdateTime = DateTimeOffset.Now;
                this.persons.Add(person);
            }

            return person;
        }
    }
}