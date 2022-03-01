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
        public List<SaleTypeApi> SaleTypes = new List<SaleTypeApi>();
        public List<ActionTypeApi> ActionTypes = new List<ActionTypeApi>();

        private object _currentListPage;
        public object CurrentListPage
        {
            get { return _currentListPage; }
            set
            {
                _currentListPage = value;
                SignalChanged("CurrentListPage");
            }
        }

        private object _currentPage;
        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                SignalChanged("CurrentPage");
            }
        }

        public CustomCommand OpenProductOrderInView { get; set; }
        public CustomCommand OpenUnitView { get; set; }
        public CustomCommand OpenFabricatorView { get; set; }
        public CustomCommand OpenProductTypeView { get; set; }
        public CustomCommand OpenPhysicalClientView { get; set; }
        public CustomCommand OpenLegalClientView { get; set; }
        public CustomCommand OpenProductView { get; set; }
        public CustomCommand OpenOrderView { get; set; }
        public CustomCommand OpenOrderOutView { get; set; }

        public MainVM()
        {
            GetList();
            CurrentListPage = new ProductView();
            CurrentPage = new OrderOutView();

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
            OpenOrderView = new CustomCommand(() =>
            {
                CurrentListPage = new OrderView();
                SignalChanged("CurrentListPage");
            }); 
            OpenFabricatorView = new CustomCommand(() =>
            {
                CurrentListPage = new FabricatorView();
                SignalChanged("CurrentListPage");
            });
            OpenProductOrderInView = new CustomCommand(() =>
            {
                CurrentPage = new ProductOrderInView();
                SignalChanged("CurrentPage");
            });
            OpenOrderOutView = new CustomCommand(() =>
            {
                CurrentPage = new OrderOutView();
                SignalChanged("CurrentPage");
            });
        } 
      
        public void FirstStart()
        {
            if(SaleTypes.Count == 0 && ActionTypes.Count == 0)
            {
                AddSaleType(new SaleTypeApi { Title="Розничная" });
                AddSaleType(new SaleTypeApi { Title = "Оптовая" });

                AddActionType(new ActionTypeApi { Name = "Продажа" });
                AddActionType(new ActionTypeApi { Name = "Возврат" });
                AddActionType(new ActionTypeApi { Name = "Переоценка" });
                AddActionType(new ActionTypeApi { Name = "Поступление" });
                AddActionType(new ActionTypeApi { Name = "Списание" });
            }
        }
        private async Task AddSaleType(SaleTypeApi saleType)
        {
            var id = await Api.PostAsync<SaleTypeApi>(saleType, "SaleType");
        }
        private async Task AddActionType(ActionTypeApi actionType)
        {
            var id = await Api.PostAsync<ActionTypeApi>(actionType, "ActionType");
        }
        private async Task GetList()
        {
            SaleTypes = await Api.GetListAsync<List<SaleTypeApi>>("SaleType");
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            FirstStart();
        }
    }
}
