using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Shared.Models.Units;
using Newtonsoft.Json;

namespace Assets.Scripts.Shared.Models
{
    public class Game
    {
        public int Id { get; set; }

        public IList<Army> Armies { get; set; }

        public IList<Dwelling> Dwellings { get; set; }

        public IList<Avatar> Avatars { get; set; }

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

            int height = lines.Length;
            int width = lines[0].Length;

            int[,] parsedMatrix = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    parsedMatrix[y, x] = (int)char.GetNumericValue(line[x]);
                }
            }

            return parsedMatrix;
        }
    }

    public static class GameExtensions
    {
        public static Army GetArmy(this Game game, int armyId)
        {
            return game.Armies.FirstOrDefault(x => x.Id == armyId);
        }

        public static Unit GetUnit(this Game game, int unitId)
        {
            return game.Armies.FirstOrDefault(x => x.Units.Any(u => u.Id == unitId)).Units.FirstOrDefault(x => x.Id == unitId);
        }
    }
}
