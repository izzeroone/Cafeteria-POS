using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.Helper.PrintHelper.Report;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper.Report;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for OrderNotePage.xaml
    /// </summary>
    public partial class OrderNotePage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly List<OrderNoteDetail> _ordernotedetailslist;
        private readonly List<OrderNote> _ordernotelist;
        private List<Product> _proList;
        private List<OrderNote> filtero = new List<OrderNote>();

        private List<OrderNoteDetail> filterod = new List<OrderNoteDetail>();
        private bool isRaiseEvent = true;

        public OrderNotePage(BusinessModuleLocator businessModuleLocator, AdminRe admin)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            _ordernotelist = _businessModuleLocator.OrderModule.getOrdernoteList().ToList();
            _ordernotedetailslist = _businessModuleLocator.RepositoryLocator.OrderDetailsRepository
                .Get(includeProperties: "Product").ToList();
            var orderdetailsTempList = new List<OrderNoteDetail>();
            foreach (var orderdetails in _ordernotedetailslist)
            {
                var found = false;
                foreach (var order in _ordernotelist)
                    if (orderdetails.OrdernoteId.Equals(order.OrdernoteId))
                    {
                        found = true;
                        break;
                    }

                if (found) orderdetailsTempList.Add(orderdetails);
            }

            _ordernotedetailslist = orderdetailsTempList;
            lvOrderNoteDetails.ItemsSource = _ordernotedetailslist;

            lvOrderNote.ItemsSource = _ordernotelist;

            Loaded += OrderNotePage_Loaded;
        }

        private void OrderNotePage_Loaded(object sender, RoutedEventArgs e)
        {
            _proList = _businessModuleLocator.ProductModule.getAllProduct().ToList();
            initData();
        }

        private void initData()
        {
            isRaiseEvent = false;
            var prol = new List<dynamic>();
            prol.Add(new {Id = "--", Name = "--"});
            foreach (var p in _proList) prol.Add(new {Id = p.ProductId, p.Name});

            cboProduct.ItemsSource = prol;
            cboProduct.SelectedValuePath = "Id";
            cboProduct.DisplayMemberPath = "Name";
            cboProduct.SelectedValue = "--";
            isRaiseEvent = true;
        }

        private void lvOrderNote_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var odn = lvOrderNote.SelectedItem as OrderNote;
            if (odn != null)
                lvOrderNoteDetails.ItemsSource = _businessModuleLocator.OrderModule.getOrderNoteDetail(odn.OrdernoteId);
            else
                lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
        }

        private void cboProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filtero = new List<OrderNote>();
            lvOrderNote.UnselectAll();
            lvOrderNoteDetails.UnselectAll();
            if (isRaiseEvent)
            {
                var cbopro = sender as ComboBox;
                var proid = cbopro.SelectedValue.ToString();
                if (!proid.Equals("--"))
                {
                    filterod = _ordernotedetailslist.Where(x => x.ProductId.Equals(proid)).ToList();
                    var odd = filterod.GroupBy(x => x.OrdernoteId).Select(y => y.ToList()).ToList();

                    foreach (var i in odd)
                    foreach (var j in i)
                    {
                        filtero.Add(_ordernotelist.Where(x => x.OrdernoteId.Equals(j.OrdernoteId)).FirstOrDefault());
                        break;
                    }

                    if (filtero.Count != 0 && pickOrderDate.SelectedDate == null)
                    {
                        lvOrderNote.ItemsSource = filtero;
                        lvOrderNote.Items.Refresh();
                        lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                        lvOrderNoteDetails.Items.Refresh();
                    }
                    else if (filtero.Count != 0 && pickOrderDate.SelectedDate != null)
                    {
                        lvOrderNote.ItemsSource = filtero.Where(x =>
                            x.OrderTime.ToShortDateString()
                                .Equals(((DateTime) pickOrderDate.SelectedDate).ToShortDateString())).ToList();
                        lvOrderNote.Items.Refresh();
                        lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                        lvOrderNoteDetails.Items.Refresh();
                    }
                    else
                    {
                        lvOrderNote.ItemsSource = new List<OrderNote>();
                        lvOrderNote.Items.Refresh();
                        lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                        lvOrderNoteDetails.Items.Refresh();
                    }
                }
                else
                {
                    if (pickOrderDate.SelectedDate == null)
                    {
                        lvOrderNote.ItemsSource = _ordernotelist;
                        lvOrderNote.Items.Refresh();
                        lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                        lvOrderNoteDetails.Items.Refresh();
                    }
                    else
                    {
                        lvOrderNote.ItemsSource = _ordernotelist.Where(x =>
                            x.OrderTime.ToShortDateString()
                                .Equals(((DateTime) pickOrderDate.SelectedDate).ToShortDateString())).ToList();
                        lvOrderNote.Items.Refresh();
                        lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                        lvOrderNoteDetails.Items.Refresh();
                    }
                }
            }
        }

        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void lvOrderNoteDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void pickOrderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var pick = sender as DatePicker;
            if (pick.SelectedDate == null) return;

            if (cboProduct.SelectedValue.Equals("--"))
            {
                lvOrderNote.ItemsSource = _ordernotelist.Where(x =>
                    x.OrderTime.ToShortDateString().Equals(((DateTime) pick.SelectedDate).ToShortDateString()));
                lvOrderNote.Items.Refresh();
                lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                lvOrderNoteDetails.Items.Refresh();
            }
            else
            {
                if (filtero.Count != 0)
                {
                    lvOrderNote.ItemsSource = filtero.Where(x =>
                        x.OrderTime.ToShortDateString().Equals(((DateTime) pick.SelectedDate).ToShortDateString()));
                    lvOrderNote.Items.Refresh();
                }
                else
                {
                    lvOrderNote.ItemsSource = new List<OrderNote>();
                    lvOrderNote.Items.Refresh();
                }

                lvOrderNoteDetails.ItemsSource = new List<OrderNoteDetail>();
                lvOrderNoteDetails.Items.Refresh();
            }
        }

        private void BtnOverViewReport_OnClick(object sender, RoutedEventArgs e)
        {
            var optionDialog = new ReportOptionDialog(new OrderNoteReport(), _businessModuleLocator);
            optionDialog.Show();
        }
    }
}