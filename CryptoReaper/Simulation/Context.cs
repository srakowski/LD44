using CryptoReaper.Simulation.BuildingFeatures;
using System;
using System.Collections.Generic;

namespace CryptoReaper.Simulation
{
    class Context
    {
        private readonly List<Crypt> _buildings = new List<Crypt>();

        public Context()
        {
            AddStarterBuilding();
        }

        public IEnumerable<Crypt> Rooms => _buildings;

        private void AddStarterBuilding()
        {
            var features = new Crypt.Feature[3, 3];

            features[0, 0] = new Wall();
            features[0, 1] = new WallWithHellFirePortal();
            features[0, 2] = new Wall();

            features[1, 0] = new Wall();
            features[1, 1] = new OpenSpace();
            features[1, 2] = new Wall();

            features[2, 0] = new Wall();
            features[2, 1] = new WallWithSoulPortal();
            features[2, 2] = new Wall();


            var startRoom = new Crypt(features);
        }
    }
}
