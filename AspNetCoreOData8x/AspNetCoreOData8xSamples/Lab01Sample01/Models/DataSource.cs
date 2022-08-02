using System.Collections.Generic;

namespace Lab01Sample01.Models
{
    /// <summary>
    /// In memory data store.
    /// Opted for this instead of EFCore since EFCore doesn't work well with Complex Types.
    /// </summary>
    public class DataSource
    {
        private static DataSource instance = null;
        public static DataSource Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataSource();
                }
                return instance;
            }
        }
        public List<Book> Books { get; set; }
        public List<Author> Authors { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<Address> Addresses { get; set; }
        public List<Translator> Translators { get; set; }

        private DataSource()
        {
            this.Reset();
            this.Initialize();
        }
        public void Reset()
        {
            this.Books = new List<Book>();
            this.Authors = new List<Author>();
            this.Publishers = new List<Publisher>();
            this.Addresses = new List<Address>();
            this.Translators = new List<Translator>();
        }
        public void Initialize()
        {
            this.Translators.AddRange(new List<Translator>()
            {
                new Translator(){TranslatorID = "100001", TranslatorName = "Translator 1"},
                new Translator(){TranslatorID = "100002", TranslatorName = "Translator 2"},
                new Translator(){TranslatorID = "100003", TranslatorName = "Translator 3"},
                new Translator(){TranslatorID = "100004", TranslatorName = "Translator 4"},
                new Translator(){TranslatorID = "100005", TranslatorName = "Translator 5"},
                new Translator(){TranslatorID = "100006", TranslatorName = "Translator 6"},
                new Translator(){TranslatorID = "100007", TranslatorName = "Translator 7"},
            });

            this.Addresses.AddRange(new List<Address>()
            {
                new Address(){ County = "County 1", Town = "Town 1"},
                new Address(){ County = "County 2", Town = "Town 2"},
                new Address(){ County = "County 3", Town = "Town 3"},
                new Address(){ County = "County 4", Town = "Town 4"},
                new Address(){ County = "County 5", Town = "Town 5"},
            });

            this.Authors.AddRange(new List<Author>()
            {
                new Author(){ID = "10001", AuthorName = "Author 1", Addresses = new List<Address>(){Addresses[0], Addresses[1], Addresses[4]}},
                new Author(){ID = "10002", AuthorName = "Author 2", Addresses = new List<Address>(){Addresses[1], Addresses[2], Addresses[4]}},
                new Author(){ID = "10003", AuthorName = "Author 3", Addresses = new List<Address>(){Addresses[2], Addresses[3]}},
                new Author(){ID = "10004", AuthorName = "Author 4", Addresses = new List<Address>(){Addresses[3], Addresses[4]}},
                new Author(){ID = "10005", AuthorName = "Author 5", Addresses = new List<Address>(){Addresses[0], Addresses[3], Addresses[2]}},
            });

            this.Publishers.AddRange(new List<Publisher>()
            {
                new Publisher(){ID = "1001", Address = Addresses[2], PublisherName = "Publisher 1", Authors = new List<Author>(){Authors[0], Authors[1], Authors[3]}},
                new Publisher(){ID = "1002", Address = Addresses[4], PublisherName = "Publisher 2", Authors = new List<Author>(){Authors[1], Authors[2]}},
                new Publisher(){ID = "1003", Address = Addresses[0], PublisherName = "Publisher 3", Authors = new List<Author>(){Authors[3], Authors[1]}},
                new Publisher(){ID = "1004", Address = Addresses[1], PublisherName = "Publisher 4", Authors = new List<Author>(){Authors[4], Authors[2]}},
                new Publisher(){ID = "1005", Address = Addresses[3], PublisherName = "Publisher 5", Authors = new List<Author>(){Authors[0], Authors[4], Authors[2]}},
            });

            this.Books.AddRange(new List<Book>()
            {
                new Book(){ID = "1", Isbn = "AA0011", Title = "Book 1", Year = 2000, ForKids = false, Authors = new List<Author>(){Authors[0], Authors[1]}, Translators = new List<Translator>(){Translators[0], Translators[1]}},
                new Book(){ID = "2", Isbn = "BB0011", Title = "Book 2", Year = 2001, ForKids = true, Authors = new List<Author>(){Authors[4], Authors[1], Authors[2]}, Translators = new List<Translator>(){Translators[0], Translators[2]}},
                new Book(){ID = "3", Isbn = "CC0011", Title = "Book 3", Year = 2002, ForKids = false, Authors = new List<Author>(){Authors[2], Authors[3]}, Translators = new List<Translator>(){Translators[3], Translators[1]}},
                new Book(){ID = "4", Isbn = "DD0011", Title = "Book 4", Year = 2003, ForKids = true, Authors = new List<Author>(){Authors[0], Authors[1], Authors[1]}, Translators = new List<Translator>(){Translators[4], Translators[2]}},
                new Book(){ID = "5", Isbn = "EE0011", Title = "Book 5", Year = 2004, ForKids = true, Authors = new List<Author>(){Authors[0], Authors[4], Authors[3]}, Translators = new List<Translator>(){Translators[5], Translators[4]}},
                new Book(){ID = "6", Isbn = "FF0011", Title = "Book 6", Year = 2005, ForKids = true, Authors = new List<Author>(){Authors[3], Authors[1]}, Translators = new List<Translator>(){Translators[6], Translators[3]}},
                new Book(){ID = "7", Isbn = "GG0011", Title = "Book 7", Year = 2006, ForKids = true, Authors = new List<Author>(){Authors[0], Authors[1], Authors[2]}, Translators = new List<Translator>(){Translators[4], Translators[0]}},
                new Book(){ID = "8", Isbn = "HH0011", Title = "Book 8", Year = 2007, ForKids = false, Authors = new List<Author>(){Authors[3], Authors[4]}, Translators = new List<Translator>(){Translators[2], Translators[5]}},
            });
        }
    }
}
