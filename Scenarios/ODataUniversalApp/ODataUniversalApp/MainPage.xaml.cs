using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.OData.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ODataUniversalApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DefaultContainer _context =
            new DefaultContainer(new Uri("http://services.odata.org/V4/(S(gyijqznoqgfzrz0llzcwwjub))/TripPinServiceRW/"));

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void AddText(string text)
        {
            this.textBox.Text += text + Environment.NewLine;
        }

        private void ClearText()
        {
            SetText(string.Empty);
        }

        private void SetText(string text)
        {
            this.textBox.Text = text;
        }

        private static T WaitForResult<T>(Task<T> task)
        {
            task.Wait();
            return task.Result;
        }

        private void query_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var airlines = WaitForResult(_context.Airlines.ExecuteAsync());
                ClearText();
                foreach (var airline in airlines)
                {
                    AddText(airline.AirlineCode);
                }
            });
        }

        private void inheritance_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var airports = WaitForResult(_context.Airports.ExecuteAsync());
                ClearText();
                foreach (var airport in airports)
                {
                    AddText($"The Location of Airports('{airport.IcaoCode}') is of type '{airport.Location.GetType()}'.");
                }
            });
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var airlines = WaitForResult(_context.Airlines.ExecuteAsync());
                var airline = airlines.Single(a => a.AirlineCode == "AA");
                _context.DeleteObject(airline);
                _context.SaveChangesAsync();
                SetText("Deleted Airlines('AA').");
            });
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var airlines = WaitForResult(_context.Airlines.ExecuteAsync());
                var airline = airlines.Single(a => a.AirlineCode == "FM");
                airline.Name = "Shanghai OData Airline";
                _context.UpdateObject(airline);
                _context.SaveChangesAsync();
                SetText("Updated Airlines('FM')/Name to 'Shanghai OData Airline'.");
            });
        }

        private void operation_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var airport = WaitForResult(_context.GetNearestAirport(0, 0).GetValueAsync());
                SetText($"The nearest airport is Airports('{airport.IcaoCode}').");
            });
        }

        private void annotation_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                _context.SendingRequest2 += (requestSender, eventArgs) =>
                {
                    eventArgs.RequestMessage.SetHeader("Prefer", "odata.include-annotations=\"*\"");
                };

                var people = WaitForResult(_context.People.ExecuteAsync());
                var person = people.Single(p => p.UserName == "russellwhyte");

                string annotationValue;
                if (_context.TryGetAnnotation(person, "TestAnnotation", "TestQualifier", out annotationValue))
                {
                    SetText($"Annotation on People('russellwhyte') is '{annotationValue}'.");
                }
                else
                {
                    SetText("No annotation on People('russellwhyte').");
                }
            });
        }

        private void collectionPrimitive_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var people = WaitForResult(_context.People.ExecuteAsync());
                var person = people.Single(p => p.UserName == "russellwhyte");
                SetText("People('russellwhyte') has emails:\n");
                foreach (var email in person.Emails)
                {
                    AddText(email);
                }
            });
        }

        private void collectionComplex_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var people = WaitForResult(_context.People.ExecuteAsync());
                var person = people.Single(p => p.UserName == "russellwhyte");
                SetText("People('russellwhyte') has addresses:\n");
                foreach (var address in person.AddressInfo)
                {
                    AddText(
                        $"{address.Address}, {address.City.Region}, {address.City.Name}, {address.City.CountryRegion}");
                }
            });
        }

        private void navigation_Click(object sender, RoutedEventArgs e)
        {
            ExceptionFriendly(() =>
            {
                var people = WaitForResult(_context.People.Expand(p => p.Friends).ExecuteAsync());
                var person = people.Single(p => p.UserName == "russellwhyte");
                SetText("People('russellwhyte') has friends:\n");
                foreach (var friend in person.Friends)
                {
                    AddText(friend.UserName);
                }
            });
        }

        private void ExceptionFriendly(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                SetText("Main Exception:\n" + ex.Message + "\n" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    AddText("\n\nInner Exception:\n" + ex.InnerException.Message + "\n" + ex.InnerException.StackTrace);
                }
            }
        }
}
}
