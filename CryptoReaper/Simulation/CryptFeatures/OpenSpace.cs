using System;

namespace CryptoReaper.Simulation.CryptFeature
{
    class OpenSpace : Crypt.Feature
    {
        public OpenSpace(Device device = null)
        {
            Device = device;
        }

        public Device Device { get; private set; }

        public SetDeviceResult SetDevice(Device device)
        {
            if (Device != null)
                return SetDeviceResult.Occupied;

            Device = device;

            return SetDeviceResult.Success;
        }

        public enum SetDeviceResult
        {
            Success,
            Occupied,
        }

        public ClearDeviceResult ClearDevice()
        {
            Device = null;
            return ClearDeviceResult.Success;
        }

        public enum ClearDeviceResult
        {
            Success,
            StaticDevice,
        }
    }

    static partial class Extension
    {
        public static T Match<T>(
            this OpenSpace.SetDeviceResult self,
            Func<T> success,
            Func<T> occupied
            )
        {
            if (self == OpenSpace.SetDeviceResult.Success) return success();
            else if (self == OpenSpace.SetDeviceResult.Occupied) return occupied();
            throw new Exception("value not mapped");
        }

        public static T Match<T>(this OpenSpace.ClearDeviceResult self, Func<T> success) =>
            self == OpenSpace.ClearDeviceResult.Success ? success() :
            throw new Exception("value not mapped");
    }
}
