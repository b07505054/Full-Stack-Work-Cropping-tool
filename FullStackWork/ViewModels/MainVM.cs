using FullStackWork.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FullStackWork.ViewModels 
{
    public class MainVM : ViewModelBase
    {
        #region
        private int _tabSelectindex = -1;
        public int TabSelectIndex
        {
            get { return _tabSelectindex; }
            set 
            { 
                _tabSelectindex = value;
                Change();
            }
         }
        private readonly NavigationStore _navigationStore;
        public ViewModelBase CurrentVM => _navigationStore.CurrentViewModel;

        #endregion

        public MainVM(NavigationStore navigationStore) 
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentVMChanged += OnCurrentVMChanged;
        }
        private void OnCurrentVMChanged()
        {
            OnPropertyChanged(nameof(CurrentVM));
        }
        public void Change()
        {
            switch(TabSelectIndex){
                case 0:
                    _navigationStore.CurrentViewModel = new ImagePageVM();
                 break;
                case 1:
                    _navigationStore.CurrentViewModel = new VideoPageVM();
                    break;
                 case 2:
                    _navigationStore.CurrentViewModel = new IPCameraPageVM();
                    break;  
            }
            
        }
    }
}
