using System.Collections.Generic;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.Shared.Models
{
    public class Avatar
    {
        public int UserId { get; set; }

        public int Wood { get; set; }

        public int Ore { get; set; }

        public int Gold { get; set; }

        public int Gems { get; set; }

        public Team Team { get; set; }

        private string _visitedString;

        public string VisitedString
        {
            get
            {
                return this._visitedString;
            }
            set
            {
                if (value != null)
                {
                    this._visitedString = value;
                    this.Visited = Common.ParseCsvIds(this._visitedString);
                }
            }
        }

        /// <summary>
        /// Adding visitor directly to this list won't be recorded in the database.
        /// Please use AddVisitor method.
        /// </summary>
        public List<int> Visited { get; private set; }

        public void AddVisited(int visitorId)
        {
            if (!this.Visited.Contains(visitorId))
            {
                this.Visited.Add(visitorId);
                this._visitedString = string.Join(",", this.Visited);
            }
        }


    }
}