using IV_Play.Model;
using System;
using System.Linq;
using System.Windows.Controls;

namespace IV_Play.View
{
    class Navigation
    {

        private ListView _gameList;

        public Navigation(ListView listView)
        {
            _gameList = listView;
        }

        internal void GoToPreviousCharacter()
        {
            var currentMachine = (Machine)_gameList.SelectedItem;

            if (currentMachine == null)
                return;

            currentMachine = GoToParent(currentMachine);

            char nextKey;
            char key = Char.ToLower(currentMachine.description[0]);

            nextKey = key == 'a' ? '9' : Char.ToLower((char)(key - 1));
            var parents = from Machine p in _gameList.Items where p.cloneof == null select p;
            
            while (true)
            {
                var machines = (from m in parents where char.ToLower(m.description[0]) == nextKey select m);
                if (machines.Count() > 0)
                {
                    ScrollTo(machines.First());
                    return;
                }

                if (nextKey == '0' - 1)
                {
                    nextKey = '(';
                }
                else if (nextKey == '(' - 1)
                {
                    nextKey = 'z';                    
                }
                else
                    nextKey--;
            }
        }
        
        internal void GoToNextLetter()
        {

            var currentMachine = (Machine)_gameList.SelectedItem;

            if (currentMachine == null)
                return;

            currentMachine = GoToParent(currentMachine);

            char nextKey;
            char key = Char.ToLower(currentMachine.description[0]);

            nextKey = key == '9' ? 'a' : Char.ToLower((char)(key + 1));
            var parents = from Machine p in _gameList.Items where p.cloneof == null select p;

            while (true)
            {
                var machines = (from m in parents where char.ToLower(m.description[0]) == nextKey select m);
                if (machines.Count() > 0)
                {
                    ScrollTo(machines.First());
                    return;
                }

                if (nextKey == '(' + 1)
                {
                    nextKey = '0';
                }
                else if (nextKey == 'z' + 1)
                {
                    nextKey = '(';
                }
                else
                    nextKey++;
            }
        }

        private Machine GoToParent(Machine machine)
        {
            if (machine.cloneof == null) return machine;

            var index = _gameList.Items.IndexOf(machine);

            while (true)
            {
                var previousMachine = (Machine)_gameList.Items[--index];
                if (previousMachine.cloneof == null) return previousMachine;
            }
        }


        private void ScrollTo(Machine machine)
        {
            _gameList.ScrollIntoView(machine);
            _gameList.SelectedItem = machine;
            var item = _gameList.ItemContainerGenerator.ContainerFromItem(machine) as ListBoxItem;
            if (item != null)
            {
                item.Focus();
            }                       
        }
    }
}
