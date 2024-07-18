// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Replacement methods for expressions involving open types.
    /// </summary>
    internal static class OpenTypesMethodReplacement
    {
        /// <summary>
        /// Adds the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of adding the objects</returns>
        public static object Add(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) + Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) + Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) + Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) + Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) + Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) + Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) + Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot add type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Ands the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of and-ing the objects</returns>
        public static object AndAlso(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left && (bool)right;
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", left.GetType(), right.GetType()));
        }

        /// <summary>
        /// Gets the ceiling of the object
        /// </summary>
        /// <param name="value">The value to get the ceiling of</param>
        /// <returns>The ceiling of the object</returns>
        public static object Ceiling(object value)
        {
            if (value is decimal)
            {
                return Math.Ceiling((decimal)value);
            }
            else if (value is double)
            {
                return Math.Ceiling((double)value);
            }
            else if (value is float)
            {
                return Math.Ceiling((float)value);
            }

            throw new DataServiceException(400, "Cannot use Ceiling with type " + value.GetType());
        }

        /// <summary>
        /// Concats the two objects
        /// </summary>
        /// <param name="first">The first object</param>
        /// <param name="second">The second object</param>
        /// <returns>The result of concatenating the objects</returns>
        public static object Concat(object first, object second)
        {
            if (first is string && second is string)
            {
                return (string)first + (string)second;
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Concat not valid between types {0} and {1}", first.GetType(), second.GetType()));
        }

        /// <summary>
        /// Gets the day of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the day of</param>
        /// <returns>The day of the object</returns>
        public static object Day(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Day");
        }

        /// <summary>
        /// Divides the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of dividing the objects</returns>
        public static object Divide(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) / Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) / Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) / Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) / Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) / Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) / Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) / Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot divide type {0} by {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Returns whether the target object ends with the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it ends with the substring</returns>
        public static object EndsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
            {
                return ((string)targetString).EndsWith((string)substring, StringComparison.Ordinal);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", targetString.GetType(), substring.GetType()));
        }

        /// <summary>
        /// Gets the floor of the object
        /// </summary>
        /// <param name="value">The value to get the floor of</param>
        /// <returns>The floor of the object</returns>
        public static object Floor(object value)
        {
            if (value is decimal)
            {
                return Math.Floor((decimal)value);
            }
            else if (value is double)
            {
                return Math.Floor((double)value);
            }
            else if (value is float)
            {
                return Math.Floor((float)value);
            }

            throw new DataServiceException(400, "Floor not valid for type " + value.GetType());
        }

        /// <summary>
        /// Gets the hour of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the hour of</param>
        /// <returns>The hour of the object</returns>
        public static object Hour(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Hour");
        }

        /// <summary>
        /// Returns the index where the substring first appears in the target
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to get the index of</param>
        /// <returns>the index where the substring first appears in the target</returns>
        public static object IndexOf(object targetString, object substring)
        {
            if (targetString is string && substring is string)
            {
                return ((string)targetString).IndexOf((string)substring, StringComparison.Ordinal);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", targetString.GetType(), substring.GetType()));
        }

        /// <summary>
        /// Gets the length of the object
        /// </summary>
        /// <param name="value">The value to get the length of</param>
        /// <returns>The length of the object</returns>
        public static object Length(object value)
        {
            if (value is string)
            {
                return ((string)value).Length;
            }
            else if (value is Array)
            {
                return ((Array)value).Length;
            }
            else
            {
                throw new DataServiceException(400, "Length not valid on type " + value.GetType());
            }
        }

        /// <summary>
        /// Gets the minute of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the minute of</param>
        /// <returns>The minute of the object</returns>
        public static object Minute(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Minute");
        }

        /// <summary>
        /// Modulos the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of mod-ing the objects</returns>
        public static object Modulo(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) % Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) % Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) % Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) % Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) % Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) % Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) % Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot modulo type {0} by {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Gets the month of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the month of</param>
        /// <returns>The month of the object</returns>
        public static object Month(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Month");
        }

        /// <summary>
        /// Multiplies the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of multiplying the objects</returns>
        public static object Multiply(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) * Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) * Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) * Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) * Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) * Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) * Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) * Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot multiply type {0} by {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Gets the negation of the object
        /// </summary>
        /// <param name="value">The value to negate</param>
        /// <returns>The negated object</returns>
        public static object Negate(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                    return -(byte)value;

                case TypeCode.Int16:
                    return -(short)value;

                case TypeCode.Int32:
                    return -(int)value;

                case TypeCode.Int64:
                    return -(long)value;
            }

            return null;
        }

        /// <summary>
        /// Gets the boolean inverse of the object
        /// </summary>
        /// <param name="value">The value to get the boolean inverse of</param>
        /// <returns>The boolean inverse of the object</returns>
        public static object Not(object value)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return value != null;
            }
        }

        /// <summary>
        /// Elses the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of else-ing the objects</returns>
        public static object OrElse(object left, object right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left || (bool)right;
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Not valid between types {0} and {1}", left.GetType(), right.GetType()));
        }

        /// <summary>
        /// Replaces instances of the given substring with the new string in the target
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to replace</param>
        /// <param name="newString">The object to replace it with</param>
        /// <returns>The object with replacements</returns>
        public static object Replace(object targetString, object substring, object newString)
        {
            if (targetString is string && substring is string && newString is string)
            {
                return ((string)targetString).Replace((string)substring, (string)newString);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0}, {1}, and {2}", targetString.GetType(), substring.GetType(), newString.GetType()));
        }

        /// <summary>
        /// Gets the rounded version of the object
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded object</returns>
        public static object Round(object value)
        {
            if (value is decimal)
            {
                return Math.Round((decimal)value);
            }
            else if (value is double)
            {
                return Math.Round((double)value);
            }
            else if (value is float)
            {
                return Math.Round((float)value);
            }

            throw new DataServiceException("Cannot use Round with type " + value.GetType());
        }

        /// <summary>
        /// Gets the second of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the second of</param>
        /// <returns>The second of the object</returns>
        public static object Second(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Second");
        }

        /// <summary>
        /// Returns whether the target object starts with the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it starts with the substring</returns>
        public static object StartsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
            {
                return ((string)targetString).StartsWith((string)substring, StringComparison.Ordinal);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0} and {1}", substring.GetType(), targetString.GetType()));
        }

        /// <summary>
        /// Gets the substring of the given object
        /// </summary>
        /// <param name="targetString">The object to get the substring of</param>
        /// <param name="startIndex">The start index</param>
        /// <returns>The substring of the object</returns>
        public static object Substring(object targetString, object startIndex)
        {
            if (targetString is string && startIndex is int)
            {
                return ((string)targetString).Substring((int)startIndex);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0} and {1}", targetString.GetType(), startIndex.GetType()));
        }

        /// <summary>
        /// Gets the substring of the given object
        /// </summary>
        /// <param name="targetString">The object to get the substring of</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="length">the length of the substring</param>
        /// <returns>The substring of the object</returns>
        public static object Substring(object targetString, object startIndex, object length)
        {
            if (targetString is string && startIndex is int && length is int)
            {
                return ((string)targetString).Substring((int)startIndex, (int)length);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use Substring with types {0}, {1} and {2}", targetString.GetType(), startIndex.GetType(), length.GetType()));
        }

        /// <summary>
        /// Returns whether the target object contains the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it contains with the substring</returns>
        public static object Contains(object targetString, object substring)
        {
            if (substring is string && targetString is string)
            {
                return ((string)targetString).Contains((string)substring);
            }

            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use with types {0} and {1}", substring.GetType(), targetString.GetType()));
        }

        /// <summary>
        /// Subtracts the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of subtracting the objects</returns>
        public static object Subtract(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) - Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) - Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) - Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) - Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) - Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) - Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) - Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot subtract type {0} from type {1}", left.GetType(), right.GetType()));
            }
        }

        /// <summary>
        /// Gets the lowercase version of the given object
        /// </summary>
        /// <param name="targetString">The object to lowercase</param>
        /// <returns>The lowercase version of the object</returns>
        public static object ToLower(object targetString)
        {
            if (targetString is string)
            {
                return ((string)targetString).ToLowerInvariant();
            }

            throw new DataServiceException(400, "Cannot use with type " + targetString.GetType());
        }

        /// <summary>
        /// Gets the uppercase version of the given object
        /// </summary>
        /// <param name="targetString">The object to uppercase</param>
        /// <returns>The uppercase version of the object</returns>
        public static object ToUpper(object targetString)
        {
            if (targetString is string)
            {
                return ((string)targetString).ToUpperInvariant();
            }

            throw new DataServiceException(400, "Cannot use with type " + targetString.GetType());
        }

        /// <summary>
        /// Trims the given object
        /// </summary>
        /// <param name="targetString">The object to trim</param>
        /// <returns>The trimmed object</returns>
        public static object Trim(object targetString)
        {
            if (targetString is string)
            {
                return ((string)targetString).Trim();
            }

            throw new DataServiceException(400, "Cannot use with type " + targetString.GetType());
        }

        /// <summary>
        /// Gets the year of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the year of</param>
        /// <returns>The year of the object</returns>
        public static object Year(object dateTimeOffset)
        {
            return GetPropertyValue(dateTimeOffset, "Year");
        }

        /// <summary>
        /// Converts the given object to the given type
        /// </summary>
        /// <param name="value">The object to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted object</returns>
        public static object Convert(object value, ResourceType type)
        {
            if (value == null)
            {
                return null;
            }

            if (type.InstanceType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            throw new InvalidCastException("Instance of '" + value.GetType() + "' cannot be converted to resource type '" + type.FullName + "'");
        }

        /// <summary>
        /// Returns whether the left object is greater than the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is greater than the right object</returns>
        public static object GreaterThan(object left, object right)
        {
            try
            {
                return Compare(left, right) > 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the left object is greater than or equal to the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is greater than or equal to the right object</returns>
        public static object GreaterThanOrEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) >= 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the left object is less than the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is less than the right object</returns>
        public static object LessThan(object left, object right)
        {
            try
            {
                return Compare(left, right) < 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the left object is less than or equal to the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is less than or equal to the right object</returns>
        public static object LessThanOrEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) <= 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        /// <summary>
        /// Returns whether the two objects are equal
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>Whether the two object are equal</returns>
        public static object Equal(object left, object right)
        {
            return Compare(left, right) == 0;
        }

        /// <summary>
        /// Returns whether the two objects are not equal
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>Whether the two object are not equal</returns>
        public static object NotEqual(object left, object right)
        {
            return Compare(left, right) != 0;
        }

        /// <summary>
        /// Compares the two objects
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns>The result of comparing the objects</returns>
        public static int Compare(object x, object y)
        {
            if (x == y)
            {
                return 0;
            }

            // null is 'less than' a non-null value
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (y == null)
            {
                return 1;
            }

            var firstArray = x as Array;
            var secondArray = y as Array;
            if (firstArray != null || secondArray != null)
            {
                return CompareArrays(firstArray, secondArray);
            }

            if (x.GetType() == y.GetType())
            {
                var comparable = x as IComparable;
                if (comparable != null)
                {
                    return comparable.CompareTo(y);
                }
            }

            // just compare them as doubles rather than trying to emulate real numeric type promotion
            var doubleX = System.Convert.ToDouble(x, CultureInfo.InvariantCulture);
            var doubleY = System.Convert.ToDouble(y, CultureInfo.InvariantCulture);
            return doubleX.CompareTo(doubleY);
        }

       
        /// <summary>
        /// Helper method for converting an object to a generic type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="thing">The object to convert</param>
        /// <returns>The converted object</returns>
        private static T Convert<T>(object thing)
        {
            return (T)System.Convert.ChangeType(thing, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Handles distance for non-spatial types or throws.
        /// </summary>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <returns>The distance between the non-spatial values</returns>
        private static object HandleNonSpatialDistance(object operand1, object operand2)
        {
            throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Distance method is only valid for spatial types. Arguments were: '{0}' and '{1}'", operand1, operand2));
        }

        /// <summary>
        /// Tries to get a property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The result</returns>
        private static bool TryGetPropertyValue(object instance, string propertyName, out object propertyValue)
        {
            propertyValue = null;

            if (instance == null)
            {
                return true;
            }

            var type = instance.GetType();
            var property = type.GetProperty(propertyName);
            if (property == null && type == typeof(TimeSpan))
            {
                // TimeSpan property names are slightly different for some reason
                property = type.GetProperty(propertyName + "s");
            }

            if (property == null)
            {
                return false;
            }

            propertyValue = property.GetValue(instance, null);
            return true;
        }

        private static object GetPropertyValue(object thing, string propertyName)
        {
            object value;
            if (!TryGetPropertyValue(thing, propertyName, out value))
            {
                throw new DataServiceException(400, string.Format(CultureInfo.InvariantCulture, "Cannot use {0} with type {1}", propertyName, thing.GetType()));
            }

            return value;
        }

        private static int CompareArrays(Array firstArray, Array secondArray)
        {
            Debug.Assert(firstArray != null || secondArray != null, "Should only be called if at least one value is an array");
            if (firstArray == null)
            {
                return -1;
            }
            else if (secondArray == null)
            {
                return 1;
            }
            else
            {
                if (firstArray.Length < secondArray.Length)
                {
                    return -1;
                }
                else if (secondArray.Length < firstArray.Length)
                {
                    return 1;
                }
                else
                {
                    for (int i = 0; i < firstArray.Length; i++)
                    {
                        var result = Compare(firstArray.GetValue(i), secondArray.GetValue(i));
                        if (result != 0)
                        {
                            return result;
                        }
                    }

                    return 0;
                }
            }
        }

    }
}
