using FullStackWork.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FullStackWork.Store
{
    public class NavigationStore
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set 
            {
                _currentViewModel = value;
                OnCurrentVMChanged();
            }
        }
        public event Action CurrentVMChanged;
        private void OnCurrentVMChanged()
        {
            CurrentVMChanged?.Invoke();
        }
    }
}
