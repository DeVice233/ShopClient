using ModelsApi;
using ShopClient.Core;
using ShopClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopClient
{
    public class MainVM : BaseViewModel
    {
        private object _currentPage;
        public object CurrentListPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                SignalChanged("CurrentPage");
            }
        }

        public CustomCommand OpenUnitView { get; set; }
        public CustomCommand OpenProductTypeView { get; set; }
        public CustomCommand OpenPhysicalClientView { get; set; }
        public CustomCommand OpenLegalClientView { get; set; }
        public CustomCommand OpenProductView { get; set; }

        public MainVM()
        {
            CurrentListPage = new UnitView();

            OpenUnitView = new CustomCommand(()=>
            {
                CurrentListPage = new UnitView();
                SignalChanged("CurrentListPage");
            });
            OpenProductTypeView = new CustomCommand(() =>
            {
                CurrentListPage = new ProductTypeView();
                SignalChanged("CurrentListPage");
            });
            OpenPhysicalClientView = new CustomCommand(() =>
            {
                CurrentListPage = new PhysicalClientView();
                SignalChanged("CurrentListPage");
            });
            OpenLegalClientView = new CustomCommand(() =>
            {
                CurrentListPage = new LegalClientView();
                SignalChanged("CurrentListPage");
            });
            OpenProductView = new CustomCommand(() =>
            {
                CurrentListPage = new ProductView();
                SignalChanged("CurrentListPage");
            });
        } 
      
    }
}
