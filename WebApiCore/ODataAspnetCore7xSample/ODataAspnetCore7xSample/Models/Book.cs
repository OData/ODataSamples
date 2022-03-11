using Microsoft.AspNet.OData.Builder;
using System.Collections.Generic;

namespace ODataAspnetCore7xSample.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        [System.ComponentModel.DefaultValue(true)] // Setting default values in the model.
        public bool ForKids { get; set; }
        public Author MainAuthor { get; set; }
        public IList<Author> Authors { get; set; }
        [Contained]
        public IList<Translator> Translators { get; set; }
    }

    public class BookRating
    {
        public int ID { get; set; }
        public int Rating { get; set; }
        public int BookID { get; set; }
    }

    public class Translator
    {
        public int TranslatorID { get; set; }
        public string TranslatorName { get; set; }
    }
}
