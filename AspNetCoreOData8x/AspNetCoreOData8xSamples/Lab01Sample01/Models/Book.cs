using Microsoft.OData.ModelBuilder;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lab01Sample01.Models
{
    [DataContract]
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

    [DataContract]
    public class Translator
    {
        [DataMember(Name = "translatorId")]
        public int TranslatorID { get; set; }

        [DataMember(Name = "translatorName")]
        public string TranslatorName { get; set; }
    }
}
