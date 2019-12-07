using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryNavigator.DataClasses
{
    public class NodeTree
    {
        public class Rootobject
        {
            public Passage[] passages { get; set; }
            public string name { get; set; }
            public string startnode { get; set; }
            public string creator { get; set; }
            public string creatorversion { get; set; }
            public string ifid { get; set; }
        }

        public class Passage
        {
            public string text { get; set; }
            public Link[] links { get; set; }
            public string name { get; set; }
            public int pid { get; set; }
            public string[] tags { get; set; }

            public Position position { get; set; }
        }

        public class Link
        {
            public string name { get; set; }
            public string link { get; set; }
            public int pid { get; set; }
        }

        public class Position
        {
            public int x { get; set; }
            public int y { get; set; }
        }
    }
}
