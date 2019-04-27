using CryptoReaper.Simulation.CryptFeature;
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
                if (!this.HasFeature(features)) throw new Exception("building does not have feature");
                return features[_key];
            }
        }

        public Feature this[Coords coords]
        {
            get =>
        }

        public PlaceDeviceResult PlaceDevice(Coords coords, Device device)
        {
            if (!coords.HasFeature(_cryptFeatures))
                return PlaceDeviceResult.InvalidFeature;

            var buildingFeature = coords.GetFeature(_cryptFeatures);

            if (buildingFeature is OpenSpace os)
            {
                return os.SetDevice(device).Match(
                    success: () => PlaceDeviceResult.Success,
                    occupied: () => PlaceDeviceResult.AlreadyHasDevice
                );
            }

            return PlaceDeviceResult.BuildingFixtureDoesNotSupportDevices;
        }

        public enum PlaceDeviceResult
        {
            Success,
            InvalidFeature,
            BuildingFixtureDoesNotSupportDevices,
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
            Func<T> buildingFixtureDoesNotSupportDevices,
            Func<T> alreadyHasDevice
            )
        {
            if (self == Crypt.PlaceDeviceResult.Success) return success();
            else if (self == Crypt.PlaceDeviceResult.InvalidFeature) return invalidFeature();
            else if (self == Crypt.PlaceDeviceResult.BuildingFixtureDoesNotSupportDevices) return buildingFixtureDoesNotSupportDevices();
            else if (self == Crypt.PlaceDeviceResult.AlreadyHasDevice) return alreadyHasDevice();
            throw new Exception("value not mapped");
        }
    }
}
