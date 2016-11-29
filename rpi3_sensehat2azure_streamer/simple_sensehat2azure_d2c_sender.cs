using System;
using System.Text;
using System.Threading.Tasks;
using Emmellsoft.IoT.Rpi.SenseHat;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace rpi3_sensehat2azure_streamer
{
    public class simple_sensehat2azure_d2c_sender
    {
        private static readonly string deviceKey = "FF4zWqDxlIyCwgHiz1rptvw+BOH4C8pge9F7+xKe92o=";
        private static readonly string iotHubUri = "jkiothub.azure-devices.net";
        private static readonly string deviceId = "jkrpi3";

        private static readonly DeviceClient deviceClient = DeviceClient.Create(iotHubUri,
            new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

        public static async
            Task
            SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            var rand = new Random();
            var senseHat = await SenseHatFactory.GetSenseHat().ConfigureAwait(false);

            while (true)
            {
                senseHat.Sensors.ImuSensor.Update();
                    // Try get a new read-out for the Gyro, Acceleration, MagneticField and Pose.
                senseHat.Sensors.PressureSensor.Update(); // Try get a new read-out for the Pressure.
                senseHat.Sensors.HumiditySensor.Update(); // Try get a new read-out for the Temperature and Humidity.

                var telemetryDataPoint = new
                {
                    deviceId,
                    temperature = senseHat.Sensors.Temperature,
                    humidity = senseHat.Sensors.Humidity,
                    light = avgWindSpeed + rand.Next(40), // todo
                    move = false,
                    timestamp = DateTime.Now.ToString()
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);

                Task.Delay(1000).Wait();
            }
        }
    }
}