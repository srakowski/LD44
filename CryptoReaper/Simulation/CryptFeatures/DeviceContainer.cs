using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoReaper.Simulation.CryptFeatures
{
    abstract class DeviceContainer : Crypt.Feature
    {
        private readonly IEnumerable<Type> _supportedDeviceTypes;

        protected DeviceContainer(IEnumerable<Type> supportedDeviceTypes)
        {
            _supportedDeviceTypes = supportedDeviceTypes;
        }

        public Device Device { get; private set; }

        public SetDeviceResult SetDevice(Device device)
        {
            if (Device != null)
                return SetDeviceResult.Occupied;

            if (!_supportedDeviceTypes.Contains(device.GetType()))
                return SetDeviceResult.Unsupported;

            Device = device;

            return SetDeviceResult.Success;
        }

        public enum SetDeviceResult
        {
            Success,
            Occupied,
            Unsupported
        }

        public ClearDeviceResult ClearDevice()
        {
            Device = null;
            return ClearDeviceResult.Success;
        }

        public enum ClearDeviceResult
        {
            Success,
        }
    }

    static partial class Extension
    {
        public static T Match<T>(
            this DeviceContainer.SetDeviceResult self,
            Func<T> success,
            Func<T> occupied,
            Func<T> unsupported
            )
        {
            if (self == DeviceContainer.SetDeviceResult.Success) return success();
            else if (self == DeviceContainer.SetDeviceResult.Occupied) return occupied();
            else if (self == DeviceContainer.SetDeviceResult.Unsupported) return unsupported();
            throw new Exception("value not mapped");
        }

        public static T Match<T>(this OpenSpace.ClearDeviceResult self, Func<T> success) =>
            self == DeviceContainer.ClearDeviceResult.Success ? success() :
            throw new Exception("value not mapped");
    }
}
