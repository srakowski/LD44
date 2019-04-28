using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static CryptoReaper.Functions;
using static System.Math;

namespace CryptoReaper
{
    public class Crypt
    {
        public const int CryptFeaturesPerSection = 200;
        public const int CryptFeaturesMidPoint = CryptFeaturesPerSection / 2;

        private readonly Random _random;
        private readonly Dictionary<string, CryptFeature> _features;
        private readonly Dictionary<string, CryptSection> _sections;
        private readonly List<CryptFeature.SpawningSpire> _spawningSpires;
        private readonly List<CryptDevice> _devices;

        public Crypt()
        {
            _random = new Random();
            _features = new Dictionary<string, CryptFeature>();
            _sections = new Dictionary<string, CryptSection>();
            _spawningSpires = new List<CryptFeature.SpawningSpire>();
            _devices = new List<CryptDevice>();
            GenerateCryptSection(0, 0);
        }

        public CryptFeature this[Point point] => this[point.Y, point.X];

        public CryptFeature this[int row, int col] => _features.TryGetValue(new Point(row, col).GetKey(), out var feature)
            ? feature : new CryptFeature.Void(null, null, new Point(row, col)) as CryptFeature;

        public void Step()
        {
            foreach (var spire in _spawningSpires)
                spire.SpawnResources();

            foreach (var device in _devices)
                device.ProcessSupply();

            foreach (var device in _devices.OrderBy(d => d.Pressure))
                device.Flush();
        }

        public bool CanPlaceCryptDevice(CryptDevice cryptDevice, Point position)
        {
            var features = cryptDevice.GetFeaturesThisWouldOccupyAtPosition(this, position);

            if (features.Any(f =>
                f is CryptFeature.Void ||
                f is CryptFeature.HardRock ||
                f is CryptFeature.SoftRock ||
                f.Device != null
                )) return false;

            return cryptDevice.Match(
                hellFireReceiver: _ => features.All(f => f is CryptFeature.HellFireSpire),
                soulReceiver: _ => features.All(f => f is CryptFeature.SoulSpire),
                pipe: _ => features.All(f => f is CryptFeature.Floor),
                cryptCoinEngine: _ => features.All(f => f is CryptFeature.Floor),
                cryptCoinExchange: _ => features.Any(f => f is CryptFeature.CryptCoinSpire)
            );
        }

        public void PlaceCryptDevice(CryptDevice cryptDevice, Point position)
        {
            if (!CanPlaceCryptDevice(cryptDevice, position)) throw new Exception("check CanPlaceCryptDevice first");
            var features = cryptDevice.GetFeaturesThisWouldOccupyAtPosition(this, position);
            foreach (var feature in features)
            {
                feature.PlaceDevice(cryptDevice);
            }
            _devices.Add(cryptDevice);
        }

        private void GenerateCryptSection(int row, int col)
        {
            var sectionPosition = new Point(col, row);
            var section = _sections[sectionPosition.GetKey()] = new CryptSection(this, sectionPosition);
            ForAllRowsAndCols(CryptFeaturesPerSection, GenerateCryptFeaturesForSection(section));
            GenerateSpires(section);
        }

        private Action<int, int> GenerateCryptFeaturesForSection(CryptSection section) =>
            (row, col) =>
            {
                var featurePosition = section.SectionPositionToCryptPosition(row, col);

                var featureRoll = _random.Next(100);

                var chanceHardRock = (Min(Abs(row - CryptFeaturesMidPoint), Abs(col - CryptFeaturesMidPoint)) * 100) / CryptFeaturesMidPoint;
                chanceHardRock = chanceHardRock < 75 ? 0 : chanceHardRock;

                var chanceSoftRock = (Min(Abs(row - CryptFeaturesMidPoint), Abs(col - CryptFeaturesMidPoint)) * 100) / CryptFeaturesMidPoint;
                chanceSoftRock = chanceSoftRock < 50 ? 0 : chanceSoftRock;

                var feature = featureRoll < chanceHardRock
                    ? new CryptFeature.HardRock(this, section, featurePosition)
                    : featureRoll < chanceSoftRock
                    ? new CryptFeature.SoftRock(this, section, featurePosition)
                    : new CryptFeature.Floor(this, section, featurePosition) as CryptFeature;

                _features[feature.Position.GetKey()] = feature;
            };

