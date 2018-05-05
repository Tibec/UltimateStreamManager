using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UltimateStreamMgr.ViewModel
{
    public abstract class DockWindowViewModel : BaseViewModel
    {
        #region Properties

        #region CloseCommand
        private ICommand _CloseCommand;
        public ICommand ToggleCommand
        {
            get
            {
                if (_CloseCommand == null)
                    _CloseCommand = new RelayCommand(() => ToggleVisibility());
                return _CloseCommand;
            }
        }
        #endregion

        #region IsVisible
        private bool _IsVisible;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                Set("IsVisible", ref _IsVisible, value);
                RaisePropertyChanged("IsVisible");
            }
        }
        #endregion


        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                Set("Title", ref _Title, value);
            }
        }
        #endregion

        #region ContentId

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                Set("ContentId", ref _contentId, value);
            }
        }

        #endregion

        #endregion

    public DockWindowViewModel()
    {
        IsVisible = true;
        ContentId = GetType().ToString();
    }

    public void ToggleVisibility()
    {
        // IsVisible value is already modified directly by the view
        // IsVisible = !IsVisible;
    }
  }
}
