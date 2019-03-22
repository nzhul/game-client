using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Shared.DataModels
{
    public class Region
    {
        public int id;
        public string name;
        public int level;
        public int realmId;
        //public string matrixString;
        public IList<Room> rooms;
        public IList<Hero> heroes;
        public IList<MonsterPack> monsterPacks;
        public IList<Dwelling> dwellings;

        private string _matrixString;

        public string MatrixString
        {
            get
            {
                return this._matrixString;
            }
            set
            {
                if (value != null)
                {
                    this._matrixString = value;
                    this.Matrix = this.ParseMatrix(this._matrixString);
                }
            }
        }

        [JsonIgnore]
        public int[,] Matrix { get; private set; }

        private int[,] ParseMatrix(string matrixString)
        {
            string[] lines = matrixString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int[,] parsedMatrix = new int[lines[0].Length, lines.Length];
            for (int row = 0; row < lines.Length; row++)
            {
                string line = lines[row];
                for (int col = 0; col < line.Length; col++)
                {
                    parsedMatrix[col, row] = (int)char.GetNumericValue(line[col]);
                }
            }

            return parsedMatrix;
        }
    }
}
