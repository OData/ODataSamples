using Microsoft.OData.ModelBuilder;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lab02Sample03.Models
{
    [DataContract(Name = "book")]
    public class Book
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "isbn")]
        public string Isbn { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }

        [DataMember(Name = "forKids")]
        [System.ComponentModel.DefaultValue(true)] // Setting default values in the model.
        public bool ForKids { get; set; }

        [DataMember(Name = "mainAuthor")]
        public Author MainAuthor { get; set; }

        [DataMember(Name = "authors")]
        public IList<Author> Authors { get; set; }

        [DataMember(Name = "translators")]
        [Contained]
        public IList<Translator> Translators { get; set; }
    }

    [DataContract(Name = "bookRating")]
    public class BookRating
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "rating")]
        public int Rating { get; set; }

        [DataMember(Name = "bookId")]
        public int BookID { get; set; }
    }

    [DataContract(Name = "translator")]
    public class Translator
    {
        [DataMember(Name = "translatorId")]
        public int TranslatorID { get; set; }

        [DataMember(Name = "translatorName")]
        public string TranslatorName { get; set; }
    }
}
