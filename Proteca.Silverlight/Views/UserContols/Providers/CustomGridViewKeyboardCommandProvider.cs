using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Proteca.Silverlight.Views.UserContols.Providers
{
    public class CustomGridViewKeyboardCommandProvider : DefaultKeyboardCommandProvider
    {
        private GridViewDataControl parentGrid;

        public CustomGridViewKeyboardCommandProvider(GridViewDataControl grid) : base(grid)
        {
            this.parentGrid = grid;
        }

        public override IEnumerable<ICommand> ProvideCommandsForKey(Key key)
        {
            List<ICommand> commandsToExecute = base.ProvideCommandsForKey(key).ToList();

            switch (key){
                case Key.Insert : commandsToExecute.Remove(RadGridViewCommands.BeginInsert);
                    break;
                case Key.Delete: commandsToExecute.Remove(RadGridViewCommands.Delete);
                    break;
                default : break;
            }

            return commandsToExecute;
        }

    }
}