        private void GenerateSpires(CryptSection section)
        {
            var centerPoint = section.SectionPositionToCryptPosition(CryptFeaturesMidPoint, CryptFeaturesMidPoint);

            if (section.Position.X == 0 && section.Position.Y == 0)
            {
                var cryptCoinSpire = new CryptFeature.CryptCoinSpire(this, section, centerPoint);
                _features[centerPoint.GetKey()] = cryptCoinSpire;
            }

            var softFeatureQueue = new Queue<CryptFeature>(section.GetCryptFeatures()
                .Where(f => !(f is CryptFeature.HardRock) && !(f is CryptFeature.Spire))
                .Where(f =>
                {
                    var cryptPosition = section.CryptPositionToSectionPosition(f.Position);
                    var rowPos = (Abs(cryptPosition.Y - CryptFeaturesMidPoint) * 100) / CryptFeaturesMidPoint;
                    var colPos = (Abs(cryptPosition.X - CryptFeaturesMidPoint) * 100) / CryptFeaturesMidPoint;
                    return rowPos < 75 && rowPos > 25 && colPos < 75 && colPos > 25;
                })
                .OrderBy(_ => _random.Next(100))
                .ToArray());

            var featureToReplace = softFeatureQueue.Dequeue();
            var soulSpire = new CryptFeature.SoulSpire(this, section, featureToReplace.Position);
            _features[featureToReplace.Position.GetKey()] = soulSpire;
            _spawningSpires.Add(soulSpire);

            featureToReplace = softFeatureQueue.Dequeue();
            var hellFireSpire = new CryptFeature.HellFireSpire(this, section, featureToReplace.Position);
            _features[featureToReplace.Position.GetKey()] = hellFireSpire;
            _spawningSpires.Add(hellFireSpire);
        }

        public void DepositCoins(int quantity)
        {
        }

        internal void WasteResources(List<IResource> resources)
        {
            throw new NotImplementedException();
        }
    }

    public class CryptSection
    {
        public CryptSection(Crypt crypt, Point position)
        {
            Crypt = crypt;
            Position = position;
        }

        public Crypt Crypt { get; }

        public Point Position { get; }

        public IEnumerable<CryptFeature> GetCryptFeatures()
        {
            for (int row = 0; row < Crypt.CryptFeaturesPerSection; row++)
                for (int col = 0; col < Crypt.CryptFeaturesPerSection; col++)
                {
                    var featurePosition = SectionPositionToCryptPosition(row, col);
                    yield return Crypt[featurePosition.Y, featurePosition.X];
                }
        }

        public Point SectionPositionToCryptPosition(int row, int col) =>
            (new Point(col, row) + (this.Position * new Point(Crypt.CryptFeaturesPerSection, Crypt.CryptFeaturesPerSection)));

        public Point CryptPositionToSectionPosition(Point cryptPosition) =>
            (cryptPosition - (this.Position * new Point(Crypt.CryptFeaturesPerSection, Crypt.CryptFeaturesPerSection)));
    }

    public abstract class CryptFeature
    {
        private CryptFeature(Crypt crypt, CryptSection cryptSection, Point position)
        {
            Crypt = crypt;
            CryptSection = cryptSection;
            Position = position;
            Device = null;
        }

        public Crypt Crypt { get; }

        public CryptSection CryptSection { get; }

        public Point Position { get; }

        public CryptDevice Device { get; private set; }

        public void PlaceDevice(CryptDevice device)
        {
            device.SetHostFeature(this);
            this.Device = device;
        }

        public class Void : CryptFeature
        {
            public Void(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }
        }

        public class Floor : CryptFeature
        {
            public Floor(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }
        }

        public class SoftRock : CryptFeature
        {
            public SoftRock(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }
        }

        public class HardRock : CryptFeature
        {
            public HardRock(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }
        }

        public abstract class Spire : CryptFeature
        {
            protected Spire(Crypt crypt, CryptSection cryptSection, Point position) : base(crypt, cryptSection, position)
            {
            }
        }

        public abstract class SpawningSpire : Spire
        {
            protected SpawningSpire(Crypt crypt, CryptSection cryptSection, Point position) : base(crypt, cryptSection, position)
            {
            }

            public abstract void SpawnResources();
        }

        public class HellFireSpire : SpawningSpire
        {
            public HellFireSpire(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }

            public override void SpawnResources()
            {
                if (this.Device == null) return;
                this.Device.Push(new HellFire());
            }
        }

        public class SoulSpire : SpawningSpire
        {
            public SoulSpire(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }

            public override void SpawnResources()
            {
                if (this.Device == null) return;
                this.Device.Push(new Soul());
            }
        }

        public class CryptCoinSpire : Spire
        {
            public CryptCoinSpire(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }
        }
    }

