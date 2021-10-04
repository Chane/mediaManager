using System.Collections.ObjectModel;

namespace UI.ViewModels
{
    public class TreeNodeViewModel
    {
        public string Directory { get; set; } = string.Empty;

        public ObservableCollection<TreeNodeViewModel> Directories { get; } = new();
    }
}
