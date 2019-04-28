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
        public const int CryptFeaturesDimPerSection = 20;
        public const int CryptFeaturesMidPoint = CryptFeaturesDimPerSection / 2;

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
            CryptCoinBalance = 100;
            WastedCryptCoins = 0;
            WastedHellFire = 0;
            WastedSouls = 0;
        }

        public int CryptCoinBalance { get; private set; }

        public int WastedCryptCoins { get; private set; }

        public int WastedHellFire { get; private set; }

        public int WastedSouls { get; private set; }

        public IEnumerable<CryptSection> CryptSections => _sections.Values;

        internal void RemoveDeviceAt(Point cryptPos)
        {
            var dev = this[cryptPos].Device;
            if (dev != null)
            {
                _devices.Remove(dev);
                this[cryptPos].RemoveDevice();
            }
        }

        public CryptFeature this[Point point] => this[point.Y, point.X];

        public CryptFeature this[int row, int col] => _features.TryGetValue(new Point(col, row).GetKey(), out var feature)
            ? feature : new CryptFeature.Void(null, null, new Point(col, row)) as CryptFeature;

        public void Step()
        {
            var devices = _devices.OrderByDescending(r => r.ResourceQuantity).ToArray();
            foreach (var device in _devices)
                device.PullResources();
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
                cryptCoinEngine: _ => features.All(f => f is CryptFeature.Floor)
            );
        }

        public CryptSection GetSection(int row, int col) =>
            _sections.TryGetValue(new Point(col, row).GetKey(), out var section)
                ? section
                : null;

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

        public void GenerateCryptSection(int row, int col)
        {
            var sectionPosition = new Point(col, row);
            var section = _sections[sectionPosition.GetKey()] = new CryptSection(this, sectionPosition);
            ForAllRowsAndCols(CryptFeaturesDimPerSection, GenerateCryptFeaturesForSection(section));
            GenerateSpires(section);
        }

        private Action<int, int> GenerateCryptFeaturesForSection(CryptSection section) =>
            (row, col) =>
            {
                var featurePosition = section.SectionPositionToCryptPosition(row, col);

                var featureRoll = _random.Next(100);

                var chanceHardRock = (Min(Abs(row - CryptFeaturesMidPoint), Abs(col - CryptFeaturesMidPoint)) * 100) / CryptFeaturesMidPoint;
                chanceHardRock = chanceHardRock < 50 ? 0 : chanceHardRock;

                var chanceSoftRock = (Min(Abs(row - CryptFeaturesMidPoint), Abs(col - CryptFeaturesMidPoint)) * 100) / CryptFeaturesMidPoint;
                chanceSoftRock = chanceSoftRock < 25 ? 0 : chanceSoftRock;

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

            //if (section.Position.X == 0 && section.Position.Y == 0)
            //{
            //    var cryptCoinSpire = new CryptFeature.CryptCoinSpire(this, section, centerPoint);
            //    _features[centerPoint.GetKey()] = cryptCoinSpire;
            //}

            var softFeatureQueue = new Queue<CryptFeature>(section.GetCryptFeatures()
                .Where(f => !(f is CryptFeature.HardRock) && !(f is CryptFeature.Spire))
                .Where(f =>
                {
                    var cryptPosition = section.CryptPositionToSectionPosition(f.Position);
                    var rowPos = (Abs(cryptPosition.Y - CryptFeaturesMidPoint) * 100) / CryptFeaturesMidPoint;
                    var colPos = (Abs(cryptPosition.X - CryptFeaturesMidPoint) * 100) / CryptFeaturesMidPoint;
                    return rowPos < 50 && rowPos > 10 && colPos < 50 && colPos > 10;
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

        internal void WasteResources(int hellFire, int souls, int coins)
        {
            WastedHellFire += hellFire;
            WastedSouls += souls;
            WastedCryptCoins += coins;
        }

        internal void DepositCoins(int quantity)
        {
            CryptCoinBalance += quantity;
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
            for (int row = 0; row < Crypt.CryptFeaturesDimPerSection; row++)
                for (int col = 0; col < Crypt.CryptFeaturesDimPerSection; col++)
                {
                    var featurePosition = SectionPositionToCryptPosition(row, col);
                    yield return Crypt[featurePosition.Y, featurePosition.X];
                }
        }

        public Point SectionPositionToCryptPosition(int row, int col) =>
            (new Point(col, row) + (this.Position * new Point(Crypt.CryptFeaturesDimPerSection, Crypt.CryptFeaturesDimPerSection)));

        public Point CryptPositionToSectionPosition(Point cryptPosition) =>
            (cryptPosition - (this.Position * new Point(Crypt.CryptFeaturesDimPerSection, Crypt.CryptFeaturesDimPerSection)));
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

        internal void RemoveDevice()
        {
            if (this.Device == null) return;
            this.Device = null;
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
        }

        public class HellFireSpire : SpawningSpire
        {
            public HellFireSpire(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
            }
        }

        public class SoulSpire : SpawningSpire
        {
            public SoulSpire(Crypt crypt, CryptSection cryptSection, Point cryptPosition) : base(crypt, cryptSection, cryptPosition)
            {
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
        private int _fireQty;
        private int _soulQty;
        private int _pullIdx;

        private CryptDevice(Crypt crypt, int resourceCapacity)
        {
            Crypt = crypt;
            ResourceCapacity = resourceCapacity;
        }

        public int ResourceQuantity => _fireQty + _soulQty;

        public int FireQuantity => _fireQty;

        public int SoulQuantity => _soulQty;

        public Crypt Crypt { get; }

        public int ResourceCapacity { get; }

        public Func<ResourceType, bool> Receives { get; }

        public bool Outputs { get; }

        public CryptFeature HostFeature { get; private set; }

        public void SetHostFeature(CryptFeature feature) => HostFeature = feature;

        private void GetOneQty(Action fire, Action soul)
        {
            if (_fireQty == 0 && _soulQty == 0)
                throw new Exception("nope");

            _pullIdx++;
            while (true)
            {
                if (_fireQty > 0 && _pullIdx % 2 == 0)
                {
                    _fireQty--;
                    fire();
                    return;
                }
                else if (_soulQty > 0 && _pullIdx % 2 == 1)
                {
                    _soulQty--;
                    soul();
                    return;
                }
                _pullIdx++;
            }
        }

        public virtual void PullResources()
        {
            var devices = new[] {
                    Crypt[HostFeature.Position + new Point(0, -1)],
                    Crypt[HostFeature.Position + new Point(1, 0)],
                    Crypt[HostFeature.Position + new Point(0, 1)],
                    Crypt[HostFeature.Position + new Point(-1, 0)],
                }
                .Select(f => f.Device)
                .Where(d => d != null)
                .Where(d => d.ResourceQuantity > this.ResourceQuantity)
                .OrderByDescending(d => d.ResourceQuantity)
                .ToArray();

            if (!devices.Any()) return;

            var top = devices.First();
            while ((top.ResourceQuantity - 1) > this.ResourceQuantity)
            {
                top.GetOneQty(
                    fire: () => _fireQty++,
                    soul: () => _soulQty++
                );
                top = devices.OrderByDescending(d => d.ResourceQuantity).First();
            }
        }

        public class HellFireReceiver : CryptDevice
        {
            public HellFireReceiver(Crypt crypt) : base(crypt, 60) { }

            public override void PullResources()
            {
                if (ResourceQuantity >= ResourceCapacity) return;
                this._fireQty += 10;
            }
        }

        public class SoulReceiver : CryptDevice
        {
            public SoulReceiver(Crypt crypt) : base(crypt, 60) { }

            public override void PullResources()
            {
                if (ResourceQuantity >= ResourceCapacity) return;
                this._soulQty += 10;
            }
        }

        public class Pipe : CryptDevice
        {
            public Pipe(Crypt crypt) : base(crypt, 60) { }
        }

        public class CryptCoinEngine : CryptDevice
        {
            public CryptCoinEngine(Crypt crypt) : base(crypt, 60) { }

            public override void PullResources()
            {
                base.PullResources();
                while (_soulQty > 0 && _fireQty > 0)
                {
                    _soulQty--;
                    _fireQty--;
                    Crypt.DepositCoins(1);
                }
                Crypt.WasteResources(_fireQty, _soulQty, 0);
                _soulQty = 0;
                _fireQty = 0;
            }
        }

        public TResult Match<TResult>(
            Func<HellFireReceiver, TResult> hellFireReceiver,
            Func<SoulReceiver, TResult> soulReceiver,
            Func<Pipe, TResult> pipe,
            Func<CryptCoinEngine, TResult> cryptCoinEngine)
        {
            if (this is HellFireReceiver hr) return hellFireReceiver(hr);
            if (this is SoulReceiver sr) return soulReceiver(sr);
            if (this is Pipe p) return pipe(p);
            if (this is CryptCoinEngine engine) return cryptCoinEngine(engine);
            throw new Exception("map the type here");
        }

        public CryptFeature[] GetFeaturesThisWouldOccupyAtPosition(Crypt crypt, Point position)
        {
            return new[] { crypt[position] };
        }
    }

    public enum ResourceType
    {
        Fire,
        Soul,
    }
}