    public abstract class CryptDevice
    {
        protected List<IResource> _resources = new List<IResource>();

        private CryptDevice(Crypt crypt, int resourceCapacity, bool receives, bool outputs)
        {
            Crypt = crypt;
            ResourceCapacity = resourceCapacity;
            Receives = receives;
            Outputs = outputs;
        }

        public Crypt Crypt { get; }

        public int ResourceCapacity { get; }

        public bool Receives { get; }

        public bool Outputs { get; }

        public CryptFeature HostFeature { get; private set; }

        public float Pressure => ResourceCapacity <= 0 ? 0 : _resources.Count / (float)ResourceCapacity;

        public virtual void ProcessSupply() { }

        public void SetHostFeature(CryptFeature feature)
        {
            HostFeature = feature;
        }

        public void Push(IResource resource)
        {
            _resources.Add(resource);
        }

        public void Flush()
        {
            if (!this.Outputs) return;
            if (!_resources.Any()) return;
            if (HostFeature == null) return;

            var adjacentDevicesThatReceive = new[] {
                Crypt[HostFeature.Position + new Point(0, -1)],
                Crypt[HostFeature.Position + new Point(1, 0)],
                Crypt[HostFeature.Position + new Point(0, 1)],
                Crypt[HostFeature.Position + new Point(-1, 0)],
            }
                .Select(f => f.Device)
                .Where(d => d != null)
                .Where(d => d.Receives)
                .ToList();

            if (!adjacentDevicesThatReceive.Any())
            {
                Crypt.WasteResources(_resources);
                _resources.Clear();
            }

            while (adjacentDevicesThatReceive.Any(d => d.Pressure < this.Pressure) && this._resources.Any())
            {
                var lowest = adjacentDevicesThatReceive.OrderBy(a => a.Pressure).First();
                var resource = _resources.First();
                this._resources.Remove(resource);
                lowest.Push(resource);
            }
        }

        public class HellFireReceiver : CryptDevice
        {
            public HellFireReceiver(Crypt crypt) : base(crypt, 1, receives: false, outputs: true) { }
        }

        public class SoulReceiver : CryptDevice
        {
            public SoulReceiver(Crypt crypt) : base(crypt, 1, receives: false, outputs: true) { }
        }

        public class Pipe : CryptDevice
        {
            public Pipe(Crypt crypt) : base(crypt, 6, receives: true, outputs: true) { }
        }

        public class CryptCoinEngine : CryptDevice
        {
            public CryptCoinEngine(Crypt crypt) : base(crypt, 12, receives: true, outputs: true) { }

            public override void ProcessSupply()
            {
                var hellFireUnits = new Queue<HellFire>(_resources.OfType<HellFire>());
                var soulUnits = new Queue<Soul>(_resources.OfType<Soul>());
                while (hellFireUnits.Count > 0 && soulUnits.Count > 0)
                {
                    _resources.Remove(hellFireUnits.Dequeue());
                    _resources.Remove(soulUnits.Dequeue());
                    _resources.Add(new CryptCoin());
                }
            }
        }

        public class CryptCoinExchange : CryptDevice
        {
            public CryptCoinExchange(Crypt crypt) : base(crypt, 0, receives: true, outputs: false) { }

            public override void ProcessSupply()
            {
                var cryptCoins = this._resources.OfType<CryptCoin>().ToList();
                Crypt.DepositCoins(cryptCoins.Count());
                cryptCoins.ForEach(c => _resources.Remove(c));
            }
        }

        public TResult Match<TResult>(
            Func<HellFireReceiver, TResult> hellFireReceiver,
            Func<SoulReceiver, TResult> soulReceiver,
            Func<Pipe, TResult> pipe,
            Func<CryptCoinEngine, TResult> cryptCoinEngine,
            Func<CryptCoinExchange, TResult> cryptCoinExchange)
        {
            if (this is HellFireReceiver hr) return hellFireReceiver(hr);
            if (this is SoulReceiver sr) return soulReceiver(sr);
            if (this is Pipe p) return pipe(p);
            if (this is CryptCoinExchange exchange) return cryptCoinExchange(exchange);
            if (this is CryptCoinEngine engine) return cryptCoinEngine(engine);
            throw new Exception("map the type here");
        }

        public CryptFeature[] GetFeaturesThisWouldOccupyAtPosition(Crypt crypt, Point position)
        {
            return new[] { crypt[position] };
        }
    }

    public interface IResource
    {
    }

    public struct Soul : IResource
    {
    }

    public struct HellFire : IResource
    {
    }

    public struct CryptCoin : IResource
    {
    }
}
