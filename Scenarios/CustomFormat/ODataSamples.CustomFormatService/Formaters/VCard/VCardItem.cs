namespace ODataSamples.CustomFormatService.Formaters.VCard
{
    using System.Collections.Generic;

    internal class VCardItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Groups { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
