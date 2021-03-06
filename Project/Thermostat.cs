﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SharpLib.FritzBox.SmartHome
{
    /// <summary>
    /// Not sure why we need the data contract here since we use the XML serializer anyway.
    /// </summary>
    [DataContract(Namespace = "", Name = "hkr"), XmlSerializerFormat, XmlRoot("hkr")]
    public class Thermostat
    {
        public enum Radiator
        {
            /// <summary>
            /// Radiator is turned on to full power.
            /// </summary>
            On = 254,
            /// <summary>
            /// Radiator regulated to maintain target temperature.
            /// </summary>
            Regulated = 42, // 21 C
            /// <summary>
            /// Radiator is turned off.
            /// </summary>
            Off = 253
        }

        const float KMinCelsius = 8.0f;
        const float KMaxCelsius = 30.0f;

        public bool IsOnMax { get { return IsTemperatureOn(TargetTemperature); } }
        public bool IsOff { get { return IsTemperatureOff(TargetTemperature); } }
        public bool Isregulated { get { return !IsOnMax && !IsOff; } }


        public static bool IsTemperatureOn(int aValue) { return aValue == (int)Radiator.On; }
        public static bool IsTemperatureOff(int aValue) { return aValue == (int)Radiator.Off; }
        public static float TemperatureInCelsius(int aValue)
        {
            if (IsTemperatureOff(aValue))
            {
                // Return fake value that makes sense
                return 0;
            }
            else if (IsTemperatureOn(aValue))
            {
                // Return fake value that makes sense
                return 30;
            }
            // Convert to Celsius then
            return aValue * 0.5f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aCelsius"></param>
        /// <returns></returns>
        public static int CelsiusToTemperatureCode(float aCelsius)
        {
            if (aCelsius < KMinCelsius)
            {
                return (int)Radiator.Off;
            }

            if (aCelsius > KMaxCelsius)
            {
                return (int)Radiator.On;
            }

            return (int)(aCelsius/0.5f); // 
        }

        public float CurrentTemperatureInCelsius { get { return TemperatureInCelsius(CurrentTemperature); } }
        public float TargetTemperatureInCelsius { get { return TemperatureInCelsius(TargetTemperature); } }
        public float EconomyTemperatureInCelsius { get { return TemperatureInCelsius(EconomyTemperature); } }
        public float ComfortTemperatureInCelsius { get { return TemperatureInCelsius(ComfortTemperature); } }

        [DataMember, XmlElement(Namespace = "", ElementName = "tist")]
        public int CurrentTemperature { get; set; }

        [DataMember, XmlElement(Namespace = "", ElementName = "tsoll")]
        public int TargetTemperature { get; set; }

        [DataMember, XmlElement(Namespace = "", ElementName = "absenk")]
        public int EconomyTemperature { get; set; }

        [DataMember, XmlElement(Namespace = "", ElementName = "komfort")]
        public int ComfortTemperature { get; set; }

        /// <summary>
        /// Locked from UI/API
        /// </summary>
        [DataMember, XmlElement(Namespace = "", ElementName = "lock")]
        public Lock Lock { get; set; }

        /// <summary>
        /// Lock from device.
        /// </summary>
        [DataMember, XmlElement(Namespace = "", ElementName = "devicelock")]
        public Lock DeviceLock { get; set; }

        public enum Error
        {
            [XmlEnum(Name = "0")]
            None,
            [XmlEnum(Name = "1")]
            BadAdaptation, // Check mounting
            [XmlEnum(Name = "2")]
            WeakBatteries, // Or valve too short, check mounting
            [XmlEnum(Name = "3")]
            ValveStuck, //
            [XmlEnum(Name = "4")]
            Preparation,
            [XmlEnum(Name = "5")]
            Installation,
            [XmlEnum(Name = "6")]
            Adaptation
        }

        [DataMember, XmlElement(Namespace = "", ElementName = "errorcode")]
        public Error ErrorCode { get; set; }

        [DataMember, XmlElement(Namespace = "", ElementName = "batterylow")]
        public Battery Battery { get; set; }

        [DataMember, XmlElement(Namespace = "", ElementName = "nextchange")]
        public NextChange NextChange { get; set; }

    }

}
