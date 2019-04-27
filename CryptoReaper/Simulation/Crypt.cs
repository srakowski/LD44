using CryptoReaper.Simulation.CryptFeatures;
using System;
using System.Collections.Generic;

namespace CryptoReaper.Simulation
{
    /// <summary>
    /// A building where the mining machines run.
    /// </summary>
    class Crypt
    {
        public abstract class Feature { }

        private readonly Dictionary<string, Feature> _cryptFeatures = new Dictionary<string, Feature>();

        public struct Coords
        {
            private string _key;

            public Coords(int row, int col)
            {
                if (row < 0 || col < 0)
                    throw new Exception("building coords may not be negative");

                Row = row;
                Col = col;
                _key = $"{row},{col}";
            }

            public int Row { get; }
            public int Col { get; }

            public bool HasFeature(Dictionary<string, Feature> features) => features.TryGetValue(_key, out var _);

            public Feature GetFeature(Dictionary<string, Feature> features)
            {
                if (!this.HasFeature(features)) return new Undefined();
                return features[_key];
            }

            public void SetFeature(Dictionary<string, Feature> cryptFeatures, Feature value)
            {
                cryptFeatures[_key] = value;
            }
        }

        public Feature this[Coords coords]
        {
            get => coords.GetFeature(_cryptFeatures);
            set => coords.SetFeature(_cryptFeatures, value);
        }

        public PlaceDeviceResult PlaceDevice(Coords coords, Device device)
        {
            if (!coords.HasFeature(_cryptFeatures))
                return PlaceDeviceResult.InvalidFeature;

            var buildingFeature = coords.GetFeature(_cryptFeatures);

            if (buildingFeature is DeviceContainer dc)
            {
                return dc.SetDevice(device).Match(
                    success: () => PlaceDeviceResult.Success,
                    occupied: () => PlaceDeviceResult.AlreadyHasDevice,
                    unsupported: () => PlaceDeviceResult.CryptFeatureDoesNotSupportDeviceType
                );
            }

            return PlaceDeviceResult.CryptFeatureDoesNotSupportDevices;
        }

        public enum PlaceDeviceResult
        {
            Success,
            InvalidFeature,
            CryptFeatureDoesNotSupportDevices,
            CryptFeatureDoesNotSupportDeviceType,
            AlreadyHasDevice,
        }

        public RemoveDeviceResult RemoveDevice(Coords coords)
        {
            if (!coords.HasFeature(_cryptFeatures))
                return RemoveDeviceResult.InvalidFeature;

            var buildingFeature = coords.GetFeature(_cryptFeatures);

            if (buildingFeature is OpenSpace os)
            {
                return os.ClearDevice()
                    .Match(success: () => RemoveDeviceResult.Success);
            }

            return RemoveDeviceResult.BuildingFixtureDoesNotSupportDevices;
        }

        public enum RemoveDeviceResult
        {
            Success,
            InvalidFeature,
            BuildingFixtureDoesNotSupportDevices,
        }
    }

    static partial class Extension
    {
        public static T Match<T>(
            this Crypt.PlaceDeviceResult self,
            Func<T> success,
            Func<T> invalidFeature,
            Func<T> cryptFeatureDoesNotSupportDevices,
            Func<T> cryptFeatureDoesNotSupportDeviceType,
            Func<T> alreadyHasDevice
            )
        {
            if (self == Crypt.PlaceDeviceResult.Success) return success();
            else if (self == Crypt.PlaceDeviceResult.InvalidFeature) return invalidFeature();
            else if (self == Crypt.PlaceDeviceResult.CryptFeatureDoesNotSupportDevices) return cryptFeatureDoesNotSupportDevices();
            else if (self == Crypt.PlaceDeviceResult.CryptFeatureDoesNotSupportDeviceType) return cryptFeatureDoesNotSupportDeviceType();
            else if (self == Crypt.PlaceDeviceResult.AlreadyHasDevice) return alreadyHasDevice();
            throw new Exception("value not mapped");
        }
    }
}
