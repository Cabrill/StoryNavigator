using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static StoryNavigator.DataTypes.DialogTreeRaw;

namespace StoryNavigator.DataTypes
{
    public class StoryData
    {
        private DialogTreeRaw _rawData { get; set; }

        public StoryData()
        {
            _rawData = new DialogTreeRaw();
            _rawData.RootObject = new Rootobject();
            _rawData.RootObject.creatorversion = GlobalConstants.VersionNumber;
        }

        public Passage[] Passages => _rawData.RootObject.passages;

        public Passage AddNewPassage(string passageName = "", int x = 250, int y = 250)
        {
            var allCurrentPassages = _rawData.RootObject.passages?.ToList() ?? new List<Passage>();

            var newPassage = new Passage();
            newPassage.pid = GetNextPassageId();
            newPassage.position = new Position();
            newPassage.position.x = x;
            newPassage.position.y = y;
            newPassage.name = !passageName.IsNullOrWhitespace() ? passageName : $"Passage #{newPassage.pid} name";

            newPassage.links = new Link[0];

            allCurrentPassages.Add(newPassage);
            _rawData.RootObject.passages = allCurrentPassages.ToArray();

            return newPassage;
        }

        private int GetNextPassageId()
        {
            if (_rawData.RootObject.passages?.Count() > 0)
            {
                return _rawData.RootObject.passages.Max(p => p.pid) + 1;
            }
            else
            {
                return 1;
            }
        }
    }
}
