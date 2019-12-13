using StoryNavigator.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoryNavigator.DataTypes.DialogTreeRaw;

namespace StoryNavigator.ViewModels
{
    public class NodePassageViewModel : ViewModel
    {

        public Passage DataContext
        {
            get; set;
        }

        [DependsOn(nameof(DataContext))]
        public string PassageName => DataContext?.name;

        [DependsOn(nameof(DataContext))]
        public string PassageText => DataContext?.text;

        [DependsOn(nameof(DataContext))]
        public string PassageNumber => (DataContext?.pid ?? 0).ToString();

        public List<NodePassageLinkViewModel> PassageLinkViewModels { get; set; } = new List<NodePassageLinkViewModel>();
        
        public string[] tags { get; set; }

        public Position position { get; set; }
    }

    public NodePassageViewModel(Passage passage)
        {
            _passage = passage;
        }

    }
}
