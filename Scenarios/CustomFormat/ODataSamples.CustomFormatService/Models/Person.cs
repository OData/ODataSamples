using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ODataSamples.CustomFormatService
{
    public class Person
    {
        public int Id { get; set; }

        public DateTimeOffset UpdateTime { get; set; }

        public string Comment { get; set; }

        public BusinessCard Card { get; set; }
    }
}