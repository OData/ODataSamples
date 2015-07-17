using ODataSamples.WebApiService.Models.Enum;

namespace ODataSamples.WebApiService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using ODataSamples.WebApiService.Models;

    public class TripPinSvcDataSource
    {
        private static TripPinSvcDataSource instance = null;
        public static TripPinSvcDataSource Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TripPinSvcDataSource();
                }
                return instance;
            }
        }

        public List<Person> People { get; private set; }
        public List<Airport> Airports { get; private set; }
        public List<Airline> Airlines { get; private set; }
        public Person Me { get; set; }

        public List<Customer> Customers { get; private set; }
        public List<Order> Orders { get; private set; }

        private TripPinSvcDataSource()
        {
            this.Reset();
            this.Initialize();
        }

        public void Reset()
        {
            this.People = new List<Person>();
            this.Airports = new List<Airport>();
            this.Airlines = new List<Airline>();
            this.Customers = new List<Customer>();
            this.Orders = new List<Order>();
        }

        public void Initialize()
        {
            InitializeCustomerOrder();

            this.Airports.AddRange(new List<Airport>()
            {
                new Airport()
                {
                    Name = "San Francisco International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "South McDonnell Road, San Francisco, CA 94128",
                        City = new City()
                        {
                            Name = "San Francisco",
                            CountryRegion = "United States",
                            Region = "California"    
                        },
                        Loc = GeographyPoint.Create(37.6188888888889, -122.374722222222)
                    },
                    IataCode = "SFO",
                    IcaoCode = "KSFO"
                },
                new Airport()
                {
                    Name = "Los Angeles International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "1 World Way, Los Angeles, CA, 90045",
                        City = new City()
                        {
                            Name = "Los Angeles",
                            CountryRegion = "United States", 
                            Region = "California"
                        },
                        Loc = GeographyPoint.Create(33.9425, -118.408055555556)
                    },
                    IataCode = "LAX",
                    IcaoCode = "KLAX"
                },
                new Airport()
                {
                    Name = "Shanghai Hongqiao International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Hongqiao Road 2550, Changning District",
                        City = new City()
                        {
                            Name = "Shanghai",
                            CountryRegion = "China",
                            Region = "Shanghai"
                        },
                        Loc = GeographyPoint.Create(31.1977777777778, 121.336111111111)
                    },
                    IataCode = "SHA",
                    IcaoCode = "ZSSS"
                },
                new Airport()
                {
                    Name = "Beijing Capital International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Airport Road, Chaoyang District, Beijing, 100621",
                        City = new City()
                        {
                            Name = "Beijing",
                            CountryRegion = "China",
                            Region = "Beijing"
                        },
                        Loc = GeographyPoint.Create(40.08, 116.584444444444)
                    },
                    IataCode = "PEK",
                    IcaoCode = "ZBAA"
                },
                new Airport()
                {
                    Name = "John F. Kennedy International Airport",
                    Location = new AirportLocation()
                    {
                        Address = "Jamaica, New York, NY 11430",
                        City = new City()
                        {
                            Name = "New York City",
                            CountryRegion = "United States",
                            Region = "New York"
                        },
                        Loc = GeographyPoint.Create(40.6397222222222, -73.7788888888889)
                    },
                    IataCode = "JFK",
                    IcaoCode = "KJFK"
                }
            });

            this.Airlines.AddRange(new List<Airline>()
            {
                new Airline()
                {
                    Name = "American Airlines",
                    AirlineCode = "AA" 
                },

                new Airline()
                {
                    Name = "Shanghai Airline",
                    AirlineCode = "FM"
                },

                new Airline()
                {
                    Name = "China Eastern Airlines",
                    AirlineCode = "MU"
                }
            });

            this.Me = new Person()
            {
                FirstName = "April",
                LastName = "Cline",
                UserName = "aprilcline",
                Gender = PersonGender.Female,
                Emails = new List<string> { "April@example.com", "April@contoso.com" },
                AddressInfo = new List<Location>
                {
                    new Location()
                    {
                        Address = "P.O. Box 555",
                        City = new City()
                        {
                            CountryRegion = "United States",
                            Name = "Lander",
                            Region = "WY"
                        }
                    }
                },
                Trips = 
                {
                    new Trip()
                    {
                        TripId = 1,
                        ShareId = new Guid("9d9b2fa0-efbf-490e-a5e3-bac8f7d47354"),
                        Name = "Trip in US",
                        Budget = 1000.0f,
                        Description = "Trip in US",
                        Tags = new List<string>
                        {
                            "business",
                            "US"
                        },
                        StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                        EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4)),
                        PlanItems =
                        {
                            new Flight()
                            {
                                PlanItemId = 11,
                                ConfirmationCode = "JH58493",
                                FlightNumber = "VA1930",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 8, 0, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 9, 20, 0)),
                                Airline = Airlines[0],
                                From = Airports[0],
                                To = Airports[1]
                            },
                            new Event()
                            {
                                PlanItemId = 12,
                                Description = "Client Meeting",
                                ConfirmationCode = "4372899DD",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 13, 0, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 16, 0, 0)),
                                Duration = new TimeSpan(3, 0, 0),
                                OccursAt = new EventLocation() 
                                {
                                    Address = "100 Church Street, 8th Floor, Manhattan, 10007",
                                    BuildingInfo = "Regus Business Center", 
                                    City = new City() 
                                    {
                                        Name = "New York City", 
                                        CountryRegion = "United States", 
                                        Region = "New York" 
                                    }
                                }
                            }
                        }
                    },
                    new Trip()
                    {
                        TripId = 2,
                        Name = "Trip in Beijing",
                        Budget = 3000.0f,
                        ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                        Description = "Trip from Shanghai to Beijing",
                        Tags = new List<string>{"Travel", "Beijing"},
                        StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                        EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4)),
                        PlanItems =
                        {
                            new Flight()
                            {
                                PlanItemId = 21,
                                ConfirmationCode = "JH58494",
                                FlightNumber = "FM1930",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 8, 0, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 9, 20, 0)),
                                Airline = Airlines[1],
                                SeatNumber = "B11",
                                From = Airports[2],
                                To = Airports[3]
                            },
                            new Flight()
                            {
                                PlanItemId = 32,
                                ConfirmationCode = "JH58495",
                                FlightNumber = "MU1930",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 15, 00, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                Airline = Airlines[2],
                                SeatNumber = "A32",
                                From = Airports[3],
                                To = Airports[2]
                            },
                            new Event()
                            {
                                PlanItemId = 5,
                                Description = "Dinner",
                                StartsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 18, 0, 0)),
                                EndsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 21, 0, 0)),
                                Duration = new TimeSpan(3, 0, 0),
                                OccursAt = new EventLocation()
                                {
                                    Address = "10 Beijing Street, 100000",
                                    City = new City(){
                                        Name = "Beijing",
                                        CountryRegion = "China",
                                        Region = "Beijing"
                                    },
                                    BuildingInfo = "Beijing Restaurant"
                                }
                            }
                        }
                    },
                    new Trip()
                    {
                        TripId = 3,
                        ShareId = new Guid("9ce142c3-5fd6-4a71-848e-5220ebf1e9f3"),
                        Name = "Honeymoon",
                        Budget = 800.0f,
                        Description = "Happy honeymoon trip",
                        Tags = new List<string>{"Travel", "honeymoon"},
                        StartsAt = new DateTime(2014, 2, 1),
                        EndsAt = new DateTime(2014, 2, 4)
                    },
                    new Trip()
                    {
                        TripId = 4,
                        ShareId = new Guid("4CCFB043-C79C-44EF-8CFE-CD493CED6654"),
                        Name = "Business trip to OData",
                        Budget = 324.6f,
                        Description = "Business trip to OData",
                        Tags = new List<string>{"business", "odata"},
                        StartsAt = new DateTime(2013, 1, 1),
                        EndsAt = new DateTime(2013, 1, 4)
                    },
                    new Trip()
                    {
                        TripId = 5,
                        ShareId = new Guid("4546F419-0070-45F7-BA2C-19E4BC3647E1"),
                        Name = "Travel trip in US",
                        Budget = 1250.0f,
                        Description = "Travel trip in US",
                        Tags = new List<string>{"travel", "overseas"},
                        StartsAt = new DateTime(2013, 1, 19),
                        EndsAt = new DateTime(2013, 1, 28)
                    },
                    new Trip()
                    {
                        TripId = 6,
                        ShareId = new Guid("26F0E8F6-657A-4561-BF3B-719366EF04FA"),
                        Name = "Study music in Europe",
                        Budget = 3200.0f,
                        Description = "Study music in Europe",
                        Tags = new List<string>{"study", "overseas"},
                        StartsAt = new DateTime(2013, 3, 1),
                        EndsAt = new DateTime(2013, 5, 4)
                    },
                    new Trip()
                    {
                        TripId = 7,
                        ShareId = new Guid("2E77BF06-A354-454B-8BCA-5F004C1AFB59"),
                        Name = "Conference talk about OData",
                        Budget = 2120.55f,
                        Description = "Conference talk about ODatan",
                        Tags = new List<string>{"odata", "overseas"},
                        StartsAt = new DateTime(2013, 7, 2),
                        EndsAt = new DateTime(2013, 7, 5)
                    },
                    new Trip()
                    {
                        TripId = 8,
                        ShareId = new Guid("E6E23FB2-C428-439E-BDAB-9283482F49F0"),
                        Name = "Vocation at hometown",
                        Budget = 1500.0f,
                        Description = "Vocation at hometown",
                        Tags = new List<string>{"voaction"},
                        StartsAt = new DateTime(2013, 10, 1),
                        EndsAt = new DateTime(2013, 10, 5)
                    },
                    new Trip()
                    {
                        TripId = 9,
                        ShareId = new Guid("FAE31279-35CE-4119-9BDC-53F6E19DD1C5"),
                        Name = "Business trip for tech training",
                        Budget = 100.0f,
                        Description = "Business trip for tech training",
                        Tags = new List<string>{"business"},
                        StartsAt = new DateTime(2013, 9, 1),
                        EndsAt = new DateTime(2013, 9, 4)
                    }
                }
            };

            this.People.AddRange(new List<Person>()
            {
                new Person()
                {
                    FirstName = "Russell",
                    LastName = "Whyte",
                    UserName = "russellwhyte",
                    Gender = PersonGender.Male,
                    Emails = new List<string> { "Russell@example.com", "Russell@contoso.com" },
                    AddressInfo = 
                    {
                      new Location()
                      {
                          Address = "187 Suffolk Ln.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Boise",
                              Region = "ID"
                          }
                      }
                    },
                    Trips =
                    {                        
                        new Trip()
                        {
                            TripId = 001001,
                            ShareId = new Guid("9d9b2fa0-efbf-490e-a5e3-bac8f7d47354"),
                            Name = "Trip in US",
                            Budget = 3000.0f,
                            Description = "Trip from San Francisco to New York City",
                            Tags = new List<string>
                            {
                                "business",
                                "New York meeting"
                            },
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4)),
                            PlanItems =
                            {
                                new Flight()
                                {
                                    PlanItemId = 11,
                                    ConfirmationCode = "JH58493",
                                    FlightNumber = "VA1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 8, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 9, 20, 0)),
                                    Airline = Airlines[0],
                                    From = Airports[0],
                                    To = Airports[4]
                                },
                                new Event()
                                {
                                    PlanItemId = 12,
                                    Description = "Client Meeting",
                                    ConfirmationCode = "4372899DD",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 13, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 1, 6, 13, 0, 0)),
                                    Duration = new TimeSpan(3, 0, 0),
                                    OccursAt = new EventLocation() 
                                    {
                                        BuildingInfo = "Regus Business Center", 
                                        City = new City() 
                                        {
                                            Name = "New York City", 
                                            CountryRegion = "United States", 
                                            Region = "New York" 
                                        }, 
                                        Address = "100 Church Street, 8th Floor, Manhattan, 10007"
                                    }
                                },
                                new Flight()
                                {
                                    PlanItemId = 13,
                                    ConfirmationCode = "JH58493",
                                    FlightNumber = "VA1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 13, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 14, 20, 0)),
                                    Airline = Airlines[0],
                                    From = Airports[4],
                                    To = Airports[0]
                                },
                            }
                        },
                        new Trip()
                        {
                            TripId = 001003,
                            Name = "Trip in Beijing",
                            Budget = 2000.0f,
                            ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                            Description = "Trip from Shanghai to Beijing",
                            Tags = new List<string>{"Travel", "Beijing"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4)),
                            PlanItems = 
                            {
                                new Flight()
                                {
                                    PlanItemId = 21,
                                    ConfirmationCode = "JH58494",
                                    FlightNumber = "FM1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 8, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 9, 20, 0)),
                                    Airline = Airlines[1],
                                    SeatNumber = "B11",
                                    From = Airports[2],
                                    To = Airports[3]
                                },
                                new Flight()
                                {
                                    PlanItemId = 32,
                                    ConfirmationCode = "JH58495",
                                    FlightNumber = "MU1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 15, 30, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                    Airline = Airlines[2],
                                    SeatNumber = "A32",
                                    From = Airports[3],
                                    To = Airports[2]
                                },
                                new Event()
                                {
                                    PlanItemId = 5,
                                    Description = "Dinner",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 18, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 21, 0, 0)),
                                    Duration = new TimeSpan(3, 0, 0),
                                    OccursAt = new EventLocation()
                                    {
                                        BuildingInfo = "Beijing Restaurant",
                                        City = new City()
                                        {
                                            Name = "Beijing",
                                            CountryRegion = "China",
                                            Region = "Beijing"
                                        },
                                        Address = "10 Beijing Street, 100000"
                                    }
                                }
                            }
                        },
                        new Trip()
                        {
                            TripId = 001007,
                            ShareId = new Guid("9ce142c3-5fd6-4a71-848e-5220ebf1e9f3"),
                            Name = "Honeymoon",
                            Budget = 2650.0f,
                            Description = "Happy honeymoon trip",
                            Tags = new List<string>{"Travel", "honeymoon"},
                            StartsAt = new DateTime(2014, 2, 1),
                            EndsAt = new DateTime(2014, 2, 4)
                        }
                    }
                },            
                new Person()
                {
                    FirstName = "Scott",
                    LastName = "Ketchum",
                    UserName = "scottketchum",
                    Gender = PersonGender.Male,
                    Emails = new List<string> { "Scott@example.com" },
                    AddressInfo = new List<Location>
                    {
                      new Location()
                      {
                          Address = "2817 Milton Dr.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Albuquerque",
                              Region = "NM"
                          }
                      }
                    },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 002002,
                            ShareId = new Guid("9d9b2fa0-efbf-490e-a5e3-bac8f7d47354"),
                            Name = "Trip in US",
                            Budget = 5000.0f,
                            Description = "Trip from San Francisco to New York City",
                            Tags = new List<string>{"business","New York meeting"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4)),
                            PlanItems =
                            {
                                new Flight()
                                {
                                    PlanItemId = 11,
                                    ConfirmationCode = "JH58493",
                                    FlightNumber = "VA1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 8, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 1, 1, 9, 20, 0)),
                                    Airline = Airlines[0],
                                    SeatNumber = "A12",
                                    From = Airports[0],
                                    To = Airports[4]
                                },
                                new Event()
                                {
                                    PlanItemId = 12,
                                    Description = "Client Meeting",
                                    ConfirmationCode = "4372899DD",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 13, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 1, 2, 16, 0, 0)),
                                    Duration = new TimeSpan(3, 0, 0),
                                    OccursAt = new EventLocation()
                                    {
                                        BuildingInfo = "Regus Business Center",
                                        City = new City()
                                        {
                                            Name = "New York City",
                                            CountryRegion = "United States",
                                            Region = "New York"
                                        },
                                        Address = "100 Church Street, 8th Floor, Manhattan, 10007"
                                    }
                                },
                                new Flight()
                                {
                                    PlanItemId = 13,
                                    ConfirmationCode = "JH58493",
                                    FlightNumber = "VA1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 13, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 1, 4, 14, 20, 0)),
                                    Airline = Airlines[0],
                                    From = Airports[4],
                                    To = Airports[0]
                                }
                            }
                        },
                        new Trip()
                        {
                            TripId = 002004,
                            ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                            Name = "Trip in Beijing",
                            Budget = 11000.0f,
                            Description = "Trip from Shanghai to Beijing",
                            Tags = new List<string>{"Travel", "Beijing"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4)),
                            PlanItems = 
                            {
                                new Flight()
                                {
                                    PlanItemId = 21,
                                    ConfirmationCode = "JH58494",
                                    FlightNumber = "FM1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 8, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 1, 9, 20, 0)),
                                    Airline = Airlines[1],
                                    SeatNumber = "B12",
                                    From = Airports[2],
                                    To = Airports[3]
                                },
                                new Flight()
                                {
                                    PlanItemId = 32,
                                    ConfirmationCode = "JH58495",
                                    FlightNumber = "MU1930",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 10, 16, 30, 0)),
                                    Airline = Airlines[2],
                                    SeatNumber = "A33",
                                    From = Airports[3],
                                    To = Airports[2]
                                },
                                new Event()
                                {
                                    PlanItemId = 5,
                                    Description = "Dinner",
                                    StartsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 18, 0, 0)),
                                    EndsAt = new DateTimeOffset(new DateTime(2014, 2, 2, 21, 0, 0)),
                                    Duration = new TimeSpan(3, 0, 0),
                                    OccursAt = new EventLocation()
                                    {
                                        BuildingInfo = "Beijing Restaurant",
                                        City = new City()
                                        {
                                            Name = "Beijing",
                                            CountryRegion = "China",
                                            Region = "Beijing"
                                        },
                                        Address = "10 Beijing Street, 100000"
                                    }
                                }
                            }
                        }
                    }
                },            
                new Person()
                {
                    FirstName = "Ronald",
                    LastName = "Mundy",
                    UserName = "ronaldmundy",
                    Gender = PersonGender.Male,
                    Emails = new List<string> { "Ronald@example.com", "Ronald@contoso.com" },
                    Trips =
                    {
                        new Trip()
                        {
                            TripId = 003009,
                            ShareId = new Guid("dd6a09c0-e59b-4745-8612-f4499b676c47"),
                            Name = "Gradutaion trip",
                            Budget = 6000.0f,
                            Description = "Gradution trip with friends",
                            Tags = new List<string>{"Travel"},
                            StartsAt = new DateTimeOffset(new DateTime(2013, 5, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2013, 5, 8))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Javier",
                    LastName = "Alfred",
                    UserName = "javieralfred",
                    Gender = PersonGender.Male,
                    Emails = new List<string> { "Javier@example.com", "Javier@contoso.com" },
                    AddressInfo =
                    {
                      new Location()
                      {
                          Address = "89 Jefferson Way Suite 2",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Portland",
                              Region = "WA"
                          }
                      }
                    },
                    Trips =
                    {
                        new Trip()
                        {
                            TripId = 004005,
                            ShareId = new Guid("f94e9116-8bdd-4dac-ab61-08438d0d9a71"),
                            Name = "Trip in Beijing",
                            Budget = 800.0f,
                            Description = "Trip from Shanghai to Beijing",
                            Tags = new List<string>{"Travel", "Beijing"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Willie",
                    LastName = "Ashmore",
                    UserName = "willieashmore",
                    Gender = PersonGender.Male, 
                    Emails = new List<string> { "Willie@example.com", "Willie@contoso.com" },
                    Trips =
                    {
                        new Trip()
                        {
                            TripId = 005007,
                            ShareId = new Guid("5ae142c3-5ad6-4a71-768e-5220ebf1e9f3"),
                            Name = "Business Trip",
                            Budget = 3800.5f,
                            Description = "This is my first business trip",
                            Tags = new List<string>{"business", "first"},
                            StartsAt = new DateTime(2014, 2, 1),
                            EndsAt = new DateTime(2014, 2, 4)
                        },
                        new Trip()
                        {
                            TripId = 005008,
                            ShareId = new Guid("9ce32ac3-5fd6-4a72-848e-2250ebf1e9f3"),
                            Name = "Trip in Europe",
                            Budget = 2000.0f,
                            Description = "The trip is currently in plan.",
                            Tags = new List<string>{"Travel", "plan"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 2, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 2, 4))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Vincent",
                    LastName = "Calabrese",
                    UserName = "vincentcalabrese", 
                    Gender = PersonGender.Male, 
                    Emails = new List<string> { "Vincent@example.com", "Vincent@contoso.com" },
                    AddressInfo = new List<Location>
                    {
                      new Location()
                      {
                          Address = "55 Grizzly Peak Rd.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Butte",
                              Region = "MT"
                          }
                      }
                    },
                    Trips =
                    {
                        new Trip()
                        {
                            TripId = 007010,
                            ShareId = new Guid("dd6a09c0-e59b-4745-8612-f4499b676c47"),
                            Name = "Gradutaion trip",
                            Budget = 1000.0f,
                            Description = "Gradution trip with friends",
                            Tags = new List<string>{"Travel"},
                            StartsAt = new DateTimeOffset(new DateTime(2013, 5, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2013, 5, 8))
                        }
                    }
                },                
                new Person()
                {
                    FirstName = "Clyde",
                    LastName = "Guess", 
                    UserName = "clydeguess",
                    Gender = PersonGender.Male,
                    Emails = new List<string> { "Clyde@example.com" },
                    Trips =
                    {
                        new Trip()
                        {
                            TripId = 008011,
                            ShareId = new Guid("a88f675d-9199-4392-9656-b08e3b46df8a"),
                            Name = "Study trip",
                            Budget = 1550.3f,
                            Description = "This is a 2 weeks study trip",
                            Tags = new List<string>{"study"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 14))
                        }
                    }
                },                
                new Person()
                {
                    FirstName = "Keith",
                    LastName = "Pinckney", 
                    UserName = "keithpinckney",
                    Gender = PersonGender.Male, 
                    Emails = new List<string> { "Keith@example.com", "Keith@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Marshall", 
                    LastName = "Garay",
                    UserName = "marshallgaray", 
                    Gender = PersonGender.Male, 
                    Emails = new List<string> { "Marshall@example.com", "Marshall@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Ryan", 
                    LastName = "Theriault", 
                    UserName = "ryantheriault", 
                    Gender = PersonGender.Male,
                    Emails = new List<string> { "Ryan@example.com", "Ryan@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Elaine",
                    LastName = "Stewart", 
                    UserName = "elainestewart",
                    Gender = PersonGender.Female,
                    Emails = new List<string> { "Elaine@example.com", "Elaine@contoso.com" }
                },                
                new Person()
                {
                    FirstName = "Sallie", 
                    LastName = "Sampson",
                    UserName = "salliesampson",
                    Gender = PersonGender.Female,
                    Emails = new List<string> { "Sallie@example.com", "Sallie@contoso.com" },
                    AddressInfo = new List<Location>
                    {
                      new Location()
                      {
                          Address = "87 Polk St. Suite 5",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "San Francisco",
                              Region = "CA"
                          }
                      },
                      new Location()
                      {
                          Address = "89 Chiaroscuro Rd.",
                          City = new City()
                          {
                              CountryRegion = "United States",
                              Name = "Portland",
                              Region = "OR"
                          }
                      }
                    },
                    Trips =
                    {
                        new Trip()
                        {
                            TripId = 013012,
                            ShareId = new Guid("a88f675d-9199-4392-9656-b08e3b46df8a"),
                            Name = "Study trip",
                            Budget = 600.0f,
                            Description = "This is a 2 weeks study trip",
                            Tags = new List<string>{"study"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 14))
                        }
                    }
                },                
                new Person()
                {
                    FirstName = "Joni",
                    LastName = "Rosales",
                    UserName = "jonirosales", 
                    Gender = PersonGender.Female, 
                    Emails = new List<string> { "Joni@example.com", "Joni@contoso.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 014013,
                            ShareId = new Guid("a88f675d-9199-4392-9656-b08e3b46df8a"),
                            Name = "Study trip",
                            Budget = 2000.0f,
                            Description = "This is a 2 weeks study trip",
                            Tags = new List<string>{"study"},
                            StartsAt = new DateTimeOffset(new DateTime(2014, 1, 1)),
                            EndsAt = new DateTimeOffset(new DateTime(2014, 1, 14))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Georgina",
                    LastName = "Barlow",
                    UserName = "georginabarlow",
                    Gender = PersonGender.Female,
                    Emails = new List<string> { "Georgina@example.com", "Georgina@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Angel", 
                    LastName = "Huffman", 
                    UserName = "angelhuffman", Gender = PersonGender.Female,
                    Emails = new List<string> { "Angel@example.com" },
                    Trips = 
                    {
                        new Trip()
                        {
                            TripId = 016014,
                            ShareId = new Guid("cb0b8acb-79cb-4127-8316-772bc4302824"),
                            Name = "DIY Trip",
                            Budget = 1500.3f,
                            Description = "This is a DIY trip",
                            Tags = new List<string>{"Travel", "DIY"},
                            StartsAt = new DateTimeOffset(new DateTime(2011, 2, 11)),
                            EndsAt = new DateTimeOffset(new DateTime(2011, 2, 14))
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Laurel", 
                    LastName = "Osborn",
                    UserName = "laurelosborn", 
                    Gender = PersonGender.Female, 
                    Emails = new List<string> { "Laurel@example.com", "Laurel@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Sandy",
                    LastName = "Osborn",
                    UserName = "sandyosborn",
                    Gender = PersonGender.Female, 
                    Emails = new List<string> { "Sandy@example.com", "Sandy@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Ursula",
                    LastName = "Bright",
                    UserName = "ursulabright",
                    Gender = PersonGender.Female,
                    Emails = new List<string> { "Ursula@example.com", "Ursula@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Genevieve",
                    LastName = "Reeves",
                    UserName = "genevievereeves", 
                    Gender = PersonGender.Female,
                    Emails = new List<string> { "Genevieve@example.com", "Genevieve@contoso.com" }
                },
                new Person()
                {
                    FirstName = "Krista", 
                    LastName = "Kemp",
                    UserName = "kristakemp",
                    Gender = PersonGender.Female,
                    Emails = new List<string> { "Krista@example.com" }
                }
            });

            People.Single(p => p.UserName == "russellwhyte").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "scottketchum"),
                    People.Single(p => p.UserName == "ronaldmundy"),
                    People.Single(p => p.UserName == "javieralfred")
                });
            People.Single(p => p.UserName == "scottketchum").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "russellwhyte"),
                    People.Single(p => p.UserName == "ronaldmundy")
                });
            People.Single(p => p.UserName == "ronaldmundy").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "russellwhyte"),
                    People.Single(p => p.UserName == "scottketchum")
                });
            People.Single(p => p.UserName == "javieralfred").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "willieashmore"),
                    People.Single(p => p.UserName == "vincentcalabrese")
                });
            People.Single(p => p.UserName == "willieashmore").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "javieralfred"),
                    People.Single(p => p.UserName == "vincentcalabrese")
                });
            People.Single(p => p.UserName == "vincentcalabrese").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "javieralfred"),
                    People.Single(p => p.UserName == "willieashmore")
                });
            People.Single(p => p.UserName == "clydeguess").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "keithpinckney")
                });
            People.Single(p => p.UserName == "keithpinckney").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "clydeguess"),
                    People.Single(p => p.UserName == "marshallgaray")
                });
            People.Single(p => p.UserName == "marshallgaray").Friends.AddRange(new[]
                {
                    People.Single(p => p.UserName == "keithpinckney")
                });

            Me.Friends.AddRange(People);
        }

        private void InitializeCustomerOrder()
        {
            const int count = 7;

            IList<IList<Color>> colors = new IList<Color>[]
            {
                new[] {Color.Red, Color.Green},
                new[] {Color.Blue, Color.Blue},
                new[] {Color.Pink, Color.Yellow, Color.Purple},
                new[] {Color.Green, Color.Green},
                new[] {Color.Purple, Color.Blue},
                new[] {Color.Yellow, Color.Purple},
                new[] {Color.Red, Color.Purple, Color.Green},
            };

            Guid[] tokens =
            {
                new Guid("F83E70FC-CFAB-45EF-9056-FB3D9B71E221"),
                new Guid("7548E3A2-6BB2-4797-92C6-11008A8FFDD3"),
                new Guid("F0B68809-7E49-4447-820F-6533A4E8EAF9"),
                new Guid("BD4CDC8E-45AB-4F20-86C5-6EE838C8D554"),
                new Guid("1E72CC05-406C-40FA-ACF9-AD24D87B74E1"),
                new Guid("E9ACEEB0-DE5B-42B1-A4E9-B1B9CF71994F"),
                new Guid("57B7C8E1-10BF-4EB0-9BD3-A54A51530E30")
            };

            string[] names = { "John", "Peter", "Mike", "Sam", "Mark", "Ted", "Bear" };

            Address redmond = new Address { City = "Redmond", Street = "One Microsoft way" };
            Address shanghai = new Address { City = "Shanghai", Street = "ZiXing Rd" };
            Address beijing = new Address { City = "Beijing", Street = "Fujian Rd" };

            IList<IList<Address>> addresses = new IList<Address>[]
            {
                new []{ redmond },
                new []{ shanghai },
                new []{ beijing },
                new []{ redmond, beijing },
                new []{ redmond, beijing, shanghai },
                new []{ beijing, shanghai },
                new []{ redmond, shanghai }
            };

            this.Customers.AddRange(Enumerable.Range(1, count).Select(e =>
                new Customer
                {
                    CustomerId = e,
                    Name = names[e - 1],
                    FavoriateColors = colors[e - 1],
                    Addresses = addresses[e - 1],
                    Email = names[e - 1] + "@microsoft.com",
                    Token = tokens[e - 1],
                    Orders = Enumerable.Range(1, e).Select(f => new Order
                    {
                        OrderId = 10*e + f,
                        Price = 9.9*e + f
                    }).ToList()
                }));

            foreach (var customer in Customers)
            {
                this.Orders.AddRange(customer.Orders);
            }
        }
    }
}