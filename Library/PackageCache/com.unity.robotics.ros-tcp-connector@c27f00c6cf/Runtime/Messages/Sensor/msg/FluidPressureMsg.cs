//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Sensor
{
    [Serializable]
    public class FluidPressureMsg : Message
    {
        public const string k_RosMessageName = "sensor_msgs/FluidPressure";
        public override string RosMessageName => k_RosMessageName;

        //  Single pressure reading.  This message is appropriate for measuring the
        //  pressure inside of a fluid (air, water, etc).  This also includes
        //  atmospheric or barometric pressure.
        // 
        //  This message is not appropriate for force/pressure contact sensors.
        public Std.HeaderMsg header;
        //  timestamp of the measurement
        //  frame_id is the location of the pressure sensor
        public double fluid_pressure;
        //  Absolute pressure reading in Pascals.
        public double variance;
        //  0 is interpreted as variance unknown

        public FluidPressureMsg()
        {
            this.header = new Std.HeaderMsg();
            this.fluid_pressure = 0.0;
            this.variance = 0.0;
        }

        public FluidPressureMsg(Std.HeaderMsg header, double fluid_pressure, double variance)
        {
            this.header = header;
            this.fluid_pressure = fluid_pressure;
            this.variance = variance;
        }

        public static FluidPressureMsg Deserialize(MessageDeserializer deserializer) => new FluidPressureMsg(deserializer);

        private FluidPressureMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            deserializer.Read(out this.fluid_pressure);
            deserializer.Read(out this.variance);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.fluid_pressure);
            serializer.Write(this.variance);
        }

        public override string ToString()
        {
            return "FluidPressureMsg: " +
            "\nheader: " + header.ToString() +
            "\nfluid_pressure: " + fluid_pressure.ToString() +
            "\nvariance: " + variance.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}